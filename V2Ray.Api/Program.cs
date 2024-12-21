using Serilog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace V2Ray.Api
{
    public class Program
    {

        private static readonly TelegramBotClient _telegramBotClient = new TelegramBotClient("6178109792:AAH9P_fd-nMu5lzrE6NaSFlJyTmCEp6-E5M");

        public static async Task Main(string[] args)
        {
            using var cts = new CancellationTokenSource();

            //string botToken = "6178109792:AAH9P_fd-nMu5lzrE6NaSFlJyTmCEp6-E5M";
            //string webhookUrl = "https://755d-2a02-4540-e006-ec1a-e5f7-2a25-caea-5e8d.ngrok-free.app";

            //using var httpClient = new HttpClient();


            //var url = $"https://api.telegram.org/bot{botToken}/setWebhook?url={webhookUrl}/webhook";
            //var response = await httpClient.GetAsync(url);
            //string result = await response.Content.ReadAsStringAsync();
            //await _telegramBotClient.DeleteWebhook();


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


//            var soce = build.Services.CreateScope();
//            var service = (IBotServices)soce.ServiceProvider.GetService(typeof(IBotServices));
//            _telegramBotClient.StartReceiving(
//           service.HandleUpdateAsync,
//           service.HandleErrorAsync,
//              receiverOptions: new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
// cancellationToken: CancellationToken.None
//);


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
                    webBuilder.UseIISIntegration()
                    .UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
                });
    }
}