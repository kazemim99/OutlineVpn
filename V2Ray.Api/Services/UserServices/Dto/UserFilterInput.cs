using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.UserServices.Dto
{
    public class UserFilterInput : PaginationModelInput
    {

        [DataMember(IsRequired = false)]
        public string? FirstName { get; set; }

        [DataMember(IsRequired = false)]
        public string? LastName { get; set; }

        [DataMember(IsRequired = false)]
        public bool? Enable { get; set; }

        [DataMember(IsRequired = false)]
        public int? UserId { get; set; }

        [DataMember(IsRequired = false)]
        public bool? IsAdmin { get; set; }

        [DataMember(IsRequired = false)]
        public string? Mobile { get;  set; }
    }

    public class ReCaptchaResponse


    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("score")]
        public float Score { get; set; }


        [JsonProperty("action")]
        public string Action { get; set; }


        [JsonProperty("challenge_ts")]
        public DateTime ChallengeTs { get; set; } // timestamp of the challenge load (ISO format yyyy-MM-dd'T'HH:mm:ssZZ)


        [JsonProperty("hostname")]
        public string HostName { get; set; }    // the hostname of the site where the reCAPTCHA was solved


        [JsonProperty("error-codes")]
        public string[] ErrorCodes { get; set; }


    }

}