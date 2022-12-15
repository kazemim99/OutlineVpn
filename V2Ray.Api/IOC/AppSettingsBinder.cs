using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using V2Ray.Api.Services.Settings;

namespace V2Ray.Api.IOC
{
    public static class AppSettingsBinder
    {
        public static IConfigurationSection JwtConfig;

        public static OtpSettings OtpSettings = new OtpSettings();

        public static SmsSettings SmsSettings = new SmsSettings();
        public static EmailConfig EmailConfig = new EmailConfig();

        public static void BindAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtConfig>(configuration.GetSection("Jwt"));
            services.Configure<JwtConfig>(configuration.GetSection("Otp"));
            services.Configure<JwtConfig>(configuration.GetSection("EmailConfiguration"));
            services.Configure<JwtConfig>(configuration.GetSection("Sms"));
        }
        
    }
}