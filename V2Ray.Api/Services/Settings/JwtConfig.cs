using System.Collections.Generic;
using System.Linq;

namespace V2Ray.Api.Services.Settings
{

    public class EmailConfig
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class JwtConfig
    {
        public string SecretKey { get; set; }

        public int AccessTokenExpiration { get; set; }

        public int RefreshTokenExpiration { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
    }
}