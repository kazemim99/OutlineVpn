using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using static V2Ray.Api.Services.Server.ServerService;

namespace V2Ray.Api.Services.V2Keys.Dto
{

    public class SwapServerKeysInput : IValidatableObject
    {
        [Required]
        public int FromServerId { get; set; }
        [Required]
        public int ToServerId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // some other random test
            if (this.FromServerId <= 0)
            {
                results.Add(new ValidationResult("شناسه سرور مقصد اشتباه است"));
            }

            if (this.ToServerId <= 0)
            {
                results.Add(new ValidationResult("شناسه سرور مبدا اشتباه است"));
            }
            return results;
        }
    }

    public class GenerateKeyOutput
    {
        public string? ClientKeyId { get; set; }
        public string? Key { get; set; }
        public string? Remark { get; set; }
    }

    public class CreateV2KeyInput
    {
        [DataMember(IsRequired = false)]
        public string? Remark { get; set; }

        [DataMember(IsRequired = false)]
        [JsonIgnore]
        public int UserId { get; set; }

        public bool State { get; set; }
        public int Count { get; set; }

        [DataMember(IsRequired = false)]
        [JsonIgnore]
        public int ClientPort { get; set; }

        public int ServerId { get; set; }

        public int Traffic { get; set; }

        public DateTime ExpireDate { get; set; }

        [DataMember(IsRequired = false)]
        [JsonIgnore]
        public string? Key { get; set; }

        [DataMember(IsRequired = false)]
        [JsonIgnore]
        public int KeyId { get; set; }

        [DataMember(IsRequired = false)]
        [JsonIgnore]
        public bool MainServer { get; set; }
    }
}
