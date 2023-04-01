using V2Ray.Api.Entity;
using V2Ray.Api.Services.SSHKeyServices.Dto;
using V2Ray.Api.Services.V2Keys.Dto;

namespace V2Ray.Api.Services.SSHKeyServices
{
    public interface ISSHKeyService : IBaseService<int,
        UpdateSSHKeyInput,
        CreateSSHKeyInput,
        GetSSHKeyOutput,
        GetSSHKeyListOutput,
        SSHKeyFilterInput>
    {
        Task<GenerateSSHOutput> GetKeyDetails(int userId);
        Task GenerateSshFromClient(int userId);
        Task DeleteFromVPS(string userName, V2Server server);
        Task GenerateSshFromAdmin(CreateSSHKeyInput input);
        Task Adjust(int serverId);
        Task ChangeState(int id);
        Task Recreate(string name);
        Task Swapp();
        Task Swapp2(string url);
        Task DisableExpired();
    }
}