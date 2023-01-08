using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using System.Text;
using V2Ray.Api.Database;
using V2Ray.Api.Shared;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Controllers;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using V2Ray.Api.Services.V2Keys.Dto;
using static V2Ray.Api.Services.Server.ServerService;
using V2Ray.Api.Services.Server;
using ShadowsocksUriGenerator.Protocols.Shadowsocks;

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

            var httpClient = await GetCookie(server);
            if (!root.Any())
                root = await GetServerKeys(server, httpClient);

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
            var httpClient = await GetCookie(server);
            if (!root.Any())
                root = GetServerKeys(server, httpClient).Result.ToList();

            var server2 = _db.V2Servers.First(a => a.Id == input.ToServerId);
            httpClient = await GetCookie(server2, true);
            var root2 = await GetServerKeys(server2, httpClient, true);
            var keys = root.Where(c => root2.All(b => b.port != c.port)).ToList();
            if (!keys.Any()) return;
            int exceptionCount = 0;
            foreach (var item in keys)
            {
                try
                {
                    await GenerateKey(server2, item, httpClient, true);
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

        public async Task DeleteKey( int serverId,int keyId)
        {
            var server = _db.V2Servers.FirstOrDefault(a => a.Id == serverId);
            var key = _db.V2Keys.First(a => a.Id == keyId);
            if (key == null)
                throw new ApiException("کلید یافت نشد");

            var ips = server.IPs.Split(',').ToList();
            var httpClient = await GetCookie(server);

            foreach (var ip in ips)
            {
                var result = await httpClient.PostAsync($"https://{ip}:{server.Port}/xui/inbound/del/{key.KeyId}", null);
                result.EnsureSuccessStatusCode();
                var tt = await result.Content.ReadAsStringAsync();
            }
            _db.V2Keys.Remove(key);
            _db.SaveChanges();
        }

        public async Task UpdateKey(int keyId,int serverId,DateTime expireDate,bool enable)
        {
            var server = _db.V2Servers.FirstOrDefault(a => a.Id == serverId);
            var key = _db.V2Keys.First(a => a.Id == keyId);
            key.ExpireDate = expireDate;
            key.State = enable;
            if (key == null)
                throw new ApiException("کلید یافت نشد");

            var ips = server.IPs.Split(',').ToList();

            var httpClient = await GetCookie(server);
            var keys = await FetchKeysFromServer(server, httpClient);
            var keyModified = keys.obj.First(a => a.id == keyId);
            keyModified.enable = enable;
            keyModified.expiryTime = expireDate.ToGeo().ToTimeStamp();
            foreach (var ip in ips)
            {
                await GenerateKey(server, keyModified, httpClient);
            }
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
                Obj? obj = null;
                foreach (var ip in ips)
                {
                    server.IPs = ip;
                    var httpClient = await GetCookie(server);
                    var sampleKey = GetServerSampleKey(server, httpClient, out _keyId);
                    for (int i = 0; i < input.Count; i++)
                    {
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
                            if (port == 0)
                            {
                                port = GeneratePort(_db);
                                sampleKey.port = port;
                            }
                            sampleKey.id = null;
                            sampleKey.remark = "iranv2ray.com";
                            sampleKey.total = GigaByteToBytes(input.Capacity);
                            //sampleKey.expiryTime = input.ExpireDate.ToTimeStamp();
                            sampleKey.down = 0;
                            sampleKey.up = 0;
                            if (sampleKey == null)
                                throw new Exception();
                            await GenerateKey(server, sampleKey, httpClient);
                            if (obj == null)
                            {
                                obj = new Obj();
                                obj = sampleKey;
                                input.ClientPort = sampleKey.port;
                                input.Remark = sampleKey.remark;
                                input.ServerId = server.Id;
                            }

                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }

                var set = JsonConvert.DeserializeObject<Setting>(obj.settings);
                var key = $"{obj.protocol}://{set.clients.First().id}@{server.Url}:{obj.port}";
                key += $"?type=tcp&security=xtls&flow=xtls-rprx-direct#{obj.remark}";
                await File.AppendAllLinesAsync("keys.txt", new List<string>());
                input.Key = key;
                input.KeyId = _keyId++;
                await base.InsertAsync(input);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task GenerateKey(V2Server server, Obj item, HttpClient httpClient, bool swap = false)
        {
            var formContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync($"https://{server.IPs}:{server.Port}/xui/inbound/add", formContent);
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



        private  Obj GetServerSampleKey(V2Server input, HttpClient httpClient, out int keyId)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var re =  httpClient.PostAsJsonAsync($"https://{input.IPs}:{input.Port}/xui/inbound/list", new { }).Result;
            var ttt =  re.Content.ReadAsStringAsync().Result;
            var obj = JsonConvert.DeserializeObject<Root>(ttt).obj;
             keyId = obj.OrderByDescending(a => a.id).First().id.Value;
            return obj.OrderBy(a => a.remark).First();
        }
        public async Task<List<Obj>> GetServerKeys(V2Server input, HttpClient httpClient, bool swap = false)
        {
            var root = await FetchKeysFromServer(input, httpClient);
            var objs = root.obj.OrderBy(a => a.remark).Where(a => a.protocol.Contains("vless") || a.protocol.Contains("trojan")).ToList();

            return objs;
        }

        public async Task<Obj> GetServerKey(V2Server input, HttpClient httpClient, string clientId)
        {
            var root = await FetchKeysFromServer(input, httpClient);
            var obj = root.obj.OrderByDescending(a => a.id).FirstOrDefault(a => a.settings.ToString().Trim().Contains(clientId));

            return obj;
        }

        private static async Task<Root> FetchKeysFromServer(V2Server input, HttpClient httpClient)
        {
            var url = $"https://{input.IPs}";
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var re = await httpClient.PostAsJsonAsync($"{url}:{input.Port}/xui/inbound/list", new { }); ;
            var ttt = await re.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Root>(ttt);
        }

        private long GigaByteToBytes(long gigateBytes)
        {
            return gigateBytes * Convert.ToInt64(Math.Pow(1024, 3));
        }

        private async Task<HttpClient> GetCookie(V2Server input, bool swap = false)
        {
            //input.Url = swap ? input.IP : input.Url;


            var formContent = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("username", input.UserName),
    new KeyValuePair<string, string>("password",input.Password)
});
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            handler.CookieContainer = cookies;
            HttpClient client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(10);

            var url = $"https://{input.IPs}";
            var response = await client.PostAsync($"{url}:{input.Port}/login", formContent);
            cookies.Add(new Uri("https://" + input.Url), new Cookie("domain", $"{input.Url}"));

            //var responseCookies = cookies.GetCookies(uri).Cast<Cookie>().ToList();

            return client;
        }
        public Task ChangeState(int id)
        {
            throw new NotImplementedException();
        }

        public async Task CreateFreeAcount(int userId, int? count)
        {
            var user = await _db.Users.Where(a => a.Id == userId).FirstOrDefaultAsync();
            if (!user.UsedFreeAccount)
            {
                if (!user.Paid)
                    throw new ApiException("هیچ پرداخت تایید شده ای یافت نگردید");
            }
            var server = await _db.V2Servers.Where(a => a.IsActive).Select(a => a.Id).FirstOrDefaultAsync();

            if (server == 0)
                throw new ApiException("سرور یافت نشد");

            await this.InsertAsync(new CreateV2KeyInput
            {
                Capacity = user.UsedFreeAccount ? 10 : 40,
                Count = 1,
                ServerId = server,
                ExpireDate = DateTime.UtcNow.AddDays(30)
            });

        }
        public async Task<UserKeyDetailsOutput> UserKeyDetails(int userId)
        {
            var keyDetailsOutput = new UserKeyDetailsOutput();
            var keyUser = await _db.V2Keys.FirstOrDefaultAsync(a => a.UserId == userId);
            if (keyUser == null)
                return keyDetailsOutput;

            keyDetailsOutput.ExpireTime = keyUser.ExpireDate.ToPeString();
            keyDetailsOutput.Key = keyUser.Key;

            return keyDetailsOutput;
        }

        public async Task GenerateUserKey(int count, int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(a => a.Id == userId);
            if (user == null)
                throw new ApiException("کاربر یافت نشد");
            if (user.UsedFreeAccount && !user.Paid)
            {
                throw new ApiException("شما باید مبلغ ترافیک را پرداخت نمایید");
            }
            var key = new CreateV2KeyInput()
            {
                Capacity = 40,
                MainServer = true,
                Count = count,
                ExpireDate = DateTime.UtcNow.AddDays(30),
                UserId = userId,
            };
            if (!user.UsedFreeAccount)
            {
                key.Capacity = 5;
                key.Count = 1;
                key.ExpireDate = DateTime.UtcNow.AddDays(7);
            }
            await InsertAsync(key);
            user.UsedFreeAccount = true;
            user.Paid = false;
            _db.Update(user);
            await _db.SaveChangesAsync();

        }
    }
}
