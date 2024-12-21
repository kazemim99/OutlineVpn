

using Renci.SshNet;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using V2Ray.Api;
using V2Ray.Api.Database;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.SSHKeyServices;
using V2Ray.Api.Services.UserServices.Dto;

public class BotServices : IBotServices
{

    private readonly DB _db;
    private readonly ISSHKeyService _iSSHKeyService;
    public BotServices(DB db, ISSHKeyService iSSHKeyService)
    {
        _db = db;
        _iSSHKeyService = iSSHKeyService;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {

        if (update.Type == UpdateType.Message && update.Message.Text != null)
        {
            string messageText = update.Message.Text;
            string userName = update.Message.Chat.Username;

            switch (messageText)
            {

                case "🛒 خرید اشتراک":
                    await Packages(botClient, update, cancellationToken);
                    break;
                case "/start":
                    await HomePage(botClient, update, cancellationToken);
                    break;
                case "🔑 اکانت تست":
                    await GetTestAccount(botClient, update, cancellationToken);
                    break;
                case "🎁 سرویس های من":
                    await Packages(botClient, update, cancellationToken);
                    break;
                case "🏠 بازگشت به منوی اصلی":
                    await HomePage(botClient, update, cancellationToken);
                    break;

                case "📞 پشتیبانی":
                    await Support(botClient, update, cancellationToken);
                    break;
                default:
                    await HomePage(botClient, update, cancellationToken);
                    break;
            }
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"An error occurred: {exception.Message}");
        return Task.CompletedTask;
    }

    private async Task Support(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await botClient.SendMessage(
                  chatId: update.Message.Chat.Id,
                  text: "@IranSshVpnSupport",
                  cancellationToken: cancellationToken
              );
    }

    private async Task GetAccountInfo(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {

        var replyKeyboard = new ReplyKeyboardMarkup(new[]
        {
                     new KeyboardButton[] { "🏠 بازگشت به منوی اصلی" }
             });


        await botClient.SendMessage(
                  chatId: update.Message.Chat.Id,
                  text: "لطفا یکی از گزینه های زیر را انتخاب نمایید",
                  replyMarkup: replyKeyboard,
                  cancellationToken: cancellationToken
              );
    }

    private async Task GetTestAccount(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var user = update.Message.Chat.Username;
        var isExist = _db.SSHKeyInfos.Any(c => c.UserName == user);
        if (isExist)
        {
            await botClient.SendMessage(
                  chatId: update.Message.Chat.Id,
                  text: "شما قبلا اکانت تست دریافت کرده اید",
                  cancellationToken: cancellationToken
              );

            return;
        }
        else
        {

            var key = new V2Ray.Api.Entity.SSHKey
            {
                AccountType = V2Ray.Api.Services.SSHKeyServices.Dto.AccountType.IRAN,
                ChargeDate = DateTime.UtcNow,
                DurationId = 1,
                Code = null,
                UserName = user,
                ExpireDate = DateTime.Now,
                IsSynced = true,
                UserId = 71
            };
            var account =await _iSSHKeyService.CreateIranAccount(new List<V2Ray.Api.Entity.SSHKey> { key }, AccountActionStatus.Create);

            _db.SSHKeyInfos.Add(key);
            _db.SaveChanges();
            var replyKeyboard = new ReplyKeyboardMarkup(new[]
      {
                     new KeyboardButton[] { "🏠 بازگشت به منوی اصلی" }
             });


            await botClient.SendMessage(
                      chatId: update.Message.Chat.Id,
                      text: account,
                      replyMarkup: replyKeyboard,
                      cancellationToken: cancellationToken
                  );
        }


    }

    private async Task HomePage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var replyKeyboard = new ReplyKeyboardMarkup(new[]
          {
                    new KeyboardButton[] { "🛒 خرید اشتراک", "🔑 اکانت تست" },
                    new KeyboardButton[] { "🎁  اکانت من" },
                    new KeyboardButton[] { "📞 پشتیبانی" },
             })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };

        await botClient.SendMessage(
                 chatId: update.Message.Chat.Id,
                 text: "لطفا یکی از گزینه های زیر را انتخاب نمایید",
                 replyMarkup: replyKeyboard,
                 cancellationToken: cancellationToken
             );
    }

    private async Task Packages(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var replyKeyboard = new ReplyKeyboardMarkup(new[]
{
    new KeyboardButton[] { "⚡️  سه ماهه 250 تومان", "⚡️ یک ماهه 100 تومان" },
    new KeyboardButton[] { "🏠 بازگشت به منوی اصلی" , "⚡️ شش ماهه 450 تومان" } // Back to main menu
});

        await botClient.SendMessage(
                 chatId: update.Message.Chat.Id,
                 text: "لطفا یکی از گزینه های زیر را انتخاب نمایید",
                 replyMarkup: replyKeyboard,
                 cancellationToken: cancellationToken
             );
    }

}

public interface IBotServices
{
    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
}