using V2Ray.Api.Controllers;
using V2Ray.Api.Entity;
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
        Task ChangeState(int id);
    }
}