using System.Text.Json.Serialization;

namespace V2Ray.Api.Services.UserServices.Dto
{
    public class RefreshTokenRequest
    {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}