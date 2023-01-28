using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.IO;
using System.Reflection;

namespace V2Ray.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
            //    .Enrich.FromLogContext()
            //    .WriteTo.File(new RenderedCompactJsonFormatter(), "logs/log.ndjson")
            //    .WriteTo.Seq("http://localhost:5341")
            //    .CreateLogger();
            //throw new Exception(Directory.GetCurrentDirectory());
            //try
            //{
            //    Log.Information("Application starting...");
            //    throw new Exception(Directory.GetCurrentDirectory());
            var build = CreateHostBuilder(args).Build();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

             build.Run();
            //}
            //catch (Exception ex)
            //{
            //    Log.Fatal(ex, "Application start-up failed");
            //}user-key-details
            //finally
            //{
            //    Log.CloseAndFlush();
            //}
        }



        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseKestrel().UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
                });
    }
}