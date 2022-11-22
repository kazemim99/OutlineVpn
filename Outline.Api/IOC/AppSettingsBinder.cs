using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Outline.Api.Services.Settings;
using SimpleInjector;

namespace Outline.Api.IOC
{
    public static class AppSettingsBinder
    {
        public static IConfigurationSection JwtConfig;

        public static OtpSettings OtpSettings = new OtpSettings();

        public static SmsSettings SmsSettings = new SmsSettings();
        public static EmailConfig EmailConfig = new EmailConfig();

        public static void BindAppSettings(this IConfiguration configuration)
        {
            JwtConfig = configuration.GetSection("Jwt");

            configuration.Bind("Otp", OtpSettings);
            configuration.Bind("EmailConfiguration", EmailConfig);
            configuration.Bind("Sms", SmsSettings);
        }

        public static void RegisterAppSettings(this Container container, IServiceCollection services)
        {
            services.Configure<JwtConfig>(JwtConfig);
            container.RegisterInstance(OtpSettings);
            container.RegisterInstance(EmailConfig);
            container.RegisterInstance(JwtConfig);
            container.RegisterInstance(SmsSettings);
        }
    }
}