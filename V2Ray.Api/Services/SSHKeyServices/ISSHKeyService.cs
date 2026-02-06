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
        Task AdjustV2();
        Task ChangeState(int id,int currentUserId, bool fromCharge = false);
        //Task Recreate(string name);
        Task UpdateUserTraffic();
        Task DisableExpired();
        Task SetUser(int userId, SetPasswordModel model);
        Task Charge(int id, int durationId, int userId);
        Task<int> CreateV2Ray(int currenUserId, List<SSHKey> sSHKeys, AccountType accountType, AccountActionStatus status = AccountActionStatus.Create, bool isSync = false);
        Task GenerateSshFromAdmin(CreateSSHKeyInput input);
    }
}