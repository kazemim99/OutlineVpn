using System;
using System.Text.Json.Serialization;

namespace V2Ray.Api.Services.UserServices.Dto
{
    public class RefreshToken
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [JsonPropertyName("tokenString")]
        public string TokenString { get; set; }

        [JsonPropertyName("expireAt")]
        public DateTime ExpireAt { get; set; }

        public int UserId { get; set; }
    }
}