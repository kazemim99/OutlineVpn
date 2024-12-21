using System.Collections.Generic;
using V2Ray.Api.Entity;


namespace V2Ray.Api.Services.V2Keys.Dto
{
    public class GetV2KeyOutput : EntityDto<int>
    {
        public int Id { get; set; }
        public int ServerId { get; set; }
        public int Count { get; set; }
        public int Traffic { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool State { get; set; }
    }
}