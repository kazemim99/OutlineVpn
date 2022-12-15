using static V2Ray.Api.Services.Server.ServerService;

namespace V2Ray.Api.Controllers
{
    public class CreateKeyInput
    {
        public string Url { get; set; }
        public string Remark { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Protocol Protocol { get; set; }
        public int? Id { get; set; }
    }
}