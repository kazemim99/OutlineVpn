using AutoMapper;
using System.Reflection;
using V2Ray.Api.Services.Server;
using V2Ray.Api.Mapping;
using V2Ray.Api.Services.JWT;
using V2Ray.Api.Services.OTP;
using V2Ray.Api.Services.sms;
using V2Ray.Api.Database;
using V2Ray.Api.Services.UserServices;
using V2Ray.Api.Services.PlanServices;
using V2Ray.Api.Services.sms.Rahyab;
using V2Ray.Api.Services.Cities;
using V2Ray.Api.Services.V2Keys;

namespace V2Ray.Api.IOC
{
    public static class IOC
    {


        public static void RegisterServices(IWebHostEnvironment env, IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRahyabSmsSender, SmsService>();
            services.AddScoped<IOtpService, OtpSharpService>();
            services.AddScoped<IPlanService, PlanService>();
            services.AddScoped<IServerService, ServerService>();
            services.AddScoped<ICitieservice, CitieService>();
            services.AddScoped<IV2KeyService, V2KeyService>();

            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();

            RegisterOutMapper(env);
            services.AddAutoMapper(typeof(Program));
        }

        private static void RegisterOutMapper(IWebHostEnvironment env)
        {
            var profiles = Assembly.GetAssembly(typeof(UserEntityMapping))
                ?.GetTypes()
                                .Where(x => typeof(Profile).IsAssignableFrom(x));

            var config = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(Activator.CreateInstance(profile) as Profile);
                }

                cfg.AddProfile(new UserEntityMapping(env));
            });

        }
    }
}