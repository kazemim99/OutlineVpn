using V2Ray.Api.Controllers;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.SSHKeys.Dto;
using V2Ray.Api.Services.V2Keys.Dto;

namespace V2Ray.Api.Services.SSHKeys
{
    public interface ISSHKeyService : IBaseService<int,
        UpdateSSHKeyInput,
        CreateSSHKeyInput,
        GetSSHKeyOutput,
        GetSSHKeyListOutput,
        SSHKeyFilterInput>
    {
        Task DeleteFromVPS(int id);
        Task GenerateSshFromAdmin(CreateSSHKeyInput input);
        Task<GenerateSSHOutput> GenerateSshFromClient(int userId);
    }
}