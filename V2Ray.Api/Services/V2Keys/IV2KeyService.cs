using V2Ray.Api.Controllers;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.V2Keys.Dto;


namespace V2Ray.Api.Services.V2Keys
{
    public interface IV2KeyService : IBaseService<int,
        UpdateV2KeyInput,
        CreateV2KeyInput,
        GetV2KeyOutput,
        GetV2KeyListOutput,
        V2KeyFilterInput>
    {
        Task ChangeState(int id,bool state);
        Task<Pagination<GetV2KeyListOutput>> GetAllFromXUIServerAsync(V2KeyFilterInput paging);
        Task SwapServerKeysAsync(SwapServerKeysInput input);
        Task<UserKeyDetailsOutput> UserKeyDetails(int userId);
        Task GenerateUserKey(int count, int userId);
        Task DeleteKey(int keyId);
        Task UpdateKey(int keyId, int traffic, DateTime? expireDate, bool enable);
        Task<string> GetUsedTraffic(int userId);
    }
}