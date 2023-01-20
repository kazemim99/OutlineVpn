using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Renci.SshNet;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using V2Ray.Api.Database;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.V2Keys.Dto;
using static V2Ray.Api.Services.Server.ServerService;

namespace V2Ray.Api.Services.V2Keys
{
    public class V2KeyService : BaseService<V2Key,
        int,
        UpdateV2KeyInput,
        CreateV2KeyInput,
        GetV2KeyOutput,
        GetV2KeyListOutput,
        V2KeyFilterInput>,
        IV2KeyService
    {
        private readonly DB _db;
        public static Dictionary<string, UserKeyDetailsOutput> _userDetail = new Dictionary<string, UserKeyDetailsOutput>();
        private static List<Obj> Objs;
        private readonly IHttpClientFactory _httpClientFactory;
        public V2KeyService(DB db, IMapper mapper, IHttpClientFactory httpClientFactory) : base(mapper, db)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<Pagination<GetV2KeyListOutput>> GetAllFromXUIServerAsync(V2KeyFilterInput input)
        {
            var server = _db.V2Servers.First(a => a.Id == input.ServerId);
            List<Obj> root = new List<Obj>();

            var httpClient = await GetCookie(server.UserName, server.Password, "", server.Url, server.Port);
            if (!root.Any())
                root = await GetServerKeys(server.IPs, 2, httpClient);

            var result = root.Select(a =>
            {
                var settin = JsonConvert.DeserializeObject<Setting>(a.settings);
                return new GetV2KeyListOutput
                {
                    ExpireDate = a.expiryTime.ToString(),
                    ClientId = settin.clients.First().id,
                    PrimaryCapacity = a.total,
                    State = a.enable,
                    Id = a.id,
                    Title = a.remark,
                    UsedCapacity = a.down,
                    User = "user"
                };
            }).ToList();

            return new Pagination<GetV2KeyListOutput>
            {
                Result = result,
                CurrentPage = input.Page,
                PageCount = input.ItemsPerPage,
                TotalItems = root.Count()
            };
        }
        bool tryAgain = true;

        public async Task SwapServerKeysAsync(SwapServerKeysInput input)
        {
            var server = _db.V2Servers.First(a => a.Id == input.FromServerId);
            List<Obj> root = new List<Obj>();
            var httpClient = await GetCookie(server.UserName, server.Password, server.IPs, server.Url, server.Port);
            if (!root.Any())
                root = GetServerKeys(server.IPs, server.Port, httpClient).Result.ToList();

            var server2 = _db.V2Servers.First(a => a.Id == input.ToServerId);
            httpClient = await GetCookie(server.UserName, server.Password, server.IPs, server.Url, server.Port);
            var root2 = await GetServerKeys(server2.IPs, server.Port, httpClient);
            var keys = root.Where(c => root2.All(b => b.port != c.port)).ToList();
            if (!keys.Any()) return;
            int exceptionCount = 0;
            foreach (var item in keys)
            {
                try
                {
                    await GenerateKey(server2.IPs, server2.Port, item, httpClient, true);
                }
                catch (Exception ex)
                {
                    exceptionCount++;
                    if (exceptionCount < 7)
                        continue;
                    throw new ApiException(ex.Message);
                }
            }
            tryAgain = false;
            if (exceptionCount == 0)
            {
                server2.Swapped = true;
                _db.V2Servers.Update(server2);
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new ApiException($"تعداد {exceptionCount} سرور جابجا نشد");

            }
        }

        public async Task DeleteKey(int keyId)
        {
            var key = _db.V2Keys.Include(a => a.V2Server).First(a => a.Id == keyId);
            var server = key.V2Server;

            if (key == null)
                throw new ApiException("کلید یافت نشد");

            var ips = server.IPs.Split(',').ToList();
            var removeFromAllServers = 0;
            foreach (var ip in ips)
            {
                var httpClient = await GetCookie(server.UserName, server.Password, ip, server.Url, server.Port);
                var result = await httpClient.PostAsync($"https://{ip}:{server.Port}/xui/inbound/del/{key.KeyId}", null);
                result.EnsureSuccessStatusCode();
                var tt = await result.Content.ReadAsStringAsync();
                removeFromAllServers++;
            }
            if (removeFromAllServers != ips.Count)
                throw new ApiException("در تمام سرورها اعمال نشد");

            _db.V2Keys.Remove(key);
            _db.SaveChanges();
        }

        public async Task UpdateKey(int keyId, int traffic, DateTime? expireDate, bool enable)
        {
            var server = _db.V2Servers.FirstOrDefault(a => a.IsMain);
            var key = _db.V2Keys.FirstOrDefault(a => a.Id == keyId);

            if (key == null)
                throw new ApiException("کلید یافت نشد");

            var ips = server.IPs.Split(',').ToList();

            var httpClient = await GetCookie(server.UserName, server.Password, ips.First(), server.Url, server.Port);
            var keys = await FetchKeysFromServer(ips.First(), server.Port, httpClient);

            var keyModified = keys.obj.First(a => a.id == key.KeyId);

            keyModified.enable = enable;
            key.State = enable;
            key.Traffic = traffic;
            keyModified.total = traffic.GigaByteToBytes();
            if (enable && expireDate != null)
            {
                key.ExpireDate = expireDate.Value.ToGeo().ToTimeStamp();
                keyModified.expiryTime = expireDate.Value.ToGeo().ToTimeStamp();
            }
            var ipsCount = 0;
            foreach (var ip in ips)
            {
                httpClient = await GetCookie(server.UserName, server.Password, ip, server.Url, server.Port);
                await UpdateKey(key.KeyId, ip, server.Port, keyModified, httpClient);
                ipsCount++;
            }
            if (ipsCount != ips.Count)
                throw new ApiException("در تمام سرورها اعمال نشد");
            _db.V2Keys.Update(key);
            _db.SaveChanges();
        }

        public override async Task InsertAsync(CreateV2KeyInput input)
        {
            try
            {
                var server = _db.V2Servers.FirstOrDefault(a => a.Id == input.ServerId);
                if (input.MainServer)
                {
                    server = _db.V2Servers.First(a => a.IsMain);
                }
                var ips = server.IPs.Split(',').ToList();

                string guid = "";
                int port = 0;
                int _keyId = 0;
                for (int i = 1; i <= input.Count; i++)
                {
                    port = GeneratePort(_db);
                    var sampleKey = new Obj();
                    foreach (var ip in ips)
                    {
                        server.IPs = ip;
                        var httpClient = await GetCookie(server.UserName, server.Password, ip, server.Url, server.Port);
                        sampleKey = GetServerSampleKey(server, httpClient, out _keyId);

                        try
                        {
                            if (guid.IsNullOrEmpty())
                            {
                                guid = Guid.NewGuid().ToString();
                                sampleKey.settings = Regex.Replace(sampleKey.settings,
                                                                  @"[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?",
                                                                  @$"{guid}",
                                                                  RegexOptions.IgnoreCase);
                            }
                            sampleKey.port = port;
                            sampleKey.id = null;
                            sampleKey.remark = "iranv2ray.com";
                            sampleKey.total = GigaByteToBytes(input.Traffic);
                            sampleKey.expiryTime = input.ExpireDate.ToTimeStamp();
                            sampleKey.down = 0;
                            sampleKey.up = 0;
                            if (sampleKey == null)
                                throw new Exception();
                            await GenerateKey(ip, server.Port, sampleKey, httpClient);



                        }

                        catch (Exception ex)
                        {
                            continue;
                        }
                    }

                    input.ClientPort = sampleKey.port;
                    input.Remark = sampleKey.remark;
                    input.ServerId = server.Id;
                    var set = JsonConvert.DeserializeObject<Setting>(sampleKey.settings);
                    var key = $"{sampleKey.protocol}://{set.clients.First().id}@{server.Url}:{sampleKey.port}";
                    key += $"?type=tcp&security=xtls&flow=xtls-rprx-direct#{sampleKey.remark}";
                    await File.AppendAllLinesAsync("keys.txt", new List<string>());
                    input.Key = key;
                    input.KeyId = ++_keyId;
                    await base.InsertAsync(input);
                }


            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task UpdateKey(int keyId, string ip, int port, Obj item, HttpClient httpClient, bool swap = false)
        {
            var formContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync($"https://{ip}:{port}/xui/inbound/update/{keyId}", formContent);
            result.EnsureSuccessStatusCode();

            var tt = await result.Content.ReadAsStringAsync();

            var serverResponse = JsonConvert.DeserializeObject<ServerResponse>(tt);
            if (!serverResponse.success)
            {
                if (serverResponse.msg.Contains(item.port.ToString()))
                {

                }
                else
                {
                    throw new ApiException(serverResponse.msg);
                }
            }

        }
        private async Task GenerateKey(string ip, int port, Obj item, HttpClient httpClient, bool swap = false)
        {
            Thread.Sleep(1000);
            var formContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync($"https://{ip}:{port}/xui/inbound/add", formContent);
            result.EnsureSuccessStatusCode();

            var tt = await result.Content.ReadAsStringAsync();

            var serverResponse = JsonConvert.DeserializeObject<ServerResponse>(tt);
            if (!serverResponse.success)
            {
                if (serverResponse.msg.Contains(item.port.ToString()))
                {

                }
                else
                {
                    throw new ApiException(serverResponse.msg);
                }
            }

        }

        private int GeneratePort(DB db)
        {
            var port = 0;
            do
            {
                port = new Random().Next(10000, 60000);

            }
            while (db.V2Keys.Any(a => a.Port == port));

            return port;
        }



        private Obj GetServerSampleKey(V2Server input, HttpClient httpClient, out int keyId)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var re = httpClient.PostAsJsonAsync($"https://{input.IPs}:{input.Port}/xui/inbound/list", new { }).Result;
            var ttt = re.Content.ReadAsStringAsync().Result;
            var obj = JsonConvert.DeserializeObject<Root>(ttt).obj;
            var key = obj.OrderByDescending(a => a.id).First(a => a.protocol == "vless");
            keyId = key.id.Value;
            return key;
        }
        public async Task<List<Obj>> GetServerKeys(string ip, int serverPort, HttpClient httpClient)
        {
            var root = await FetchKeysFromServer(ip, serverPort, httpClient);
            var objs = root.obj.OrderBy(a => a.remark).Where(a => a.protocol.Contains("vless")).ToList();

            return objs;
        }

        public async Task<Obj> GetServerKey(string ip, int keyId, int serverPort, HttpClient httpClient)
        {
            var root = await FetchKeysFromServer(ip, serverPort, httpClient);
            var obj = root.obj.FirstOrDefault(a => a.id == keyId);

            return obj;
        }

        private static async Task<Root> FetchKeysFromServer(string ip, int port, HttpClient httpClient)
        {
            var url = $"https://{ip}";
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var re = await httpClient.PostAsJsonAsync($"{url}:{port}/xui/inbound/list", new { }); ;
            var ttt = await re.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Root>(ttt);
        }

        private long GigaByteToBytes(long gigateBytes)
        {
            return gigateBytes * Convert.ToInt64(Math.Pow(1024, 3));
        }

        private async Task<HttpClient> GetCookie(string userName, string password, string ip, string url, int port, bool swap = false)
        {
            //input.Url = swap ? input.IP : input.Url;


            var formContent = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("username", userName),
    new KeyValuePair<string, string>("password",password)
});
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            handler.CookieContainer = cookies;
            HttpClient client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(10);

            ip = $"https://{ip}";
            var response = await client.PostAsync($"{ip}:{port}/login", formContent);
            cookies.Add(new Uri("https://" + url), new Cookie("domain", $"{url}"));

            //var responseCookies = cookies.GetCookies(uri).Cast<Cookie>().ToList();

            return client;
        }
        public async Task ChangeState(int id, bool state)
        {
            var key = _db.V2Keys.First(a => a.Id == id);
            await UpdateKey(id, key.Traffic, null, state);
        }

