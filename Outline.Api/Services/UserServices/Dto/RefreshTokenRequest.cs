using System.Text.Json.Serialization;

namespace Outline.Api.Services.UserServices.Dto
{
    public class RefreshTokenRequest
    {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}