using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace V2Ray.Api.Services.SSHKeyServices.Dto
{
    public class CreateSSHKeyInput
    {
        [JsonIgnore]
        public DateTime ChargeDate
        {
            get; set;
        }
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
        [Required]
        [Range(50000, 900000, ErrorMessage = "مبلغ وارد شده صحیح نیست")]
        public int Amount { get; set; }
    }
}