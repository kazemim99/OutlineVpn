using MailKit.Net.Smtp;
using MimeKit;
using NETCore.MailKit.Core;
using Outline.Api.Database;
using Outline.Api.Entity;
using Outline.Api.Extensions;
using Outline.Api.Services.sms;
using Serilog;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Outline.Api.Services.sms.Rahyab
{
    public class SmsService : IRahyabSmsSender
    {
        private readonly DB _db;

        public SmsService(DB db)
        {
            _db = db;
        }
        public void SendEmail(string code, string to)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Power Box", "powerbox.ir98@gmail.com"));
            message.To.Add(new MailboxAddress("pritom", to));
            message.Subject = "کد ورود";
            message.Body = new TextPart("plain")
            {
                Text = $"کد ورود شما به پاور باکس : {code}",
            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("powerbox.ir98", "xfjsiqpukvgsqzgh");

                client.Send(message);
                client.Disconnect(true);
            }
        }


        public async Task SendAsync(RahyabSendSmsReques request)
        {
            try
            {
                if (!request.destinationAddress.ValidPhone())
                    throw new Exception($" {request.destinationAddress} نا معتبر است"); ;

                var baseAddres = "https://api.rahyab.ir/";

                HttpClient _client = new HttpClient();
                var result = await _client.PostAsJsonAsync($"{baseAddres}api/Auth/getToken", new
                {
                    username = "web_negahno@NEGAHNO",
                    request.password
                });
                var token = await result.Content.ReadAsStringAsync();
                _client.DefaultRequestHeaders.Add("authorization", $"Bearer {token}");
                var redss = await _client.PostAsJsonAsync($"{baseAddres}api/v1/SendSMS_Single", request);


                if (redss.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception($"خطا در ارسال پیام به {request.destinationAddress}");

                Log.Information(await redss.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                if (_db.FailedSms.Any(a => a.DestinationAddress == request.destinationAddress && !a.Sent))
                    throw;

                _db.FailedSms.Add(new FailedSms
                {
                    Sent = false,
                    DestinationAddress = request.destinationAddress,
                    Password = request.password,
                    UserName = request.userName,
                    Message = request.message,
                    Company = request.company,
                    Number = request.number,
                    MessageId = request.messageId,
                    ExceptionMessage = ex.Message,
                    CreatedAt = DateTime.Now
                });
                _db.SaveChanges();
            }
        }
    }
}