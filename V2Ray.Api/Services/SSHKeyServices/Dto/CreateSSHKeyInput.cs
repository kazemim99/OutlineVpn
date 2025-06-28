using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
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

        [JsonIgnore]
        public bool IsAdmin { get; set; }

        public string UserName { get; set; }
        public string Name { get; set; }
        public string? Password { get; set; }
        [JsonIgnore]
        public int Port { get; set; }


        [DataMember(IsRequired = true)]
        public int DurationId { get; set; }

        [DataMember(IsRequired = false)]
        [JsonIgnore]
        public string? ExpireDate { get; set; }
        //[Required]
        //[MaxLength(6, ErrorMessage = "مقدار بیشتر از 6 نمیتواند باشد")]

        //public int Month { get; set; }
        //[MaxLength(6,ErrorMessage ="مقدار بیشتر از 6 نمتواند باشد")]
        //public int ExtraDay { get; set; }
        public int? Count { get; set; }

        [DataMember(IsRequired = false)]
        [JsonIgnore]
        public string? Server { get; set; }
        public int? UserId { get; set; }
        public int ExtraDayId { get; set; }

        [Required(ErrorMessage = "لطفا تعداد کاربر را وارد نمایید")]
        public int MultiUser { get; set; } = 1;
        [DataMember(IsRequired = false)]
        [JsonIgnore]
        public string? Code { get; set; }
        [DataMember(IsRequired = false)]
        [JsonIgnore]
        public int? V2Port { get; set; }

        [DataMember(IsRequired = false)]
        [JsonIgnore]
        public int? V2Id { get; internal set; }

        [DataMember(IsRequired = false)]
        [JsonIgnore]
        public string? V2Guid { get; set; }

        [DataMember(IsRequired = false)]
        [JsonIgnore]
        public string? SSHCode { get; set; }

        [Required]
        public AccountType AccountType { get; set; }


    }

    public enum AccountType
    {
        //[Description("SSH")]
        //SSH = 1,
        [Description("VLess")]
        V2RAy = 2,
        //[Description("IRAN")]
        //IRAN = 6,
        //[Description("L2TP")]
        //L2TP = 7,
    }
}