        public async Task<string> GetUsedTraffic(int userId)
        {
            var keyUser = _db.V2Keys.FirstOrDefault(a => a.UserId == userId);
            if (keyUser == null)
                throw new ApiException("برای این کاربر کلیدی یافت نشد");


            string guid = "";
            int port = 0;
            int _keyId = 0;
            Obj? obj = null;
            bool keyCreated = false;

            int up = 0;
            int down = 0;
            double total = 0;
            var server = _db.V2Servers.First(c => c.IsMain);
            var ips = server.IPs.Split(',');
            foreach (var ip in ips)
            {
                var httpClient = await GetCookie(server.UserName, server.Password, ip, server.Url, server.Port);
                var keyDetails = await GetServerKey(ip, keyUser.KeyId, server.Port, httpClient);
                total += keyDetails.down.ByteToGigaByte();
            }

            return String.Format("{0:0.00}", total);
        }
        public async Task<UserKeyDetailsOutput> UserKeyDetails(int userId)
        {
            var keyDetailsOutput = new UserKeyDetailsOutput();
            var keyUser = await _db.V2Keys.FirstOrDefaultAsync(a => a.UserId == userId);
            if (keyUser == null)
                return keyDetailsOutput;

            keyDetailsOutput.ExpireTime = keyUser.ExpireDate.ToDateTime().ToPeString();
            keyDetailsOutput.Key = keyUser.Key;
            keyDetailsOutput.Traffic = keyUser.Traffic;
            return keyDetailsOutput;
        }

