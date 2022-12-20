using static V2Ray.Api.Services.Server.ServerService;

namespace V2Ray.Api.Controllers
{
    public class CreateBuldKeyInput
    {
        public string Customer { get; set; } = "cu";
        public int Count { get; set; }
    }
}