using System.Collections.Generic;
using V2Ray.Api.Entity;
using static V2Ray.Api.Services.Server.ServerService;

namespace V2Ray.Api.Services.V2Keys.Dto
{
    public class GetV2KeyOutput : EntityDto<int>
    {
        public int Id { get; set; }
        public int ServerId { get; set; }
        public int Capacity { get; set; }
        public int ExpireDate { get; set; }
        public bool State { get; set; }
        public Protocol Protocol { get; set; }
    }
}