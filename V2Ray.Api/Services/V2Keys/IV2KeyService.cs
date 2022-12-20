using V2Ray.Api.Controllers;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.V2Keys.Dto;
using static V2Ray.Api.Services.Server.ServerService;

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
        Task<List<Obj>> GetServerKeys(V2Server input, HttpClient httpClient);
        Task<string> GenerateKey(V2Server server, int userId = 0, string user = "cu");
    }
}