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

                if (_db.SSHKeyInfos.Count(c => c.ServerId == input.ServerId && c.Enable) > _db.V2Servers.First(a => a.Id == input.ServerId).Capacity)
                    throw new ApiException("ظرفیت سرور تکمیل است");

                var server = _db.V2Servers.FirstOrDefault(a => a.Id == input.ServerId);


                for (int i = 0; i < input.Count; i++)
                {
                    input.Password = input.Password.IsNullOrEmpty() ? CreatePassword() : input.Password;
                    input.Port = 1027;
                    input.UserName = input.UserName.IsNullOrEmpty() ? GenerateUser() : input.UserName;
                    await AddUserFromServer(new List<SSHKey>
                    {
                        new SSHKey
                        {
                            UserName = input.UserName,
                            Password = input.Password
                        }
                    }, server);

                    input.ChargeDate = DateTime.UtcNow;
                    var id = await base.InsertGetIdAsync(input);

                    _db.Orders.Add(new Order
                    {
                        Amount = input.Amount,
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


        private static async Task DeleteFromPanel(string userName, V2Server server)
        {
            if (!server.HasLicense)
                return;
            var formContent = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("method ", "deleteuser"),
    new KeyValuePair<string, string>("username", userName),
});
            var uri = $"http://{server.Url}/apiV1/api.php?token=DPkHNDErGtEb2ZVf";
            var myHttpClient = new HttpClient();
            var response = await myHttpClient.PostAsync(uri, formContent);
        }
        private static async Task AddToPanel(string userName, string password, V2Server? server)
        {
            if (!server.HasLicense)
                return;
            var formContent = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("username", userName),
    new KeyValuePair<string, string>("password", password),
    new KeyValuePair<string, string>("multiuser", "1"),
    new KeyValuePair<string, string>("finishdate ", "2030-01-31"),
    new KeyValuePair<string, string>("traffic", "5000000"),
    new KeyValuePair<string, string>("method", "adduser"),
});
            var uri = $"http://{server.Url}/apiV1/api.php?token=DPkHNDErGtEb2ZVf";
            var myHttpClient = new HttpClient();
            var response = await myHttpClient.PostAsync(uri, formContent);
        }

        public override async Task UpdateAsync(int id, UpdateSSHKeyInput input, params string[] include)
        {
            var key = _db.SSHKeyInfos.Include(new[] { "V2Server", "Orders" }).Where(a => a.Id == id).ToList();
            var user = key.First();

            input.ChargeDate = user.ChargeDate;
            if (input.ExpireDate.ToGeo().Date > user.ExpireDate.AddDays(7).Date)
            {

                _db.Orders.Add(new Order
                {
                    Amount = input.Amount,
                    SSHKeyId = id,
                    Status = sms.Kavenegar.Models.Enums.OrderStateEnum.Confirmed,
                    CardNumber = "",
                    CreatedAt = DateTime.UtcNow,
                    CreatorUserId = input.UserId,
                    UserId = input.UserId.Value,

                });
                input.ChargeDate = DateTime.UtcNow;

            }


            var map = _mapper.Map<CreateSSHKeyInput>(input);
            map.UserId = input.UserId;

            await DeleteFromPanel(user.UserName, user.V2Server);
            await AddToPanel(user.UserName, user.Password, user.V2Server);
            await DeleteFromVPS(map.UserName, user.V2Server);
            await AddUserFromServer(key.ToList(), user.V2Server);
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


            string str = "";
            var i = 0;
            var connectionInfo = GetConnectionInfo(server.Url, 1027, server.Password, server.UserName);
            using var ssh = new SshClient(connectionInfo);
            Connect(ssh);




            foreach (var item in users)
            {
                //if (server.HasLicense)
                //{
                //    await DeleteFromPanel(item.UserName, server);
                //}
                i++;
                str += $"killall -u {item.UserName} \n ";
            }
            str += "\n";

            var command = ssh.CreateCommand(str);
            command.Execute();

            str = "";
            i = 0;
            foreach (var item in users)
            {
                if (server.HasLicense)
                {
                    await DeleteFromPanel(item.UserName, server);
                }
                //str += $"killall -u {item.UserName} \n deluser --remove-home {item.UserName} \n ";
                str += $"deluser --remove-home {item.UserName} \n ";
            }
            str += "\n";
            var command2 = ssh.CreateCommand(str);
            command2.Execute();

            ssh.Disconnect();
        }
        private async Task AddUserFromServer(List<SSHKey> users, V2Server server)
        {

            string str = "";

            foreach (var item in users)
            {
                await AddToPanel(item.UserName, item.Password, server);
                str += $"useradd -m -p  $(openssl passwd -1 {item.Password}) -s /bin/bash {item.UserName} \n ";
            }



            var connectionInfo = new PasswordConnectionInfo(server.Url, 1027, server.UserName, server.Password);
            var ssh = new SshClient(connectionInfo);
            Connect(ssh);

            var comm = str;
            var command = ssh.CreateCommand(comm);
            command.Execute();
            ssh.Disconnect();
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
                Password = key.Password,
                UserId = null,
                Port = 1027,
                Enable = true,
                ChargeDate = DateTime.UtcNow,
                ExpireDate = expireDate.ToPeString("yyyy/MM/dd"),
                Name = key.User.Mobile,
                UserName = key.UserName,
                ServerId = key.ServerId.Value
            };

            var map = _mapper.Map<SSHKey>(input);
            var keys = new List<SSHKey>();
            keys.Add(map);
            if (key.V2Server.HasLicense)
            {
                await AddToPanel(input.UserName, input.Password, key.V2Server);

            }
            else
            {
                //await DeleteUserFromServer(keys, key.V2Server);
                await AddUserFromServer(keys, key.V2Server);
            }


            await base.UpdateAsync(key.Id, input);


        }


        public async Task ChangeState(int id)
        {
            var keyInfo = await _db.SSHKeyInfos.Include(a => a.V2Server).FirstOrDefaultAsync(a => a.Id == id);

            keyInfo.Enable = !keyInfo.Enable;
            var keys = new List<SSHKey>();

            if (!keyInfo.Enable)
            {
                if (keyInfo.V2Server.HasLicense)
                {
                    await DeleteFromPanel(keyInfo.UserName, keyInfo.V2Server);
                }
                else
                {
                    await DeleteFromVPS(keyInfo.UserName, keyInfo.V2Server);
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
                if (keyInfo.V2Server.HasLicense)
                {
                    await AddToPanel(keyInfo.UserName, keyInfo.Password, keyInfo.V2Server);
                }
                else
                {
                    await AddUserFromServer(keys, keyInfo.V2Server);
                }


            }
            _db.Update(keyInfo);
            _db.SaveChanges();

        }

        //private async void CreateSSHUser(CreateSSHKeyInput input, bool isUpdate)
        //{
        //    try
        //    {
        //        var server = _db.V2Servers.First(a => a.Id == input.ServerId);

        //        try
        //        {
        //            var connectionInfo = GetConnectionInfo(server.Url, server.Port, server.Password, server.UserName);

        //            using var ssh = new SshClient(connectionInfo);
        //            Connect(ssh);
        //            var rrr = ssh.RunCommand("ls /home");

        //            var comm = $"useradd -m -p  $(openssl passwd -1 {input.Password}) -s /bin/bash {input.UserName}";
        //            var command = ssh.CreateCommand(comm);
        //            command.Execute();

        //            ssh.Disconnect();
        //        }
        //        catch (Exception)
        //        {
        //            throw new ApiException($"اتصال به این سرور برقرار نشد {server.Url}");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApiException("ارتباط با سرور برقرار نشد");
        //    }
        //}

        public PasswordConnectionInfo GetConnectionInfo(string url, int port, string password, string userName)
        {
            var result = new PasswordConnectionInfo(url, port, userName, password);
            return result;
        }

        private string CreatePassword()
        {
            int length = 4;
            const string valid = "1369";
            StringBuilder res = new();
            Random rnd = new();
            res.Append("Tacp");

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

        //public async Task ChargeOneMonth(string email)
        //{
        //    var key = await _db.SSHKeyInfos.Include(a => a.User).FirstAsync(a => a.User.Email == email);
        //    key.ExpireDate = DateTime.UtcNow.AddDays(30);
        //    _db.SSHKeyInfos.Update(key);
        //    _db.SaveChanges();
        //}


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

            var server = await _db.V2Servers.Include(a => a.SSHKeys).FirstAsync(a => a.Id == serverId);
            await DeleteUserFromServer(server.SSHKeys, server);
            await AddUserFromServer(server.SSHKeys.Where(a => a.Enable).ToList(), server);
        }
        public async Task DisableExpired()
        {
            try
            {

                var keys = _db.SSHKeyInfos.Include(c => c.V2Server).Where(c => c.ExpireDate <= DateTime.UtcNow).ToList();
                //var servers = keys.Select(a => a.V2Server).Distinct();
                //foreach (var server in servers)
                //{
                //    DeleteUserFromServer(keys.Where(a=>a.ServerId == server.Id).ToList(), server.IP);
                //}
                foreach (var item in keys)
                {
                    var key = _db.SSHKeyInfos.First(a => a.Id == item.Id);
                    if (key.V2Server != null)
                    {
                        await DeleteFromVPS(item.UserName, item.V2Server);

                        item.Enable = false;
                        _db.SSHKeyInfos.Update(key);
                        _db.SaveChanges();
                    }
                }
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

        public async Task DeleteFromVPS(string userName, V2Server server)
        {
            var key = await _db.SSHKeyInfos.Include(a => a.V2Server).FirstOrDefaultAsync(a => a.UserName == userName);
            await DeleteUserFromServer(new List<SSHKey>
            {
                key
            }, key.V2Server);
        }
    }
}
