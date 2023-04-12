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

        [DataMember(IsRequired = false)]
        public string? ExpireDate { get; set; }
        //[Required]
        //[MaxLength(6, ErrorMessage = "مقدار بیشتر از 6 نمیتواند باشد")]

        //public int Month { get; set; }
        //[MaxLength(6,ErrorMessage ="مقدار بیشتر از 6 نمتواند باشد")]
        //public int ExtraDay { get; set; }
        public int? Count { get; set; }
        [Required]
        public int ServerId { get; set; }
        public int? UserId { get; set; }
        public int Amount { get; set; }
    }
}