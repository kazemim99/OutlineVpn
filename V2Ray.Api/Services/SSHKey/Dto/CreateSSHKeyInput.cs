using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace V2Ray.Api.Services.SSHKeys.Dto
{
    public class CreateSSHKeyInput
    {
        public string UserName { get; set; }
        [JsonIgnore]
        public string? Password { get; set; }
        public DateTime ExpireDate { get; set; }
        public int UserId { get; set; }
    }
}