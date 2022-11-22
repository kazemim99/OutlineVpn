using System;

namespace Outline.Api.Services.UserServices.Dto
{
    public class JwtToken
    {
        public string Token { get; set; }

        public DateTime Expire { get; set; }
    }
}