        public async Task GenerateUserKey(int count, int userId)
        {
            try
            {

                var user = await _db.Users.Include(new[] { "V2Keys" }).FirstOrDefaultAsync(a => a.Id == userId);
                if (user == null)
                    throw new ApiException("کاربر یافت نشد");
                //if (user.UsedFreeAccount && !user.Paid)
                //{
                //    throw new ApiException("شما باید مبلغ ترافیک را پرداخت نمایید");
                //}
                if (user.V2Keys.Any(c => c.ExpireDate >= DateTime.Now.AddHours(2).ToTimeStamp()))
                {
                    throw new ApiException("اعتبار قبلی شما به پایان نرسیده");
                }

                var key = new CreateV2KeyInput()
                {
                    Traffic = 40,
                    MainServer = true,
                    Count = count,
                    ExpireDate = DateTime.UtcNow.AddDays(30),
                    UserId = userId,
                };
                if (!user.UsedFreeAccount)
                {
                    key.Traffic = 5;
                    key.Count = 1;
                    key.ExpireDate = DateTime.UtcNow.AddDays(7);
                }
                if (user.V2Keys.Any())
                {
                    var getKey = user.V2Keys.First();
                    await UpdateKey(getKey.Id, key.Traffic, DateTime.Now.AddDays(7), true);
                }

                await InsertAsync(key);
                user.UsedFreeAccount = false;
                user.Paid = true;
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
    }
}
