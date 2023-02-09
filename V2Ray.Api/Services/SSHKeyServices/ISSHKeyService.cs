using V2Ray.Api.Controllers;
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
        Task DeleteFromVPS(string userName);
        Task GenerateSshFromAdmin(CreateSSHKeyInput input);
        Task GenerateSshFromClient(int userId);
        Task<GenerateSSHOutput> GetUserSSHKey(int userId);
        Task ChargeOneMonth(string email);
        Task ChangeState(int id);
    }
}