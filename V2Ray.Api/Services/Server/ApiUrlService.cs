using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using System.Text;
using V2Ray.Api.Database;
using V2Ray.Api.Shared;
using V2Ray.Api.Services.Server.Dto;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Controllers;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;

namespace V2Ray.Api.Services.Server
{
    public class ServerService : BaseService<V2Server,
        int,
        UpdateServerInput,
        CreateServerInput,
        GetServerOutput,
        GetServerListOutput,
        ServerFilterInput>,
        IServerService
    {
        private readonly DB _db;

        private readonly IMapper _mapper;

        public ServerService(DB db, IMapper mapper) : base(mapper, db)
        {
            _mapper = mapper;
            _db = db;
        }

        public override async Task UpdateAsync(int id, UpdateServerInput input, params string[] include)
        {
            try
            {
                var Server = await _db.V2Servers.FirstOrDefaultAsync(a => a.Id == id);
                if (Server == null)
                    throw new ApiException(AppErrors.ServerNotFound);

                var map = _mapper.Map<V2Server>(input);
                map.Id = id;

                _db.V2Servers.Update(map);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task InsertAsync(CreateServerInput input)
        {
            var map = _mapper.Map<CreateServerInput, V2Server>(input);

            await _db.AddAsync(map);
            await _db.SaveChangesAsync();
        }

        public async Task ChangeState(int id, string fullName)
        {
            var Server = _db.V2Servers.FirstOrDefault(a => a.Id == id);
            Server.State = !Server.State;
            _db.Update(Server);

            var stateString = Server.State ? "فعال" : "غیر فعال";

            await _db.SaveChangesAsync();
        }

        public override IQueryable<V2Server> Filter(ServerFilterInput filter)
        {
            var query = _db.V2Servers.AsQueryable();

            if (!filter.Title.IsNullOrEmpty())
                query = query.Where(a => a.Title.Contains(filter.Title));




            return query;
        }


        public async Task IsDelete(int id, string fullName)
        {
            var Server = await _db.V2Servers.FirstAsync(a => a.Id == id);
            Server.IsDeleted = true;
            _db.Update(Server);

            await _db.SaveChangesAsync();
        }

        public void SaveKey(string key, int serverId, int userId)
        {
            _db.V2Keys.Add(new V2Key
            {
                Key = key,
                ServerId = serverId,
                UserId = userId
            });
            _db.SaveChanges();
        }

        public async Task CreateKey(int userId)
        {
            var servers =await GetActiveServers();
            foreach (var input in servers)
            {
                var httpClient = GetCookie(input);

                var root = await GetServerKeys(input, httpClient);

                foreach (var item in root)
                {

                    var guid = Guid.NewGuid().ToString();
                    item.settings = Regex.Replace(
          item.settings,
          @"[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?",
          @$"{guid}",
          RegexOptions.IgnoreCase
    );

                    item.id = null;
                    item.port = new Random().Next(10000, 60000);
                    item.remark = $"{input.City.Title}_{item.port}";
                    var formContent = new StringContent(JsonConvert.SerializeObject(root), Encoding.UTF8, "application/json");

                    var result = await httpClient.PostAsync($"https://{input.Url}:{input.Port}/xui/inbound/add", formContent);
                    var tt = await result.Content.ReadAsStringAsync();
                    var serverResponse = JsonConvert.DeserializeObject<ServerResponse>(tt);
                    if (!serverResponse.success)
                        throw new ApiException(serverResponse.msg);

                    result.EnsureSuccessStatusCode();
                    var key = $"{item.protocol.ToString()}://{guid}@{input.Url}:{item.port}";
                    if (item.protocol == Protocol.vless.ToString())
                        key += $"?type=tcp&security=xtls&flow=xtls-rprx-direct#{item.remark}";
                   else if (item.protocol == Protocol.shadowsocks.ToString())
                        key += $"?type=tcp&security=xtls&flow=xtls-rprx-direct#{item.remark}";
                    else
                        key += $"#{item.remark}";

                    SaveKey(key,input.Id, userId);
                }
            }

        }

        private async Task<List<V2Server>> GetActiveServers()
        {
            return await _db.V2Servers.Where(a => a.IsActive).ToListAsync();
        }

        private static async Task<List<Obj>> GetServerKeys(V2Server input, HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var re = await httpClient.PostAsJsonAsync($"https://{input.Url}:{input.Port}/xui/inbound/list", new { }); ;
            var ttt = await re.Content.ReadAsStringAsync();
            var root = JsonConvert.DeserializeObject<Root>(ttt).obj.OrderByDescending(a => a.id).Where(a=>a.protocol != "vmess").Take(4).ToList();
            return root;
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


        public enum Protocol
        {
            vless,
            vmess,
            trojan,
            shadowsocks
        }
        public class ServerResponse
        {
            public bool success { get; set; }
            public string msg { get; set; }
            public object obj { get; set; }
        }


        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Obj
        {
            public int? id { get; set; }
            public long up { get; set; }
            public long down { get; set; }
            public long total { get; set; }
            public string remark { get; set; }
            public bool enable { get; set; }
            public long expiryTime { get; set; }
            public string listen { get; set; }
            public int port { get; set; }
            public string protocol { get; set; }
            public string settings { get; set; }
            public string streamSettings { get; set; }
            public string tag { get; set; }
            public string sniffing { get; set; }
            public string client { get; set; }
        }

        public class Root
        {
            public bool success { get; set; }
            public string msg { get; set; }
            public List<Obj> obj { get; set; }
        }
    }
}
