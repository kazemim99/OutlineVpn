using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using System.Text;
using V2Ray.Api.Database;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.V2Keys.Dto;
using Renci.SshNet;
using V2Ray.Api.Services.SSHKeyServices.Dto;
using System.Text.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Linq;

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
                var keys = new List<SSHKey>();
                if (server.SSHKeys.Count(a => a.Enable) > server.Capacity)
                    throw new ApiException("ظرفیت سرور تکمیل است");

                for (int i = 0; i < input.Count; i++)
                {
                    input.Password = input.Password.IsNullOrEmpty() ? CreatePassword() : input.Password;
                    input.Port = 1027;
                    input.UserName = input.UserName.IsNullOrEmpty() ? GenerateUser() : input.UserName;
                    input.ExpireDate = DateTime.UtcNow.AddDays(input.DurationId + input.ExtraDayId).ToPeString("yyyy/MM/dd");

                    //TimeSpan days = input.ExpireDate - input.ChargeDate;

                    //int daysDifference = Math.Abs(days.Days);


                    keys.Add(new SSHKey
                    {
                        UserName = input.UserName,
                        Password = input.Password
                    });







                    input.ChargeDate = DateTime.UtcNow;
                    var id = await base.InsertGetIdAsync(input);


                    if (!input.IsAdmin && input.DurationId != 1)
                    {
                        _db.Orders.Add(new Order
                        {
                            SSHKeyId = id,
                            CreatedAt = DateTime.UtcNow.Date,
                            DurationId = input.DurationId,
                            CreatorUserId = input.UserId,
                            UserId = input.UserId.Value,
                        });
                    }
                    _db.SaveChanges();
                    input.UserName = "";
                    input.Password = "";
                }
                if (server.HasLicense)
                {
                    await BulkAddUserToServer(keys);
                }
                else
                {
                  await  AddUserToServer(keys, server);
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
            if (key.V2Server.HasLicense)
            {
                await BulkDeleteServer(new List<SSHKey> { key });
                await BulkAddUserToServer(new List<SSHKey> { key });
            }
            else
            {
                await DeleteUserFromServer(new List<SSHKey> { key }, key.V2Server);
               await AddUserToServer(new List<SSHKey> { key }, key.V2Server);
            }



        }

        public override async Task UpdateAsync(int id, UpdateSSHKeyInput input, params string[] include)
        {
            var key = _db.SSHKeyInfos.Include(new[] { "V2Server", "Orders" }).Where(a => a.Id == id).ToList();
            var user = key.First();
            if (user.ServerId != input.ServerId)
            {
                var server = _db.V2Servers.First(c => c.Id == input.ServerId);
                ChangeServer(user, server, user.V2Server);
            }

            input.DurationId = user.DurationId;
            input.Enable = user.Enable;
            input.ChargeDate = user.ChargeDate;
            input.ExpireDate = user.ExpireDate.AddDays(input.ExtraDayId).ToPeString("yyyy/MM/dd");
            input.Port = user.Port;




            await base.UpdateAsync(id, input, include);
        }


        public override async Task<GetSSHKeyOutput> GetById(int id, params string[] include)
        {
            var result = await base.GetById(id, include);


            return result;
        }

        private async Task DeleteUserFromServer(List<SSHKey> users, V2Server server)
        {
            try
            {
                var i = 0;

                var ssh = await Connect(server);
                foreach (var item in users)
                {
                    try
                    {


                        if (!string.IsNullOrEmpty(item.UserName))
                        {
                            var str = $"sudo pkill -u {item.UserName}";
                            var command2 = ssh.CreateCommand(str);
                            command2.Execute();
                            str = $"sudo deluser --remove-home {item.UserName}";
                            command2 = ssh.CreateCommand(str);
                            command2.Execute();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                ssh.Disconnect();
            }
            catch (Exception ex)
            {
            }
        }

        private async Task BulkDeleteServer(List<SSHKey> keys)
        {
            List<Instance> instances = await GetInstances();

            var ips = instances.Where(c => c.label.Contains("D")).OrderBy(c => c.label).Select(c => c.main_ip);
            var server = new V2Server();
            foreach (var ip in ips)
            {
                server.IP = ip;
                server.Url = ip;
                server.UserName = "master";
                server.Password = "!Q@W#E$R5t6y7u8i";
                await DeleteUserFromServer(keys, server);
            }
        }

        public async Task BulkAddUserToServer(List<SSHKey> keys)
        {
            List<Instance> instances = await GetInstances();

            // var ips = instances.Where(c => c.label.Contains("D")).OrderBy(c => c.label).Select(c => c.main_ip);
            var ips = new List<string>()
           {
               "64.176.165.231",
               "64.176.167.116",
               "64.176.165.78"
           };
            foreach (var ip in ips)
            {
                var skipCount = 100M;
                var chunkCount = (int)Math.Ceiling(keys.Count > 100 ? keys.Count / skipCount : keys.Count);

                var ssh = await Connect(new V2Server());
                for (int i = 0; i < chunkCount; i++)
                {
                    try
                    {


                        string str = "";

                        foreach (var item in keys.Skip(i * Convert.ToInt32(skipCount)).Take(Convert.ToInt32(skipCount)))
                        {

                            if (!string.IsNullOrEmpty(item.UserName) && !string.IsNullOrEmpty(item.Password))
                            {
                                str += $"sudo useradd -m -p  $(openssl passwd -1 {item.Password}) -s /bin/bash {item.UserName} \n ";

                            }

                        }
                        var comm = str;
                        var command = ssh.CreateCommand(comm);
                        command.Execute();
                    }
                    catch (Exception ex)
                    {


                    }
                }




                ssh.Disconnect();
            }

        }
        private async Task AddUserToServer(List<SSHKey> users, V2Server server)
        {
            string str = "";

            foreach (var item in users)
            {
                if (!string.IsNullOrEmpty(item.UserName) && !string.IsNullOrEmpty(item.Password))
                {
                    str += $"sudo useradd -m -p  $(openssl passwd -1 {item.Password}) -s /bin/bash {item.UserName} \n ";
                }
            }

            var ssh = await Connect(server);

            var comm = str;
            var command = ssh.CreateCommand(comm);
            command.Execute();
            ssh.Disconnect();

        }

        public async Task Charge(int keyId, int durationId, int userId)
        {
            try
            {
                var key = await _db.SSHKeyInfos.Include(new[] { "V2Server", "User" })
                    .FirstAsync(a => a.Id == keyId);
                //if (key.ExpireDate.Date > DateTime.UtcNow.Date.AddDays(10))
                //    throw new ApiException("تاریخ اعتبار به پایان نرسیده");
                //if (key == null)
                //    await GenerateSshFromClient(userId);
                DateTime expireDate = key.ExpireDate.Date <= DateTime.Now.Date ?
                    DateTime.UtcNow.AddDays(durationId) :
                    key.ExpireDate.AddDays(durationId);




                var input = new UpdateSSHKeyInput
                {
                    Password = key.Password.Trim(),
                    UserId = userId,
                    Port = 1027,
                    Enable = true,
                    ChargeDate = DateTime.UtcNow,
                    ExpireDate = expireDate.ToPeString("yyyy/MM/dd"),
                    Name = key.User != null ? key.User.Mobile : " ",
                    UserName = key.UserName.Trim(),
                    ServerId = key.ServerId,

                };

                if (durationId < 0)
                {
                    key.DurationId += durationId;
                    input.DurationId = key.DurationId;
                }
                else
                {
                    input.DurationId = durationId;
                }

                var map = _mapper.Map<SSHKey>(input);
                var keys = new List<SSHKey>();
                keys.Add(map);

                if (key.V2Server.HasLicense)
                {
                    // await BulkDeleteServer(new List<SSHKey> { key });
                }
                else
                {
                    await DeleteUserFromServer(keys, key.V2Server);
                }


                if (input.DurationId <= 0)
                {
                    input.Enable = false;
                    input.ExpireDate = DateTime.UtcNow.ToPeString("yyyy/MM/dd");
                }

                else
                {
                    if (key.V2Server.HasLicense)
                    {

                        await BulkAddUserToServer(keys);
                    }
                    else
                    {
                       await AddUserToServer(keys, key.V2Server);

                    }

                    // Other code can continue executing here

                    // Wait for the task to complete if necessary
                }


                if (key.User != null && !key.User.IsAdmin)
                {
                    if (durationId >= 30)
                    {
                        _db.Orders.Add(new Order
                        {
                            SSHKeyId = key.Id,
                            DurationId = durationId,
                            CreatedAt = DateTime.UtcNow,
                            CreatorUserId = userId,
                            UserId = userId,
                        });
                    }
                    if (durationId <= 0)
                    {
                        if (key.CreatedAt.Date >= DateTime.UtcNow.AddDays(5).Date)
                            throw new ApiException("امکان تغییر تاریخ اکانت بعد از ده رو امکان پذیر نیست");

                        var order = await _db.Orders.Where(c => c.SSHKeyId == keyId).ToListAsync();
                        if (order != null)
                        {
                            order.ForEach(c => c.DurationId += durationId);
                            if (order.Any(c => c.DurationId <= 0))
                            {
                                _db.Orders.RemoveRange(order);
                            }
                            else
                            {
                                _db.Orders.UpdateRange(order);
                            }
                        }
                    }
                }
                await base.UpdateAsync(key.Id, input);


            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task ChangeState(int id, bool fromCharge = false)
        {
            try
            {
                var keyInfo = await _db.SSHKeyInfos.Include(a => a.V2Server).FirstOrDefaultAsync(a => a.Id == id);

                keyInfo.Enable = !keyInfo.Enable;
                if (fromCharge)
                    keyInfo.Enable = true;

                var keys = new List<SSHKey>();

                if (!keyInfo.Enable)
                {
                    if (keyInfo.V2Server.HasLicense)
                    {
                        await BulkDeleteServer(new List<SSHKey> { keyInfo });
                    }
                    else
                    {
                        await DeleteUserFromServer(new List<SSHKey> { keyInfo }, keyInfo.V2Server);

                    }

                }
                else
                {
                    var key = new CreateSSHKeyInput
                    {
                        UserName = keyInfo.UserName,
                        Password = keyInfo.Password,
                        ServerId = keyInfo.ServerId,
                        Count = 1,
                        Port = 1027
                    };
                    var map = _mapper.Map<SSHKey>(key);
                    keys.Add(map);


                    if (keyInfo.V2Server.HasLicense)
                    {
                        await BulkAddUserToServer(new List<SSHKey> { keyInfo });
                    }
                    else
                    {
                      await  AddUserToServer(keys, keyInfo.V2Server);
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
            res.Append("643");
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString().Trim();
        }
        public override async Task Delete(int id)
        {
            var keyInfo = await _db.SSHKeyInfos.Include(a => a.V2Server).FirstAsync(a => a.Id == id);
            if (keyInfo.ExpireDate.Date >= DateTime.Now.AddDays(6))
            {
                var order = await _db.Orders.Where(c => c.SSHKeyId == keyInfo.Id).ToListAsync();
                if (order != null)
                {
                    _db.RemoveRange(order);
                    _db.SaveChanges();
                }

            }
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

        public async Task AddIps(int sreverId)
        {
            var instances = await GetInstances();

            var server = _db.V2Servers.First(c => c.Id == sreverId);

            var instance = instances.FirstOrDefault(c => c.label.ToLower() == server.Title.ToLower());
            var ips = await GetIps(instance.id);
            var ssh = await Connect(server);


            var command2 = ssh.CreateCommand($"echo '{AddIPBashScript(ips, instance.v6_main_ip, instance.internal_ip)}' | sudo tee /etc/netplan/10-ens3.yaml");
            command2.Execute();
            //https://api.vultr.com/v2/instances/{instance-id}/reboot

            command2 = ssh.CreateCommand($"sudo netplan apply");
           await RebootInstance(instance.id);
            command2.Execute();
            ssh.Disconnect();

        }
        public async Task Adjust(int serverId)
        {
            //var keys = await _db.SSHKeyInfos.ToListAsync();
            var servers = _db.V2Servers.Where(c => c.Id == serverId).ToList();



            //var serverLinked = GetLinkedServer(servers.FirstOrDefault());
            //if(serverLinked != null)
            //{
            //    servers.AddRange(_db.V2Servers.Where(c => c.Url.StartsWith(serverLinked)));
            //}

            foreach (var item in servers)
            {

                try
                {
                    if (item.HasLicense)
                    {
                        await BulkSync();
                        return;
                    }
                    //PreInitialScript(item);
                    var accounts = _db.SSHKeyInfos.Where(a => a.ServerId == item.Id);

                    var enableAccounts = accounts.Where(a => a.Enable);
                    await DeleteUserFromServer(accounts.ToList(), item);

                   await AddUserToServer(enableAccounts.ToList(), item);
                }
                catch (Exception ex)
                {

                }

            }

        }

        private string? GetLinkedServer(V2Server? v2Server)
        {
            if (v2Server == null)
                return null;
            var title = v2Server.Url.Split('.')[0].ToLower();
            if (title == "r4")
                return "a";

            if (title == "r5")
                return "b";

            if (title == "d14")
                return "e";

            if (title == "r15")
                return "f";

            if (title == "r6")
                return "g";

            if (title == "r10")
                return "h";



            if (title == "r19")
                return "i";

            //if (title == "r9")
            //    return "k";

            //if (title == "r10")
            //    return "l";

            //if (title == "r11")
            //    return "m";

            //if (title == "r12")
            //    return "o";

            //if (title == "r13")
            //    return "p";

            //if (title == "r14")
            //    return "s";
            //if (title == "r15")
            //    return "t";
            return null;
        }



        private async Task PreInitialScript(V2Server server)
        {
            var client = await Connect(server);


            var command2 = client.CreateCommand($"sudo apt-get update");
            command2.Execute();

            command2 = client.CreateCommand($"sudo apt-get install -y net-tools");
            command2.Execute();
            command2 = client.CreateCommand($"echo '{GetBashScript()}' | sudo tee /usr/local/bin/InoVPN-Single-User.sh");
            command2.Execute();

            command2 = client.CreateCommand($"sudo chmod +x /usr/local/bin/InoVPN-Single-User.sh");
            command2.Execute();

            //command2 = client.CreateCommand($"sudo sh -c 'echo \"account    required     pam_exec.so /usr/local/bin/InoVPN-Single-User.sh\" >> /etc/pam.d/sshd'");
            //command2.Execute();

            //command2 = client.CreateCommand($"sudo sh -c 'echo \"auth       required     pam_exec.so /usr/local/bin/InoVPN-Single-User.sh\" >> /etc/pam.d/sshd'");
            //command2.Execute();

            command2 = client.CreateCommand($"sudo sed -i 's/^\\(\\s*Port\\s*\\)\\d\\+/\\1{1027}/' /etc/ssh/sshd_config");
            command2.Execute();

            command2 = client.CreateCommand($"sudo systemctl restart ssh");
            command2.Execute();
            // Add lines to /etc/pam.d/sshd

            client.Disconnect();

        }

        static void ExecuteCommand(SshClient client, string command)
        {
            using (var commandExec = client.CreateCommand(command))
            {
                var result = commandExec.Execute();
                Console.WriteLine($"Command: {command}\nResult: {result}");
            }
        }


        static string AddIPBashScript(List<Ipv4> ips, string ipv6, string privateIp)
        {
            var gateWay = ips.FirstOrDefault().gateway;
            var ipsStr = string.Join(',', ips.Select(c => c.ip));
            return @$"network:
  version: 2
  renderer: networkd
  ethernets:
    enp1s0:
      dhcp4: no
      addresses: [{ipsStr},'{ipv6}']
      nameservers:
        addresses: [108.61.10.10]
      routes:
      - to: default
        via: {gateWay}
      - to: 169.254.0.0/16
        via: {gateWay}
        metric: 100
";
        }
        static string GetBashScript()
        {
            return @"#!/bin/bash

# Set the maximum allowed concurrent connections
MAX_CONNECTIONS=1

# Get the currently logged-in user from PAM_USER environment variable
CURRENT_USER=""$PAM_USER""

PUBLIC_IP=$(curl -s ipinfo.io/ip)

# Check if the current user is root
if [[ ""$CURRENT_USER"" = ""root"" ] || [ ""$CURRENT_USER"" = ""master"" ]]; then
  # Allow root user to have unlimited concurrent connections
  exit 0
fi

# Check if the current user is online using netstat and grep
LIVE_CONNECTIONS=$(sudo netstat -tnpa | grep 'ESTABLISHED.*sshd' | grep "":1027"" | grep -w ""$CURRENT_USER"" | grep -c ""$PUBLIC_IP:1027"")

# Compare the number of live connections with the maximum allowed connections
if [[ ""$LIVE_CONNECTIONS"" -gt ""$MAX_CONNECTIONS"" ]]; then
  # Deny access if the number of live connections exceeds the maximum allowed
  echo ""Maximum concurrent connections reached. Access denied.""
  exit 1
else
  # Allow access if the number of live connections is within the allowed limit
  exit 0
fi
";
        }



        public async Task DisableExpired()
        {
            //await ShotDownServers();

            //var sskKeys = _db.SSHKeyInfos.Where(c=>c.ChargeDate.Date < DateTime.Now.AddDays(-4).Date);
            //var list = new List<SSHKey>();
            //var orders = new List<Order>();
            //foreach (var key in sskKeys)
            //{
            //   var order = _db.Orders.First(c => c.SSHKey.Id == key.Id);

            //    TimeSpan days = key.ExpireDate - key.ChargeDate;

            //    int daysDifference = Math.Abs(days.Days);
            //    // Get the absolute value of the difference in days
            //    if (daysDifference < 70 && daysDifference > 50)
            //    {
            //        order.DurationId = 60;
            //        key.DurationId = 60;
            //        _db.Update(order);
            //        _db.Update(key);
            //    }


            //    //_db.Update(key);


            //    //_db.SaveChanges();
            //    //_db.SaveChanges();
            //    //list.Add(key);
            //    //orders.Add(order);

            //}
            ////_db.UpdateRange(list);
            ////_db.UpdateRange(orders);
            //_db.SaveChanges();
            bool deleted = false;
            var info = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            DateTimeOffset currentTime = TimeZoneInfo.ConvertTime(localServerTime, info);
            if (currentTime.Hour <= 200 && currentTime.Hour >= 19)
            {

                var keys = _db.SSHKeyInfos
                    .Where(c => c.ExpireDate <= DateTime.Now);

                var tt = keys.GroupBy(a => a.ServerId, (id, key) => new { Keys = key.ToList(), serverId = id });
                foreach (var key in tt)
                {
                    try
                    {
                        var server = _db.V2Servers.FirstOrDefault(a => a.Id == key.serverId);
                        if (server != null)
                        {

                            if (server.HasLicense && !deleted)
                            {
                                await BulkDeleteServer(key.Keys);
                                deleted = true;
                                // await BulkAddUserToServer(key.Keys);
                            }
                            else
                            {
                                await DeleteUserFromServer(key.Keys, server);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                    }
                }
                foreach (var item in keys)
                {
                    try
                    {

                        if (item.ExpireDate.AddDays(15) < DateTime.UtcNow)
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

                    }
                    catch (Exception ex)
                    {

                    }
                    //}
                }
                await _db.SaveChangesAsync();


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


                var query = _db.SSHKeyInfos.Include(a => a.V2Server).AsQueryable();
                if (!filter.IsAdmin)
                {
                    query = query.Where(c => c.V2Server.UserId == filter.UserId);
                }

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



        private async Task<SshClient>  Connect(V2Server server)
        {

            var connectionInfo = new PasswordConnectionInfo(server.Url, 1027, server.UserName, server.Password);
            using var ssh = new SshClient(connectionInfo);
            int attempts = 0;
            int _connectiontRetryAttempts = 70;
            do
            {
                try
                {
                    ssh.Connect();
                    attempts = _connectiontRetryAttempts;
                }
                catch (Renci.SshNet.Common.SshConnectionException)
                {

                    attempts++;
                }
            } while (attempts < _connectiontRetryAttempts && !ssh.IsConnected);

            return ssh;
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

        public async void ChangeServer(SSHKey sshKey, V2Server newServer, V2Server oldServer)
        {
            var lst = new List<SSHKey>
            {
                sshKey
            };

            if (newServer.HasLicense)
            {
                await BulkDeleteServer(lst);
            }
            else
            {
                await DeleteUserFromServer(lst, oldServer);

            }
            Thread.Sleep(1000);
            if (sshKey.Enable)
            {
                if (newServer.HasLicense)
                {
                    await BulkAddUserToServer(lst);
                }
                else
                {
                   await AddUserToServer(lst, newServer);
                }


            }
        }

        public async Task ChangePassword()
        {

            Renci.SshNet.ConnectionInfo ConnNfo = new Renci.SshNet.ConnectionInfo("o.iranv2ray.com", 1027, "root",
           new AuthenticationMethod[]{

                // Pasword based Authentication
                                new PasswordAuthenticationMethod("username","password"),

                // Key Based Authentication (using keys in OpenSSH Format)
                new PrivateKeyAuthenticationMethod("root",new PrivateKeyFile[]{
                    new PrivateKeyFile(@"C:\Users\stocksna\.ssh\id_ed25519")
                }),
           });

            // Execute a (SHELL) Command - prepare upload directory
            using (var sshclient = new SshClient(ConnNfo))
            {
                sshclient.Connect();
                using (var cmd = sshclient.CreateCommand("mkdir -p /tmp/uploadtest && chmod +rw /tmp/uploadtest"))
                {
                    cmd.Execute();
                    Console.WriteLine("Command>" + cmd.CommandText);
                    Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                }
                sshclient.Disconnect();
            }


        }
        public async Task BulkSync()
        {

            var keys = _db.SSHKeyInfos.Include(c => c.V2Server).Where(c => c.V2Server.HasLicense && c.Enable).ToList();
            List<Task> tasks = new List<Task>();
            var ttt = keys.Where(c => c.V2Server.Title == "L");
            //var keys = servers.SelectMany(c => c.SSHKeys).ToList();


            await BulkAddUserToServer(keys);


        }



        public async Task ShotDownServers()
        {



            // Set your Vultr API key
            string apiKey = "H46OMSQRYTYFHAFUDMSTALHANESEVNBIPXKA";

            // Set your server IDs

            // Set the base URL for the Vultr API
            string baseUrl = "https://api.vultr.com/v2/";

            // Set the headers for the HTTP requests
            var headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {apiKey}" },
            { "Accept", "application/json" },
             { "Content-Type", "application/json" }
        };


            var info = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            DateTimeOffset currentTime = TimeZoneInfo.ConvertTime(localServerTime, info);
            // Get the current time
            //if (currentTime.Hour >= 3 && currentTime.Hour < 4 && currentTime.Minute >= 0)
            //{
            //    await ShutdownServers(baseUrl, headers);

            //}

            // Check if the current time is 7:00 AM
            //if (currentTime.Hour >= 7 && currentTime.Hour <= 8 && currentTime.Minute >= 0)
            //{
            await StartServers(baseUrl, headers);

            //}
        }

        public class Instances
        {
            public List<string> instance_ids { get; set; }
        }
        static async Task ShutdownServers(string baseUrl, Dictionary<string, string> headers)
        {
            using (HttpClient client = new HttpClient())
            {
                List<Instance> instances = await GetInstances();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "H46OMSQRYTYFHAFUDMSTALHANESEVNBIPXKA");
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                var input = instances.Where(c => c.power_status == "running").Select(c => c.id);
                var instancessss = new Instances
                {
                    instance_ids = input.ToList(),
                };
                string jsonPayload = JsonConvert.SerializeObject(instancessss);
                // Convert the JSON payload to a StringContent object
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Send a POST request to halt the servers
                HttpResponseMessage response = await client.PostAsync($"{baseUrl}instances/halt", content);



                // 
                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                }
                else
                {
                }
            }
        }


        public async Task<List<Ipv4>> GetIps(string instanceid)
        {

            string baseUrl = "https://api.vultr.com/v2/";

            // Set the headers for the HTTP requests
            var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
             { "Content-Type", "application/json" }
        };
            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "H46OMSQRYTYFHAFUDMSTALHANESEVNBIPXKA");

                // Send a GET request to get the list of instances
                HttpResponseMessage response = await client.GetAsync($"{baseUrl}instances/{instanceid}/reboot", HttpCompletionOption.ResponseHeadersRead);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Parse the JSON response to get a list of instances
                    string content = await response.Content.ReadAsStringAsync();
                    IPRoot instancesResponse = JsonConvert.DeserializeObject<IPRoot>(content);
                    return instancesResponse.ipv4s;
                }
                else
                {
                    Console.WriteLine($"Failed to get instances. Error: {response.StatusCode}");
                    return null;
                }
            }
        }

        public async Task<List<Ipv4>> RebootInstance(string instanceid)
        {
            string baseUrl = "https://api.vultr.com/v2/";

            // Set the headers for the HTTP requests
            var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
             { "Content-Type", "application/json" }
        };
            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "H46OMSQRYTYFHAFUDMSTALHANESEVNBIPXKA");
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

                // Send a GET request to get the list of instances
                HttpResponseMessage response = await client.PostAsync($"{baseUrl}instances/{instanceid}/ipv4", null);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Parse the JSON response to get a list of instances
                    string content = await response.Content.ReadAsStringAsync();
                    IPRoot instancesResponse = JsonConvert.DeserializeObject<IPRoot>(content);
                    return instancesResponse.ipv4s;
                }
                else
                {
                    Console.WriteLine($"Failed to get instances. Error: {response.StatusCode}");
                    return null;
                }
            }
        }
        static async Task<List<Instance>> GetInstances()
        {

            string baseUrl = "https://api.vultr.com/v2/";

            // Set the headers for the HTTP requests
            var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
             { "Content-Type", "application/json" }
        };
            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "H46OMSQRYTYFHAFUDMSTALHANESEVNBIPXKA");

                // Send a GET request to get the list of instances
                HttpResponseMessage response = await client.GetAsync($"{baseUrl}instances", HttpCompletionOption.ResponseHeadersRead);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Parse the JSON response to get a list of instances
                    string content = await response.Content.ReadAsStringAsync();
                    InstancesResponse instancesResponse = JsonConvert.DeserializeObject<InstancesResponse>(content);
                    return instancesResponse.Instances;
                }
                else
                {
                    Console.WriteLine($"Failed to get instances. Error: {response.StatusCode}");
                    return null;
                }
            }
        }

        static async Task StartServers(string baseUrl, Dictionary<string, string> headers)
        {
            using (HttpClient client = new HttpClient())
            {
                List<Instance> instances = await GetInstances();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "H46OMSQRYTYFHAFUDMSTALHANESEVNBIPXKA");
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                var input = instances.Select(c => c.id);
                var instancessss = new Instances
                {
                    instance_ids = input.ToList(),
                };
                string jsonPayload = JsonConvert.SerializeObject(instancessss);
                // Convert the JSON payload to a StringContent object
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Send a POST request to halt the servers
                HttpResponseMessage response = await client.PostAsync($"{baseUrl}instances/start", content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                }
                else
                {
                }
            }
        }


    }


    // Define classes to represent the JSON response structure
    public class InstancesResponse
    {
        public List<Instance> Instances { get; set; }
    }

    public class Instance
    {
        public string id { get; set; }
        public string os { get; set; }
        public int ram { get; set; }
        public int disk { get; set; }
        public string main_ip { get; set; }
        public int vcpu_count { get; set; }
        public string region { get; set; }
        public string plan { get; set; }
        public DateTime date_created { get; set; }
        public string status { get; set; }
        public int allowed_bandwidth { get; set; }
        public string netmask_v4 { get; set; }
        public string gateway_v4 { get; set; }
        public string power_status { get; set; }
        public string server_status { get; set; }
        public string v6_network { get; set; }
        public string v6_main_ip { get; set; }
        public int v6_network_size { get; set; }
        public string label { get; set; }
        public string internal_ip { get; set; }
        public string kvm { get; set; }
        public string hostname { get; set; }
        public int os_id { get; set; }
        public int app_id { get; set; }
        public string image_id { get; set; }
        public string firewall_group_id { get; set; }
        public List<string> features { get; set; }
        public List<string> tags { get; set; }
        public string user_scheme { get; set; }
    }

    public class Root
    {
        public Instance instance { get; set; }
    }



    public class Ipv4
    {
        public string ip { get; set; }
        public string netmask { get; set; }
        public string gateway { get; set; }
        public string type { get; set; }
        public string reverse { get; set; }
    }

    public class Links
    {
        public string next { get; set; }
        public string prev { get; set; }
    }

    public class Meta
    {
        public int total { get; set; }
        public Links links { get; set; }
    }

    public class IPRoot
    {
        public List<Ipv4> ipv4s { get; set; }
        public Meta meta { get; set; }
    }


}

