using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using V2Ray.Api;
using V2Ray.Api.Database;
using V2Ray.Api.Services.JWT;
using V2Ray.Api.Services.OTP;
using V2Ray.Api.Services.PlanServices;
using V2Ray.Api.Services.Server;
using V2Ray.Api.Services.Settings;
using V2Ray.Api.Services.sms;
using V2Ray.Api.Services.sms.Rahyab;
using V2Ray.Api.Services.UserServices;
using V2Ray.Api.Services.UserServices.Mapping;

namespace V2Ray.Api.Test.Fixtures
{
    public class ApiWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public IConfiguration Configuration { get; private set; }
        public IConfigurationSection ConfigSection;
        public OtpSettings OtpSettings = new OtpSettings();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                Configuration = new ConfigurationBuilder()
                  .AddJsonFile("appsettings.json")
                  .Build();
                config.AddConfiguration(Configuration);
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddDbContext<DB>(options =>
                {
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                    options.UseInMemoryDatabase(databaseName: "testdb");
                });

                services.AddAuthentication("IntegrationTest")
       .AddScheme<AuthenticationSchemeOptions, IntegrationTestAuthenticationHandler>(
         "IntegrationTest",
         options => { }
       );





                //services.Configure<OtpSettings>(Configuration.GetSection("Otp"));
                //services.AddScoped<IUserService, UserService>();
                //services.AddScoped<IRahyabSmsSender, SmsService>();
                //services.AddScoped<IOtpService, OtpSharpService>();
                //services.AddScoped<IPlanService, PlanService>();

                //services.AddScoped<IJwtAuthManager, JwtAuthManager>();
                //services.AddAutoMapper(typeof(Program));

            });
        }
    }
}