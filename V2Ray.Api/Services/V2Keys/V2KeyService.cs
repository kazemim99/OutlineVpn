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

            var httpClient = _server.GetCookie(input);
            if (!root.Any())
                root = await _server.GetServerKeys(input, httpClient);

            var result = root.Select(a => new GetV2KeyListOutput
            {
                ExpireDate = DateTime.Now.ToPeString(),
                PrimaryCapacity =50,
                State = true,
                Id = 1,
                Title  =  a.remark,
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
