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
        //Task GenerateSshFromClient(int userId);
        Task GenerateSshFromAdmin(CreateSSHKeyInput input);
        Task Adjust();
        Task AdjustV2();
        Task ChangeState(int id,int currentUserId, bool fromCharge = false);
        Task ChangePassowrd(int id);
        //Task Recreate(string name);
        Task UpdateUserTraffic();
        Task DisableExpired();
        Task SetUser(int userId, SetPasswordModel model);
        Task Charge(int id, int durationId, int userId);
        void ChangeServer(SSHKey sshKey, V2Server newServer, V2Server oldserver);
        Task<string> CreateIranAccount(List<SSHKey> sSHKeys, AccountActionStatus status, bool isSync = false);
    }
}