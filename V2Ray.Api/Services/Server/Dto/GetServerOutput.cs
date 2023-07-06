using System.Collections.Generic;
using V2Ray.Api.Entity;

namespace V2Ray.Api.Services.Server.Dto
{
    public class GetServerOutput : EntityDto<int>
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public int Capacity { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? UserId { get; set; }
        public string IP { get; set; }
        public bool HasLicense { get; set; }
        public string? Url { get; set; }
    }
}