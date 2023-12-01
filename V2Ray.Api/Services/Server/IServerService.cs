
using V2Ray.Api.Controllers;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.Server.Dto;
using static V2Ray.Api.Services.Server.ServerService;

namespace V2Ray.Api.Services.Server
{
    public interface IServerService : IBaseService<int,
        UpdateServerInput,
        CreateServerInput,
        GetServerOutput,
        GetServerListOutput,
        ServerFilterInput>
    {
        Task IsDelete(int id, string fullName);
        Task ChangeState(int id);
        Task<CustomerInfoOutput> CustomerInfo(string userName);
        List<CustomerServerOutput> CustomerServers(string userName);
        Task ChangeServer(int serverId, string customerUserName);
        Task ChangeActive(int id);
    }
}