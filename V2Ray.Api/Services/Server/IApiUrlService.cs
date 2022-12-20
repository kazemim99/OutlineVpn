
using V2Ray.Api.Controllers;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.Server.Dto;

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
        Task ChangeState(int id, string fullName);
        void SaveKey(string key, int id,int port);
        Task CreateKey(int count,string customer);
    }
}