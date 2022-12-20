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

        private readonly IMapper _mapper;
        private readonly IServerService _server;
        private static Obj Objs;

        public V2KeyService(DB db, IMapper mapper, IServerService server) : base(mapper, db)
        {
            _mapper = mapper;
            _db = db;
            _server = server;
        }
        public override async Task<Pagination<GetV2KeyListOutput>> GetAllAsync(V2KeyFilterInput paging, params string[] include)
        {
            var input = _db.V2Servers.First(a => a.Id == 1);
            List<Obj> root = new List<Obj>();

            var httpClient = GetCookie(input);
            if (!root.Any())
                root = await GetServerKeys(input, httpClient);

            var result = root.Select(a => new GetV2KeyListOutput
            {
                ExpireDate = DateTime.Now.ToPeString(),
                PrimaryCapacity = 50,
                State = true,
                Id = 1,
                Title = a.remark,
                UsedCapacity = 25,
                User = "user"
            }).ToList();

            return new Pagination<GetV2KeyListOutput>
            {
                Result = result,
                CurrentPage = paging.Page,
                PageCount = paging.ItemsPerPage,
                TotalItems = root.Count()
            };
        }

        public override async Task InsertAsync(CreateV2KeyInput input)
        {
            var server = _db.V2Servers.First(a => a.Id == input.ServerId);
           var result = await GenerateKey(server);
            input.Key = result;
            await InsertAsync(input);

        }

        public async Task<string> GenerateKey(V2Server server, int userId = 0, string user = "cu")
        {
            var httpClient = GetCookie(server);
            var item = await GetServerKeys(server, httpClient);

            if (item == null)
                throw new Exception();
            var guid = Guid.NewGuid().ToString();
            item.settings = Regex.Replace(item.settings,
                                              @"[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?",
                                              @$"{guid}",
                                              RegexOptions.IgnoreCase);
            var set = JsonConvert.DeserializeObject<Setting>(item.settings);
            item.id = null;
            item.port = GeneratePort(_db);
            item.remark = $"{server.City.Title}_{item.port}_{item.protocol}_{user}";
            var formContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync($"https://{server.Url}:{server.Port}/xui/inbound/add", formContent);
            var tt = await result.Content.ReadAsStringAsync();
            var serverResponse = JsonConvert.DeserializeObject<ServerResponse>(tt);
            if (!serverResponse.success)
                throw new ApiException(serverResponse.msg);

            result.EnsureSuccessStatusCode();
            var key = "";
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
            return key;
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

        public async Task<Obj> GetServerKeys(V2Server input, HttpClient httpClient)
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
        private long GigaByteToBytes(long gigateBytes)
        {
            return gigateBytes * Convert.ToInt64(Math.Pow(1024, 3));
        }
        private long ConvertToTimestamp(DateTime value)
        {
            long epoch = (value.Ticks - 621355968000000000) / 10000000;
            return epoch;
        }
        private HttpClient GetCookie(V2Server input)
        {
            var uri = new Uri($"{input.Url}:{input.Port}");


            var formContent = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("username", input.UserName),
    new KeyValuePair<string, string>("password",input.Password)
});
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            HttpClient client = new HttpClient(handler);

            var response = client.PostAsync($"https://{input.Url}:{input.Port}/login", formContent).Result;

            var stringContent = response.Content.ReadAsStringAsync().Result;

            cookies.Add(uri, new Cookie("domain", $"{input.Url}"));

            var responseCookies = cookies.GetCookies(uri).Cast<Cookie>().ToList();

            return client;


        }
        private long GigaByteToBytes(long gigateBytes)
        {
            return gigateBytes * Convert.ToInt64(Math.Pow(1024, 3));
        }
        private long ConvertToTimestamp(DateTime value)
        {
            long epoch = (value.Ticks - 621355968000000000) / 10000000;
            return epoch;
        }

        public Task ChangeState(int id)
        {
            throw new NotImplementedException();
        }
    }
}
