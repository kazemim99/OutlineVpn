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
using V2Ray.Api.Services.V2Keys;

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
        private readonly IV2KeyService _v2KeyService;
        public ServerService(DB db, IMapper mapper, IV2KeyService v2KeyService) : base(mapper, db)
        {
            _mapper = mapper;
            _db = db;
            _v2KeyService = v2KeyService;
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

        public async Task ChangeState(int id)
        {
            var Server = _db.V2Servers.FirstOrDefault(a => a.Id == id);
            Server.IsActive = !Server.IsActive;
            _db.Update(Server);
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

        public void SaveKey(string key, int serverId, int port)
        {
            _db.V2Keys.Add(new V2Key
            {
                Port = port,
                Key = key,
                ServerId = serverId,
            });
            _db.SaveChanges();
        }

        public async Task CreateKey(int count = 1, string customer = "cu")
        {
            try
            {
                var servers = await _db.V2Servers.Where(a=>a.IsActive).ToListAsync();
                for (int i = 0; i < count; i++)
                {
                    foreach (var input in servers)
                    {
                        await _v2KeyService.InsertAsync(new V2Keys.Dto.CreateV2KeyInput
                        {
                            Capacity =""
                        });
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }


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

        public class Client
        {
            public string password { get; set; }
            public string flow { get; set; }
        }

        public class Setting
        {
            public List<Client> clients { get; set; }
            public List<object> fallbacks { get; set; }
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
