using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace V2Ray.Api.Services.SSHKeyServices.Dto
{
    public class CreateSSHKeyInput
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string? Password { get; set; }
        [JsonIgnore]
        public int Port { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int? Count { get; set; }
        [Required]
        public int ServerId { get; set; }
        public int? UserId { get; set; }
    }
}