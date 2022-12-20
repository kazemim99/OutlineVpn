using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using V2Ray.Api.Services.Settings;

namespace V2Ray.Api.IOC
{
    public static class AppSettingsBinder
    {

        public static void BindAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtConfig>(configuration.GetSection("Jwt"));
            services.Configure<OtpSettings>(configuration.GetSection("Otp"));
            services.Configure<EmailConfig>(configuration.GetSection("EmailConfiguration"));
            services.Configure<SmsSettings>(configuration.GetSection("Sms"));
        }
        
    }
}