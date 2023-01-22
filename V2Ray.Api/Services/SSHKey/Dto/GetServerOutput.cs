using System.Collections.Generic;
using V2Ray.Api.Entity;

namespace V2Ray.Api.Services.SSHKeys.Dto
{
    public class GetSSHKeyOutput : EntityDto<int>
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Email { get; set; }
    }
}