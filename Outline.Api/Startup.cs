using AutoWrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
using Outline.Api.Common;
using Outline.Api.Database;
using Outline.Api.Services.JWT;
using Outline.Api.BackgroundJob;
using Outline.Api.IOC;
using Outline.Api.Filter;
using Outline.Api.Entity;

namespace Outline.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;

            Configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
            IOC.IOC.Container.Options.ResolveUnregisteredConcreteTypes = false;
            IOC.IOC.Container.Options.DefaultScopedLifestyle = Lifestyle.CreateHybrid(
                defaultLifestyle: new AsyncScopedLifestyle(),
                fallbackLifestyle: new ThreadScopedLifestyle()
            );
            IOC.IOC.Container.Options.EnableAutoVerification = false;

            isTest = Configuration.GetSection("IsTest").Value == "True";
        }

        private bool isTest;

        public IConfiguration Configuration { get; }

        private readonly IWebHostEnvironment _env;

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().AddNewtonsoftJson().AddDataAnnotations()
            .AddApiExplorer();

            Configuration.BindAppSettings();
            services.AddHostedService<UpdateUserUsageLockerState>();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyHeader()
                    .AllowAnyMethod()
                .AllowAnyOrigin());
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            services.Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = 209715200;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                            ClockSkew = TimeSpan.Zero
                        };
                    });

            if (!isTest)
            {
                services
               .AddDbContext<DB>(options =>
               {
                   options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                   options.UseSqlServer
                   (Configuration.GetConnectionString("Default"));
               });
                services.AddSimpleInjector(IOC.IOC.Container, options =>
                {
                    options.AddAspNetCore().AddControllerActivation();
                    options.AddLogging();
                    options.AddHostedService<JwtRefreshTokenCache>();
                });
                InitializeContainer(services);
            }

            SwaggerConfig(services);

            services.AddAuthorization(config =>
            {
                config.AddPolicy(Policies.Admin, Policies.AdminPolicy());
                config.AddPolicy(Policies.User, Policies.UserPolicy());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (!isTest)
            {
                app.UseSimpleInjector(IOC.IOC.Container);
                //if (!_env.IsDevelopment())
                //{
                //    app.UseHttpsRedirection();
                //}
                app.UseApiResponseAndExceptionWrapper(new AutoWrapperOptions
                {
                    UseApiProblemDetailsException = true,
                    ShowIsErrorFlagForSuccessfulResponse = true,
                    BypassHTMLValidation = true,
                    IsApiOnly = false,
                }); ;
            }
            app.UseStaticFiles();
            if (!isTest)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), @"Resources");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                    RequestPath = new PathString("/Resources")
                });
            }
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseDeveloperExceptionPage();

            app.EnsureMigrationOfContext<DB>(_env.EnvironmentName);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.DefaultModelsExpandDepth(0);
            });

            var defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            app.UseStaticFiles();

            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            if (!isTest)
                IOC.IOC.Container.Verify();
        }

        private static void SwaggerConfig(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<ReApplyOptionalRouteParameterOperationFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "PowerBox",
                    Version = "v1-0-0"
                });
                c.UseAllOfToExtendReferenceSchemas();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
   {
     new OpenApiSecurityScheme
     {
       Reference = new OpenApiReference
       {
         Type = ReferenceType.SecurityScheme,
         Id = "Bearer"
       }
      },
      new string[] { }
    }
  });
            });
        }

        private void InitializeContainer(IServiceCollection services)
        {
            IOC.IOC.Container.RegisterAppSettings(services);
            IOC.IOC.RegisterServices(_env, services);
        }
    }
}