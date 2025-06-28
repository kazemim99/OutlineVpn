
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Renci.SshNet;
using V2Ray.Api.Database;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.SSHKeyServices.Dto;
namespace V2Ray.Api.Services.SSHKeyServices
{


    public interface ICloudflareDnsUpdater
    {
        Task UpdateDnsRecordAsync();
    }

    public class CloudflareDnsUpdater : ICloudflareDnsUpdater
    {

        private readonly string apiToken = "ZaRDBKNj9Ku4GuCp_1jvRPt-2ef2bKg-m7S57xP3";
        private readonly string zoneId = "33f8faf72fa3bce38720ba3e84e2769c";
        private readonly string recordId = "f34a7af0db0070ce43f50a3bee492c7f";
        private readonly DB _db;
        public CloudflareDnsUpdater(DB db)
        {
            _db = db;
        }

        private int currentIndex = 0;

        public async Task UpdateDnsRecordAsync()
        {


            var records = _db.DNSRecords.OrderByDescending(c => c.UpdateDate).Where(c => c.Enable);
            if (!records.Any())
                return;
            var currentRecord = records.FirstOrDefault(c => c.IsActive);
            //DateTime currentTime = DateTime.UtcNow;
            //TimeSpan timeDifference = currentTime - currentRecord.UpdateDate;

            var info = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            DateTimeOffset currentTime = TimeZoneInfo.ConvertTime(localServerTime, info);


            if (currentRecord != null && currentTime.Hour >= 4 && currentTime.Hour < 16 && records.Count() > 1)
            {
                var newRecord = records.First(c => !c.IsActive);
                var newIp = newRecord.Ipv4;
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);

                    var requestBody = new
                    {
                        Content = newIp,
                        Id = "f34a7af0db0070ce43f50a3bee492c7f",
                        Name = "v.iransshvpn.com",
                        Proxiable = true,
                        Proxied = false,
                        Ttl = 60,
                        Type = "A",
                        ZoneId = "33f8faf72fa3bce38720ba3e84e2769c",
                        ZoneName = "iransshvpn.com"
                    };

                    var json = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(
                        $"https://api.cloudflare.com/client/v4/zones/{zoneId}/dns_records/{recordId}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {

                            var oldRecord = records.First(c => c.IsActive);
                            oldRecord.IsActive = false;
                            newRecord.UpdateDate = DateTime.Now;
                            newRecord.IsActive = true;
                            _db.Update(newRecord);
                            _db.Update(oldRecord);
                            _db.SaveChanges();
                            RebootServer();
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to update DNS record: {response.ReasonPhrase}");
                    }
                }

            }
        }

        public async void RebootServer()
        {
            Thread.Sleep(60000);
            var hosts = new List<string>()
            {
                "v25.iransshvpn.com",
                "v26.iransshvpn.com",
                "v27.iransshvpn.com",
                "ssh.iransshvpn.com"
            };
            foreach (var host in hosts)
            {
                try
                {


                    string username = "root";
                    string password = "o^qw7^n3LdDhfs5O";
                    using (var client = new SshClient(host, 1027, username, password))
                    {
                        client.Connect();

                        // Use curl to make a GET request on the remote server
                        var command = client.CreateCommand($"sudo reboot");
                        var jsonResult = command.Execute();

                        var apiResponse = JsonConvert.DeserializeObject<AccessKeyListOutput>(jsonResult);

                        client.Disconnect();
                    }
                }
                catch (Exception)
                {


                }
            }

            //await CreateIranAccount(_db.SSHKeyInfos.Where(c => c.AccountType == AccountType.IRAN && c.IsSynced == false).ToList(), AccountActionStatus.Create);


        }

        private async Task<string> CreateIranAccount(List<SSHKey> sSHKeys, AccountActionStatus status)
        {
            var url = "http://irv.iransshvpn.com/W5TFZcn3ia";

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
            { return true; };

            using var httpClient = new HttpClient(clientHandler)
            {

                Timeout = TimeSpan.FromSeconds(7)
            };

            var loginData = new
            {
                username = "Mostafa",
                password = "M0staf@136$"
            };

            StringContent queryString = new StringContent(JsonConvert.SerializeObject(loginData), UnicodeEncoding.UTF8, "application/json");


            EntityEntry<SSHKey>? entity = null;



            foreach (var item in sSHKeys)
            {

                var loginResponse = await httpClient.PostAsJsonAsync($"{url}/login", loginData);
                loginResponse.EnsureSuccessStatusCode();
                var sessionCookie = loginResponse.Headers.GetValues("Set-Cookie").ToString();
                httpClient.DefaultRequestHeaders.Add("Cookie", sessionCookie);

                if (status == AccountActionStatus.Delete)
                {

                    try
                    {
                        var api = $"{url}/panel/inbound/{2}/delClient/{item.V2Guid}";
                        var postResponse = await httpClient.PostAsync($"{url}", null);
                        postResponse.EnsureSuccessStatusCode();
                    }

                    catch (Exception ex)
                    {


                    }
                }
                else
                {





                    var formData = new Dictionary<string, string>
        {
            { "id", "2" },
            { "settings", "{\"clients\":" +
            "[" +
            "{\"flow\":\"\"," +
            "\"id\":\"" + item.V2Guid + "\"," +
            "\"email\":\"" + item.UserName + "\"," +
            "\"limitIp\":0," +
            "\"totalGB\":\"" +(item.DurationId/30) * 52212254720 + "\"," +
            "\"expiryTime\":0," +
            "\"enable\":true," +
            "\"tgId\":0," +

            "\"reset\":0" +
            "}" +
            "]" +
            "}"
                            },

        };


                    // Encode the form data
                    var content = new FormUrlEncodedContent(formData);
                    try
                    {

                        var panelUrl = $"{url}/panel/inbound/addClient";
                        // Perform POST request to /panel/inbound/add
                        var postResponse = await httpClient.PostAsync($"{panelUrl}", content);
                        postResponse.EnsureSuccessStatusCode();
                        var contents = await postResponse.Content.ReadAsStringAsync();

                        var jsonObject = JObject.Parse(contents);

                        var success = (bool)jsonObject["success"];

                        if (!((string)jsonObject["msg"]).Contains("Duplicate") && !success)
                        {
                            throw new ApiException((string)jsonObject["msg"]);
                        }
                        item.IsSynced = true;
                        _db.Update(item);
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
            return sSHKeys.First().Code;
        }
    }


}

