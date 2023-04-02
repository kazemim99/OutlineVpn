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



        public async Task Swapp2(string url)
        {
            var users = await _db.SSHKeyInfos.Include(a => a.V2Server)
                .Where(a => a.V2Server.Url == url && a.Enable).ToListAsync();

            var usersD = await _db.SSHKeyInfos.Include(a => a.V2Server)
               .Where(a => a.V2Server.Url != url).ToListAsync();
            var server = users.First().V2Server;
            DeleteUserFromServer(usersD, "95.179.237.94");
            AddUserFromServer(users, "95.179.237.94");

        }


        private void DeleteUserFromServer(List<SSHKey> users, string ip)
        {
            string str = "";
            var i = 0;
            var connectionInfo = GetConnectionInfo(ip, 1027, "!Q@W#E$R5t6y7u8i", "root");
            using var ssh = new SshClient(connectionInfo);
            Connect(ssh);
            foreach (var item in users)
            {
                i++;
                var app = "&&";
                if (i >= users.Count())
                {
                    app = "";
                }
                str += $"killall -u {item.UserName} \n ";
            }
            str += "\n";

            var command = ssh.CreateCommand(str);
            command.Execute();

            str = "";
            i = 0;
            foreach (var item in users)
            {
                i++;
                var app = "&&";
                if (i >= users.Count())
                {
                    app = "";
                }
                str += $"deluser --remove-home {item.UserName} \n";
            }
            str += "\n";
            var command2 = ssh.CreateCommand(str);
            command2.Execute();

            ssh.Disconnect();
        }
        private void AddUserFromServer(List<SSHKey> users, string ip)
        {
            string str = "";
            var i = 0;
            foreach (var item in users)
            {
                i++;
                var app = "&&";
                if (i >= users.Count())
                {
                    app = "";
                }
                str += $"useradd -m -p  $(openssl passwd -1 {item.Password}) -s /bin/bash {item.UserName} \n ";
            }


            var connectionInfo = new PasswordConnectionInfo(ip, 1027, "root", "!Q@W#E$R5t6y7u8i");
            var ssh = new SshClient(connectionInfo);
            Connect(ssh);

            var comm = str;
            var command = ssh.CreateCommand(comm);
            command.Execute();
            ssh.Disconnect();
        }

        public async Task Swapp()
        {
            int attempts = 0;
            int _connectiontRetryAttempts = 50;
            var users = await _db.SSHKeyInfos.Include(a => a.V2Server).Where(a => a.Enable).ToListAsync();
            //var servers = await _db.V2Servers.ToListAsync();
            //foreach (var server in servers)
            //{
            foreach (var item in users)
            {
                var connectionInfo = new PasswordConnectionInfo("45.77.66.228", 1027, "root", "!Q@W#E$R5t6y7u8i");
                using var ssh = new SshClient(connectionInfo);

                Connect(ssh);

                var comm = $"useradd -m -p  $(openssl passwd -1 {item.Password}) -s /bin/bash {item.UserName}";
                var command = ssh.CreateCommand(comm);
                command.Execute();
                ssh.Disconnect();

            }
            //} 
        }
        //public async Task Onlines()
        //{
        //    var connectionInfo = new PasswordConnectionInfo("45.77.66.228", 1027, "root", "!Q@W#E$R5t6y7u8i");
        //    using var ssh = new SshClient(connectionInfo);

        //    Connect(ssh);
        //    string command = "sudo lsof -i | awk '!seen[$9]++ {print $1,$2,$3,$9,$10}'";
        //    ProcessStartInfo processInfo = new ProcessStartInfo("/bin/bash", "-c \"" + command + "\"");
        //     ssh.CreateCommand(comm);
        //    command.Execute();
        //    processInfo.RedirectStandardOutput = true;

        //    Process process = new Process();
        //    process.StartInfo = processInfo;
        //    process.Start();
        //    process.WaitForExit();

        //    string result = process.StandardOutput.ReadToEnd();

        //}
        private void Connect(SshClient ssh)
        {
            int attempts = 0;
            int _connectiontRetryAttempts = 50;
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

        public async Task DeleteFromVPS(string username, V2Server server)
        {

            foreach (var ip in server.IP.Split(','))
            {
                try
                {
                    var connectionInfo = GetConnectionInfo(ip, server.Port, server.Password, server.UserName);
                    using (var ssh = new SshClient(connectionInfo))
                    {
                        Connect(ssh);
                        var com1 = $"killall -u {username}";
                        var command = ssh.CreateCommand(com1);
                        command.Execute();

                        var com2 = $"deluser --remove-home -f {username}";
                        var command2 = ssh.CreateCommand(com2);
                        command2.Execute();

                        ssh.Disconnect();
                    }
                }
                catch (Exception)
                {

                    throw new ApiException($"اتصال به این سرور برقرار نشد {ip}");
                }

            }

        }
        public override async Task UpdateAsync(int id, UpdateSSHKeyInput input, params string[] include)
        {
            var user = _db.SSHKeyInfos.Where(a => a.Id == id).Select(c => new { c.Password, c.UserName, c.V2Server }).First();
            if (input.ServerId != user.V2Server.Id)
            {
                if (_db.SSHKeyInfos.Count(c => c.ServerId == input.ServerId && c.Enable) > _db.V2Servers.First(a => a.Id == input.ServerId).Capacity)
                    throw new ApiException("ظرفیت سرور تکمیل است");
            }
            if (user.V2Server != null)
            {
                await DeleteFromVPS(user.UserName, user.V2Server);
            }
            var map = _mapper.Map<CreateSSHKeyInput>(input);
            map.UserId = null;
            CreateSSHUser(map, true);
            await base.UpdateAsync(id, input, include);
        }
        public async Task GenerateSshFromAdmin(CreateSSHKeyInput input)
        {
            try
            {
                if (_db.SSHKeyInfos.Count(c => c.ServerId == input.ServerId && c.Enable) > _db.V2Servers.First(a => a.Id == input.ServerId).Capacity)
                    throw new ApiException("ظرفیت سرور تکمیل است");

                input.ExpireDate = input.ExpireDate == null ? DateTime.Now.AddDays(31).ToPersianDate() : input.ExpireDate;
                for (int i = 0; i < input.Count; i++)
                {
                    input.Password = input.Password.IsNullOrEmpty() ? CreatePassword() : input.Password;
                    input.Port = 1027;
                    input.UserName = input.UserName.IsNullOrEmpty() ? GenerateUser() : input.UserName;
                    CreateSSHUser(input, false);
                    await base.InsertAsync(input);
                    input.UserName = "";
                    input.Password = "";
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private string GenerateUser()
        {
            var user = _db.SSHKeyInfos.Max(c => c.Id);
            return $"u{user}";
        }

        public async Task GenerateSshFromClient(int userId)
        {
            var user = await _db.Users.Include(a => a.SSHKeyInfos).FirstAsync(a => a.Id == userId);
            if (user.SSHKeyInfos.Any())
                throw new ApiException("شما قبلا از یوزر تست استفاده کرده اید");

            var input = new CreateSSHKeyInput
            {
                Password = CreatePassword(),
                UserId = userId,
                Port = 1027,
                ExpireDate = DateTime.Now.AddHours(2).ToPersianDate(),
                Name = user.Mobile,
                UserName = GenerateUser(),
                ServerId = _db.V2Servers.First(a => a.SSHKeys.Count() < 50 && !a.Url.StartsWith("r")).Id
            };
            CreateSSHUser(input, false);
            await base.InsertAsync(input);
        }

        public async Task ChangeState(int id)
        {
            var keyInfo = await _db.SSHKeyInfos.Include(a => a.V2Server).FirstOrDefaultAsync(a => a.Id == id);

            keyInfo.Enable = !keyInfo.Enable;

            if (!keyInfo.Enable)
            {

                await DeleteFromVPS(keyInfo.UserName, keyInfo.V2Server);
            }
            else
            {
                CreateSSHUser(new CreateSSHKeyInput
                {
                    UserName = keyInfo.UserName,
                    ExpireDate = keyInfo.ExpireDate,
                    Password = keyInfo.Password,
                    ServerId = keyInfo.ServerId.Value,
                    Count = 1,
                    Port = 1027
                }, false);
            }

            _db.Update(keyInfo);
            _db.SaveChanges();

        }

        private async void CreateSSHUser(CreateSSHKeyInput input, bool isUpdate)
        {
            try
            {
                var server = _db.V2Servers.First(a => a.Id == input.ServerId);
                foreach (var ip in server.IP.Split(','))
                {
                    try
                    {
                        var connectionInfo = GetConnectionInfo(ip, server.Port, server.Password, server.UserName);

                        using var ssh = new SshClient(connectionInfo);
                        Connect(ssh);

                        var comm = $"useradd -m -p  $(openssl passwd -1 {input.Password}) -s /bin/bash {input.UserName}";
                        var command = ssh.CreateCommand(comm);
                        command.Execute();

                        ssh.Disconnect();
                    }
                    catch (Exception)
                    {
                        throw new ApiException($"اتصال به این سرور برقرار نشد {ip}");
                    }

                }


            }
            catch (Exception ex)
            {
                throw new ApiException("ارتباط با سرور برقرار نشد");
            }
        }

        public PasswordConnectionInfo GetConnectionInfo(string ip, int port, string password, string userName)
        {
            var result = new PasswordConnectionInfo(ip, port, userName, password);
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
            await DeleteFromVPS(keyInfo.UserName, keyInfo.V2Server);
            await base.Delete(id);
        }

        //public async Task ChargeOneMonth(string email)
        //{
        //    var key = await _db.SSHKeyInfos.Include(a => a.User).FirstAsync(a => a.User.Email == email);
        //    key.ExpireDate = DateTime.Now.AddDays(30);
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
            var keys = await _db.SSHKeyInfos.Include(c => c.V2Server).Where(a => a.ServerId != serverId).ToListAsync();
            var server = await _db.V2Servers.FirstAsync(a => a.Id == serverId);
            DeleteUserFromServer(keys, server.IP);
        }
        public async Task DisableExpired()
        {
            try
            {

                var keys = _db.SSHKeyInfos.Include(c => c.V2Server).Where(c => c.ExpireDate.Date <= DateTime.Now.Date).ToList();
                //var servers = keys.Select(a => a.V2Server).Distinct();
                //foreach (var server in servers)
                //{
                //    DeleteUserFromServer(keys.Where(a=>a.ServerId == server.Id).ToList(), server.IP);
                //}
                foreach (var item in keys)
                {
                    var key = _db.SSHKeyInfos.First(a => a.Id == item.Id);
                    await DeleteFromVPS(item.UserName, item.V2Server);
                    if (item.ExpireDate.Date < DateTime.Now.AddDays(-10))
                    {

                        _db.SSHKeyInfos.Remove(key);
                        _db.SaveChanges();

                    }
                    else
                    {
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
        //private int GeneratePort(DB db)
        //{
        //    var port = 0;
        //    do
        //    {
        //        port = new Random().Next(10000, 60000);

        //    }
        //    while (db.SSHKeyInfos.Any(a => a.Port == port));

        //    return port;
        //}

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
            var query = _db.SSHKeyInfos.AsQueryable();

            if (filter.UserName != null)
                query = query.Where(a => a.UserName.Contains(filter.UserName));
            if (filter.Expired)
            {
                query = query.Where(a => a.ExpireDate.Date <= DateTime.Now.Date);
            }

            return query;
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
    }
}
