using AutoMapper;
using SimpleInjector;
using System.Reflection;
using Outline.Api.Database;
using Outline.Api.Services.sms;
using Outline.Api.Services.sms.Rahyab;
using Outline.Api.Services.OTP;
using Outline.Api.Services.JWT;
using Outline.Api.Services.UserServices;
using Outline.Api.Mapping;
using Outline.Api.Services.PlanServices;
using Outline.Api.Services.ApiUrlServices;
using OutlineVpn;

namespace Outline.Api.IOC
{
    public static class IOC
    {
        public static readonly Container Container = new Container();

        private static DB _db;

        public static void RegisterServices(IWebHostEnvironment env, IServiceCollection services)
        {
            _db = services.BuildServiceProvider().GetService<DB>();
            Container.Register<IUserService, UserService>();
            Container.Register<IRahyabSmsSender, SmsService>();
            Container.Register<IOtpService, OtpSharpService>();
            Container.Register<IPlanService, PlanService>();
            Container.Register<IApiUrlService, ApiUrlService>();
            Container.Register<IOutlineApi, OutlineApi>();

            Container.Register<IJwtAuthManager, JwtAuthManager>(Lifestyle.Singleton);

            RegisterOutMapper(env);
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

            Container.RegisterInstance(config);
            Container.Register(() => config.CreateMapper(Container.GetInstance));
        }
    }
}