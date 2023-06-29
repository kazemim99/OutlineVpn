using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using System.Text;
using V2Ray.Api.Database;
using V2Ray.Api.Shared;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Controllers;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using V2Ray.Api.Services.V2Keys.Dto;
using Renci.SshNet;
using V2Ray.Api.Services.SSHKeyServices.Dto;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace V2Ray.Api.Services.SSHKeyServices
{
    public class SSHKeyService : BaseService<SSHKey,
        int,
        UpdateSSHKeyInput,
        CreateSSHKeyInput,
        GetSSHKeyOutput,
        GetSSHKeyListOutput,
        SSHKeyFilterInput>,
        ISSHKeyService
    {
        private readonly DB _db;



        private readonly IMapper _mapper;

        public SSHKeyService(IMapper mapper, DB db) : base(mapper, db)
        {
            _db = db;
            _mapper = mapper;

        }

        public async Task GenerateSshFromAdmin(CreateSSHKeyInput input)
        {
            try
            {
                var server = _db.V2Servers.Include(a => a.SSHKeys).FirstOrDefault(a => a.Id == input.ServerId);

                if (server.SSHKeys.Count(a => a.Enable) > server.Capacity)
                    throw new ApiException("ظرفیت سرور تکمیل است");

                for (int i = 0; i < input.Count; i++)
                {
                    input.Password = input.Password.IsNullOrEmpty() ? CreatePassword() : input.Password;
                    input.Port = 1027;
                    input.UserName = input.UserName.IsNullOrEmpty() ? GenerateUser() : input.UserName;
                    input.ExpireDate = input.ExpireDate.IsNullOrEmpty() ? DateTime.UtcNow.AddMonths(1).AddDays(1).ToPeString("yyyy/MM/dd") : input.ExpireDate;



                    await AddUserToServer(new List<SSHKey>
                    {
                        new SSHKey
                        {
                            UserName = input.UserName,
                            Password = input.Password
                        }
                    }, server);


                    if (server.HasLicense)
                    {
                        await AddOrUpdateToPanel(input.UserName, input.Password, input.ExpireDate, server);
                    }




                    input.ChargeDate = DateTime.UtcNow;
                    var id = await base.InsertGetIdAsync(input);

                    _db.Orders.Add(new Order
                    {
                        SSHKeyId = id,
                        Status = sms.Kavenegar.Models.Enums.OrderStateEnum.Confirmed,
                        CardNumber = "",
                        CreatedAt = DateTime.UtcNow.Date,
                        CreatorUserId = input.UserId,
                        UserId = input.UserId.Value,

                    });

                    _db.SaveChanges();
                    input.UserName = "";
                    input.Password = "";
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task ChangePassowrd(int id)
        {
            var key = await _db.SSHKeyInfos.Include(new[] { "V2Server" }).FirstAsync(a => a.Id == id);

            key.Password = CreatePassword();
            _db.Update(key);
            _db.SaveChanges();
            await DeleteUserFromServer(new List<SSHKey> { key }, key.V2Server);
            await AddUserToServer(new List<SSHKey> { key }, key.V2Server);


        }

        public override async Task UpdateAsync(int id, UpdateSSHKeyInput input, params string[] include)
        {




            var key = _db.SSHKeyInfos.Include(new[] { "V2Server", "Orders" }).Where(a => a.Id == id).ToList();
            var user = key.First();
            if (user.ServerId != input.ServerId || user.Password != input.Password)
            {
                await DeleteUserFromServer(key, user.V2Server);
                await DeleteFromPanel(user.UserName, user.V2Server);
            }
            input.Enable = user.Enable;
            input.ChargeDate = user.ChargeDate;
            if (input.ExpireDate.ToGeo().Date > user.ExpireDate.AddDays(7).Date)
            {

                _db.Orders.Add(new Order
                {
                    SSHKeyId = id,
                    Status = sms.Kavenegar.Models.Enums.OrderStateEnum.Confirmed,
                    CardNumber = "",
                    CreatedAt = DateTime.UtcNow,
                    CreatorUserId = input.UserId,
                    UserId = input.UserId.Value,

                });
                input.ChargeDate = DateTime.UtcNow;

            }


            if (user.V2Server.HasLicense)
            {
                await AddOrUpdateToPanel(user.UserName, user.Password, input.ExpireDate, user.V2Server, "edituser");
            }
            else
            {
                await AddUserToServer(key.ToList(), user.V2Server);
            }
            await base.UpdateAsync(id, input, include);
        }

        public override async Task<GetSSHKeyOutput> GetById(int id, params string[] include)
        {
            var result = await base.GetById(id, include);
            var orders = _db.Orders.Where(a => a.SSHKey.Id == id);
            if (orders.Any())
            {
                var key = orders.OrderByDescending(c => c.CreatedAt).FirstOrDefault();
            }

            return result;
        }

        private async Task DeleteUserFromServer(List<SSHKey> users, V2Server server)
        {
            try
            {


                string str = "";
                var i = 0;

                foreach (var item in users)
                {

                    if (server.HasLicense)
                    {
                        // await DeleteFromPanel(item.UserName, server);
                    }
                    else
                    {
                        str += $"sudo killall -u {item.UserName} \n sudo deluser --remove-home {item.UserName} \n ";
                    }
                }
                if (!server.HasLicense)
                {
                    var connectionInfo = GetConnectionInfo(server.Url, 1027, server.Password, server.UserName);
                    using var ssh = new SshClient(connectionInfo);
                    Connect(ssh);
                    str += "\n";
                    var command2 = ssh.CreateCommand(str);
                    command2.Execute();
                    ssh.Disconnect();
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"{ex.Message} {server.Url}");
            }
        }

        private async Task AddUserToServer(List<SSHKey> users, V2Server server)
        {

            string str = "";

            foreach (var item in users)
            {
                str += $"sudo useradd -m -p  $(openssl passwd -1 {item.Password}) -s /bin/bash {item.UserName} \n ";
            }

            var connectionInfo = new PasswordConnectionInfo(server.Url, 1027, server.UserName, server.Password);
            var ssh = new SshClient(connectionInfo);
            Connect(ssh);

            var comm = str;
            var command = ssh.CreateCommand(comm);
            command.Execute();
            ssh.Disconnect();


        }

        public async Task Charge(int userId)
        {
            var key = await _db.SSHKeyInfos.Include(new[] { "V2Server", "User" }).FirstAsync(a => a.Id == userId);
            if (key.ExpireDate.Date > DateTime.UtcNow.Date)
                throw new ApiException("تاریخ اعتبار به پایان نرسیده");
            //if (key == null)
            //    await GenerateSshFromClient(userId);
            DateTime expireDate = DateTime.UtcNow.AddDays(31);



            var input = new UpdateSSHKeyInput
            {
                Password = key.Password.Trim(),
                UserId = null,
                Port = 1027,
                Enable = true,
                ChargeDate = DateTime.UtcNow,
                ExpireDate = expireDate.ToPeString("yyyy/MM/dd"),
                Name = key.User.Mobile,
                UserName = key.UserName.Trim(),
                ServerId = key.ServerId.Value
            };

            var map = _mapper.Map<SSHKey>(input);
            var keys = new List<SSHKey>();
            keys.Add(map);

            //await DeleteUserFromServer(keys, key.V2Server);

            await AddUserToServer(keys, key.V2Server);

            if (key.V2Server.HasLicense)
            {
                await AddOrUpdateToPanel(input.UserName, input.Password, input.ExpireDate, key.V2Server, "edituser");
            }


            await base.UpdateAsync(key.Id, input);


        }

        public async Task ChangeState(int id)
        {
            try
            {

//                var receiverOptions = new ReceiverOptions()
//                {
//                    AllowedUpdates = new UpdateType[]
//                    {
//UpdateType.Message,
//UpdateType.EditedMessage
//                    }
//                };
//                TelegramBotClient botClient = new TelegramBotClient("6178109792:AAH9P_fd-nMu5lzrE6NaSFlJyTmCEp6-E5M");
//                await botClient.ReceiveAsync(UpdateHander, ErrorHander, receiverOptions);
//                //var chat = await botClient.GetChatAsync("@kazemimstbot)");

                //// Send the message to the account
                //await botClient.SendTextMessageAsync(chat.Id, "1:1");

                var keyInfo = await _db.SSHKeyInfos.Include(a => a.V2Server).FirstOrDefaultAsync(a => a.Id == id);

                keyInfo.Enable = !keyInfo.Enable;
                var keys = new List<SSHKey>();

                if (!keyInfo.Enable)
                {
                    await DeleteUserFromServer(new List<SSHKey> { keyInfo }, keyInfo.V2Server);

                    if (keyInfo.V2Server.HasLicense)
                    {
                        await SuspendUnSuspendFromPanel(keyInfo.UserName, keyInfo.V2Server);
                    }

                }
                else
                {
                    var key = new CreateSSHKeyInput
                    {
                        UserName = keyInfo.UserName,
                        Password = keyInfo.Password,
                        ServerId = keyInfo.ServerId.Value,
                        Count = 1,
                        Port = 1027
                    };
                    var map = _mapper.Map<SSHKey>(key);
                    keys.Add(map);

                    await AddUserToServer(keys, keyInfo.V2Server);

                    if (keyInfo.V2Server.HasLicense)
                    {
                        await SuspendUnSuspendFromPanel(keyInfo.UserName, keyInfo.V2Server, "unsuspenduser");
                    }

                }
                _db.Update(keyInfo);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private Task ErrorHander(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }

        private Task UpdateHander(ITelegramBotClient arg1, Update arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }

        public PasswordConnectionInfo GetConnectionInfo(string url, int port, string password, string userName)
        {
            var result = new PasswordConnectionInfo(url, port, userName, password);
            return result;
        }

        private string CreatePassword()
        {
            int length = 3;
            const string valid = "0123456789";
            StringBuilder res = new();
            Random rnd = new();
            res.Append("Tacpq");

            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString().Trim();
        }
        public override async Task Delete(int id)
        {
            var keyInfo = await _db.SSHKeyInfos.Include(a => a.V2Server).FirstAsync(a => a.Id == id);
            await DeleteFromPanel(keyInfo.UserName, keyInfo.V2Server);
            await base.Delete(id);
        }


        public async Task Recreate(string name)
        {
            var keys = _db.SSHKeyInfos.Where(a => a.UserName.Contains(name)).ToList();
            foreach (var item in keys)
            {
                Thread.Sleep(1000);
                await ChangeState(item.Id);
            }
        }
        public async Task Adjust(int serverId)
        {
            //var keys = await _db.SSHKeyInfos.ToListAsync();

            var enableAccounts = await _db.SSHKeyInfos.Include(a => a.V2Server).Where(a => a.ServerId == serverId && a.Enable).ToListAsync();
            var server = enableAccounts.First().V2Server;
            await DeleteUserFromServer(server.SSHKeys, server);
            Thread.Sleep(2000);
            await AddUserToServer(enableAccounts, server);

            if (server.HasLicense)
            {
                foreach (var item in enableAccounts)
                {
                    await AddOrUpdateToPanel(item.UserName, item.Password, item.ExpireDate.ToPeString(), server);
                }
            }

        }
        public async Task DisableExpired()
        {
            try
            {

                var keys = _db.SSHKeyInfos.Include(new[] { "V2Server" })
                    .Where(c => c.ExpireDate <= DateTime.UtcNow && c.Enable);

                var tt = keys.GroupBy(a => a.ServerId, (id, key) => new { Keys = key.ToList(), serverId = id });
                foreach (var key in tt)
                {
                    await DeleteUserFromServer(key.Keys, _db.V2Servers.Find(key.serverId));
                }
                foreach (var item in keys)
                {
                    //if (item.V2Server.HasLicense)
                    //{
                    //    await SuspendUnSuspendFromPanel(item.UserName, item.V2Server, "suspenduser");
                    //}
                    //else
                    //{
                    if (item.ExpireDate.AddDays(15) < DateTime.Now)
                    {
                        var newItem = await _db.SSHKeyInfos.FirstOrDefaultAsync(a => a.Id == item.Id);
                        _db.SSHKeyInfos.Remove(newItem);
                    }
                    if (item.Enable)
                    {
                        var newItem = await _db.SSHKeyInfos.FirstOrDefaultAsync(a => a.Id == item.Id);
                        newItem.Enable = false;
                        _db.Update(newItem);
                    }

                    //}
                }
                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<GenerateSSHOutput> GetKeyDetails(int userId)
        {
            var userKeyInfo = _db.SSHKeyInfos.Include(c => c.V2Server).FirstOrDefault(c => c.UserId == userId);
            if (userKeyInfo == null)
            {
                return new GenerateSSHOutput();
            }
            return new GenerateSSHOutput
            {
                ExpireDate = userKeyInfo.ExpireDate.ToPeString("yyyy/MM/dd"),
                HostName = userKeyInfo.V2Server.Url,
                Password = userKeyInfo.Password,
                Port = 1027,
                UserName = userKeyInfo.UserName
            };
        }
        public override IQueryable<SSHKey> Filter(SSHKeyFilterInput filter)
        {
            try
            {


                var query = _db.SSHKeyInfos.AsQueryable();

                if (filter.UserName != null && filter.UserName.Length >= 3)
                    query = query.Where(a => a.UserName.Contains(filter.UserName));

                if (filter.Name != null && filter.Name.Length > 4)
                    query = query.Where(a => a.Name.Contains(filter.Name));

                if (filter.Expired)
                {
                    query = query.Where(a => a.ExpireDate.Date <= DateTime.UtcNow.Date);
                }
                if (filter.ServerId != null)
                {
                    query = query.Where(a => a.ServerId == filter.ServerId);
                }


                return query;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task SetUser(int userId, SetPasswordModel model)
        {
            var key = _db.SSHKeyInfos.FirstOrDefault(a => a.UserName == model.UserName && a.Password == model.Password);
            if (key == null)
                throw new ApiException("رمز عبور و نام کاربری اشتباه است");

            key.UserId = userId;
            _db.Update(key);
            _db.SaveChanges();
        }



        private void Connect(SshClient ssh)
        {
            int attempts = 0;
            int _connectiontRetryAttempts = 70;
            do
            {
                try
                {
                    ssh.Connect();
                }
                catch (Renci.SshNet.Common.SshConnectionException)
                {
                    attempts++;
                }
            } while (attempts < _connectiontRetryAttempts && !ssh.IsConnected);
        }


        private string GenerateUser()
        {
            var user = _db.SSHKeyInfos.Max(c => c.Id);
            return $"u{user}";
        }

        private static async Task DeleteFromPanel(string userName, V2Server server)
        {
            try
            {
                if (!server.HasLicense)
                    return;
                var formContent = new FormUrlEncodedContent(new[]
    {
    new KeyValuePair<string, string>("method", "deleteuser"),
    new KeyValuePair<string, string>("username", userName),
});
                var uri = $"http://{server.Url}/apiV1/api.php?token={server.Token}";
                var myHttpClient = new HttpClient();
                var response = await myHttpClient.PostAsync(uri, formContent);
            }
            catch (Exception)
            {

                return;
            }
        }
        private static async Task SuspendUnSuspendFromPanel(string userName, V2Server server, string method = "suspenduser")
        {
            try
            {


                if (!server.HasLicense)
                    return;
                var formContent = new FormUrlEncodedContent(new[]
    {
    new KeyValuePair<string, string>("method", method),
    new KeyValuePair<string, string>("username", userName),
});
                var uri = $"http://{server.Url}/apiV1/api.php?token={server.Token}";
                var myHttpClient = new HttpClient();
                var response = await myHttpClient.PostAsync(uri, formContent);
            }
            catch (Exception)
            {

                return;
            }
        }
        private static async Task AddOrUpdateToPanel(string userName, string password, string expireTimeFa, V2Server? server, string method = "adduser")
        {
            try
            {


                if (!server.HasLicense)
                    return;

                var date = expireTimeFa.ToGeo().ToString("yyyy-MM-dd");
                var formContent = new FormUrlEncodedContent(new[]
                                    {
                                    new KeyValuePair<string, string>("username", userName.Trim()),
                                    new KeyValuePair<string, string>("password", password.Trim()),
                                    new KeyValuePair<string, string>("multiuser", "1"),
                                    new KeyValuePair<string, string>("finishdate","2030-01-01"),
                                    new KeyValuePair<string, string>("traffic", "500000"),
                                    new KeyValuePair<string, string>("method", method),
                                });
                var token = server.Token.Trim();
                //"eb5GXv9LjCy3Omsr";
                var uri = $"http://{server.Url}/apiV1/api.php?token={token}";
                var myHttpClient = new HttpClient();
                var response = await myHttpClient.PostAsync(uri, formContent);
                var contents = await response.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                return;
            }
        }


    }
}
