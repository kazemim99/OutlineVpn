using System;

namespace V2Ray.Api.Services.UserServices.Dto
{
    public class JwtToken
    {
        public string Token { get; set; }

        public DateTime Expire { get; set; }
    }
}