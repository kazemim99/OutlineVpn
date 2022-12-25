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

        private static Obj Objs;
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

            var httpClient = GetCookie(server);
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
            var httpClient = GetCookie(server);
            if (!root.Any())
                root =  GetServerKeys(server, httpClient).Result.Where(a=>!a.protocol.Contains("shadowsocks")).ToList();
            var server2 = _db.V2Servers.First(a => a.Id == input.ToServerId);
            httpClient = GetCookie(server2, true);
            var root2 = await GetServerKeys(server2, httpClient, true);
            var keys = root.Where(c => root2.All(b => b.port != c.port)).ToList();
            foreach (var item in keys)
            {
                try
                {
                    var result = await GenerateKey(server2, item, httpClient, true);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            tryAgain = false;
            server2.Swapped = true;
            _db.V2Servers.Update(server2);
            await _db.SaveChangesAsync();

        }
        public override async Task InsertAsync(CreateV2KeyInput input)
        {
            var server = _db.V2Servers.First(a => a.Id == input.ServerId);
            var httpClient = GetCookie(server);
            var item = await GetServerSampleKey(server, httpClient);

            item.total = GigaByteToBytes(input.Capacity);
            item.expiryTime = input.ExpireDate.ToGeo().ToTimeStamp();
            item.enable = input.State;
            for (int i = 0; i < input.Count; i++)
            {
                if (item == null)
                    throw new Exception();
                var result = await GenerateKey(server, item, httpClient);
                input.Key = result.Key;
                input.ClientKeyId = result.ClientKeyId;
                input.Remark = result.Remark;

                await base.InsertAsync(input);
            }

        }

        private async Task<GenerateKeyOutput> GenerateKey(V2Server server, Obj item, HttpClient httpClient, bool swap = false)
        {


            var output = new GenerateKeyOutput();
            string guid = "";
            if (!swap)
            {
                guid = Guid.NewGuid().ToString();
                item.settings = Regex.Replace(item.settings,
                                                  @"[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?",
                                                  @$"{guid}",
                                                  RegexOptions.IgnoreCase);
            }
            var set = JsonConvert.DeserializeObject<Setting>(item.settings);
            try
            {


                if (set != null && set.clients != null && set.clients.Any())
                {
                    guid = swap ? set.clients.First().id : guid;
                    output.ClientKeyId = set.clients.First().id;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            item.id = null;

            item.port = swap ? item.port : GeneratePort(_db);
            item.remark = swap ? item.remark : $"{server.Title}_{item.port}_{item.protocol}";
            output.Remark = item.remark;
            var formContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            var http = "";
            http = swap ? "http" : "https";
            var result = await httpClient.PostAsync($"{http}://{server.Url}:{server.Port}/xui/inbound/add", formContent);
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

            if (!swap)
            {
                string key = "";

                if (item.protocol == Protocol.vless.ToString())
                {
                    key = $"{item.protocol}://{guid}@{server.Url}:{item.port}";

                    key += $"?type=tcp&security=xtls&flow=xtls-rprx-direct#{item.remark}";

                }
                else if (item.protocol == Protocol.shadowsocks.ToString())
                    key = $"?type=tcp&security=xtls&flow=xtls-rprx-direct#{item.remark}";
                else
                {
                    key = $"{item.protocol}://{set.clients.First().password}@{server.Url}:{item.port}";
                    key += $"?type=tcp&security=xtls&flow=xtls-rprx-direct#{item.remark}";
                }
                await File.AppendAllLinesAsync("keys.txt", new List<string>
            {
                key
            });
            }
            return output;
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

        private async Task<List<V2Server>> GetActiveServers()
        {
            return await _db.V2Servers.Include(a => a.City).Where(a => a.IsActive).ToListAsync();
        }


        public async Task<Obj> GetServerSampleKey(V2Server input, HttpClient httpClient)
        {
            if (Objs != null)
                return Objs;

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var re = await httpClient.PostAsJsonAsync($"https://{input.Url}:{input.Port}/xui/inbound/list", new { }); ;
            var ttt = await re.Content.ReadAsStringAsync();
            var root = JsonConvert.DeserializeObject<Root>(ttt).obj.OrderBy(a => a.remark).First(a => a.protocol == "vless");
            Objs = new Obj();
            Objs = root;
            return Objs;
        }
        public async Task<List<Obj>> GetServerKeys(V2Server input, HttpClient httpClient, bool swap = false)
        {

            var url = !swap ? $"https://{input.Url}" : $"http://{input.IP}";
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var re = await httpClient.PostAsJsonAsync($"{url}:{input.Port}/xui/inbound/list", new { }); ;
            var ttt = await re.Content.ReadAsStringAsync();
            var root = JsonConvert.DeserializeObject<Root>(ttt).obj.OrderBy(a => a.remark).ToList();

            return root;
        }
        private long GigaByteToBytes(long gigateBytes)
        {
            return gigateBytes * Convert.ToInt64(Math.Pow(1024, 3));
        }

        private HttpClient GetCookie(V2Server input, bool swap = false)
        {
            input.Url = swap ? input.IP : input.Url;


            var formContent = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("username", input.UserName),
    new KeyValuePair<string, string>("password",input.Password)
});
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            HttpClient client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(10);
            var url = swap ? $"http://{input.Url}" : $"https://{input.Url}";
            var response = client.PostAsync($"{url}:{input.Port}/login", formContent).Result;

            var stringContent = response.Content.ReadAsStringAsync().Result;

            cookies.Add(new Uri("http://www.iranoutline.tk"), new Cookie("domain", $"{input.Url}"));

            //var responseCookies = cookies.GetCookies(uri).Cast<Cookie>().ToList();

            return client;


        }

        public Task ChangeState(int id)
        {
            throw new NotImplementedException();
        }
    }
}
