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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json.Serialization;

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

        private readonly IOutlineVpnManager _outlineVpnManager;
        private readonly ICloudflareDnsUpdater _cloudflareDnsUpdater;

        private readonly IMapper _mapper;
        private List<string> NodeIpD = new List<string>
        {
                "65.109.134.138"
        };




        public SSHKeyService(IMapper mapper, DB db, IOutlineVpnManager outlineVpnManager, ICloudflareDnsUpdater cloudflareDnsUpdater) : base(mapper, db)
        {
            _db = db;
            _mapper = mapper;
            _outlineVpnManager = outlineVpnManager;
            _cloudflareDnsUpdater = cloudflareDnsUpdater;
        }

        public async Task GenerateSshFromAdmin(CreateSSHKeyInput input)
        {

            if (input.Count > 10)
                throw new ApiException("امکان ساخت بیشتر از ده اکانت همزمان  وجود ندارد");



            var user = _db.Users.Include(c => c.SSHKeyInfos).First(c => c.Id == input.UserId);

            //if (user.SSHKeyInfos.Count > 50)
            //    throw new ApiException("امکان ارائه این پروتکل وجود ندارد");

            var ceiling = user.SSHKeyInfos.Count(c => c.Enable) + input.Count;

            if (user.AccountLimit > 0 && user.AccountLimit < ceiling)
            {
                throw new ApiException($"امکان ساخت بیش از {user.AccountLimit} برای شما وجود ندارد");
            }






            for (int i = 0; i < input.Count; i++)
            {
                var keys = new List<SSHKey>();

                input.Password = input.Password.IsNullOrEmpty() ? CreatePassword() : input.Password;
                input.Port = 1027;
                input.UserName = input.UserName.IsNullOrEmpty() ? GenerateUser() : input.UserName;
                input.ExpireDate = DateTime.UtcNow.AddDays(input.DurationId + 1 + input.ExtraDayId).ToPeString("yyyy/MM/dd");
                var server = GetServer(input.UserId);
                input.Server = server;
                input.ChargeDate = DateTime.UtcNow;
                var key = new SSHKey
                {
                    SSHCode = $"ssh://{input.UserName}:{input.Password}@a.iransshvpn.com:1027?LCHepgjuVVy6UQRcXWdT8MFUMaAm31Xu8huIC93UZkqH92e6+WtSSbKYEp0PHKy5#${input.UserName}",
                    UserName = input.UserName,
                    Password = input.Password,
                    Name = input.Name,
                    ChargeDate = input.ChargeDate,
                    Server = input.Server,
                    DurationId = input.DurationId,
                    ExpireDate = input.ExpireDate.ToGeo(),
                    MultiUser = input.MultiUser,
                    UserId = input.UserId.Value,
                    Enable = true,
                    AccountType = input.AccountType,
                };

                if (input.DurationId == 30)
                {
                    key.TotalTraffic = input.UserId == 82 ? 40 : 55;
                }
                else if (input.DurationId == 60)
                {
                    key.TotalTraffic = input.UserId == 82 ? 80 : 110;
                }
                else if (input.DurationId == 90)
                {
                    key.TotalTraffic = input.UserId == 82 ? 120 : 165;
                }

                keys.Add(key);

                int id = 0;


                if (input.AccountType == AccountType.Outline)
                {
                    input.Code = _outlineVpnManager.AddAccessKey(_db.SSHKeyInfos.Max(c => c.Id), key.UserName, 50000000000);
                    id = await base.InsertGetIdAsync(input);
                }
                else if (input.AccountType == AccountType.V2RAy || input.AccountType == AccountType.VMess || input.AccountType == AccountType.IRAN)
                {
                    id = await CreateV2Ray(input.UserId.Value, keys, input.AccountType, AccountActionStatus.Create);
                }
                else if (input.AccountType == AccountType.SSH)
                {
                    await CreateSSH(input, keys);
                    id = await base.InsertGetIdAsync(input);

                }





                input.ChargeDate = DateTime.UtcNow;
                if (input.DurationId != 1)
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




        }


        private object GetLimit(int durationId)
        {
            if (durationId == 60)
                return 100;
            else if (durationId == 90)
                return 150;
            return 50;
        }

        public async Task CreateSSH(CreateSSHKeyInput input, List<SSHKey> keys)
        {


            //TimeSpan days = input.ExpireDate - input.ChargeDate;

            //int daysDifference = Math.Abs(days.Days);
            var server = GetServer(input.UserId);


            await BulkAddUserToServer(keys);


        }
        private string GetServer(int? userId)
        {
            int length = 1;
            string valid = "123456789";
            StringBuilder res = new();
            Random rnd = new();

            if (userId == 41)
            {

                res.Append("r");
            }
            else
            if (userId == 71)
            {
                res.Append("d");
            }
            else
            if (userId == 73)
            {
                valid = "12345";
                res.Append("s");
            }
            else
            {
                valid = "abcefghi";
                res.Append(valid[rnd.Next(valid.Length)]);
                return res.ToString().Trim(); ;
            }
            res.Append(valid[rnd.Next(valid.Length)]);
            return res.ToString().Trim();
        }

        public async Task ChangePassowrd(int id)
        {
            var key = await _db.SSHKeyInfos.FirstAsync(a => a.Id == id);

            key.Password = CreatePassword();
            _db.Update(key);
            _db.SaveChanges();

            await BulkDeleteServer(new List<SSHKey> { key });
            await BulkAddUserToServer(new List<SSHKey> { key });




        }

        public override async Task UpdateAsync(int id, UpdateSSHKeyInput input, params string[] include)
        {

            var key = _db.SSHKeyInfos.Include(new[] { "Orders" }).First(a => a.Id == id);
            //if(input.AccountType == AccountType.SSH && input.UserId == 71)
            //{
            //    throw new ApiException("امکان ارائه این پروتکل وجود ندارد");
            //}

            input.DurationId = key.DurationId;
            input.UserId = key.UserId;
            input.Enable = key.Enable;
            input.ChargeDate = key.ChargeDate;
            input.ExpireDate = key.ExpireDate.AddDays(input.ExtraDayId).ToPeString("yyyy/MM/dd");
            input.Server = key.Server;
            input.V2Guid = key.V2Guid;
            input.V2Id = key.V2Id;
            input.Code = key.Code;
            input.SSHCode = key.SSHCode.IsNullOrEmpty() ? $"ssh://{key.UserName}:{key.Password}@a.iransshvpn.com:1027?LCHepgjuVVy6UQRcXWdT8MFUMaAm31Xu8huIC93UZkqH92e6+WtSSbKYEp0PHKy5#${key.SSHCode}" : key.SSHCode; ;
            key.ExpireDate = input.ExpireDate.ToGeo().AddDays(input.ExtraDayId);
            var keys = new List<SSHKey>() { key };
            if (input.AccountType != key.AccountType)
            {
                if (input.AccountType == AccountType.Outline)
                {

                    key.Code = _outlineVpnManager.AddAccessKey(key.Id, key.UserName, 50000000000);
                    key.AccountType = AccountType.Outline;
                    await BulkDeleteServer(keys);
                    await CreateV2Ray(input.UserId.Value, keys, key.AccountType, AccountActionStatus.Delete);
                    return;

                }
                else if (input.AccountType == AccountType.V2RAy || input.AccountType == AccountType.VMess || input.AccountType == AccountType.IRAN)
                {
                    await BulkDeleteServer(keys);
                    _outlineVpnManager.DeleteAccessKey(key.UserName);
                    await CreateV2Ray(input.UserId.Value, keys, key.AccountType, AccountActionStatus.Delete);
                    await CreateV2Ray(input.UserId.Value, keys, input.AccountType, AccountActionStatus.Create);
                    return;

                }
                else
                {
                    _outlineVpnManager.DeleteAccessKey(key.UserName);
                    await BulkAddUserToServer(keys);
                    await CreateV2Ray(input.UserId.Value, keys, key.AccountType, AccountActionStatus.Delete);
                    return;
                }

            }

            await base.UpdateAsync(id, input, include);

        }


        public override async Task<GetSSHKeyOutput> GetById(int id, params string[] include)
        {
            var result = await base.GetById(id, include);


            return result;
        }

        private void DeleteUserFromServerExpired(List<SSHKey> users, V2Server server)
        {
            var i = 0;

            var connectionInfo = new PasswordConnectionInfo(server.Url, 1027, server.UserName, "o^qw7^n3LdDhfs5O");
            using var ssh = new SshClient(connectionInfo);
            try
            {

                var str = "";
                Connect(server, ssh);
                foreach (var item in users)
                {

                    try
                    {

                        if (!string.IsNullOrEmpty(item.UserName))
                        {
                            str = $"sudo pkill -u {item.UserName} \n";
                            var command2 = ssh.CreateCommand(str);
                            command2.Execute();
                            str += $"sudo deluser --remove-home {item.UserName} \n";
                            var command3 = ssh.CreateCommand(str);
                            command3.Execute();

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

                // throw new ApiException(ex.Message + "=" + server.Url);
            }

        }

        private void DeleteUserFromServer(List<SSHKey> users, V2Server server)
        {
            var i = 0;

            var connectionInfo = new PasswordConnectionInfo(server.Url, 1027, server.UserName, "o^qw7^n3LdDhfs5O");
            using var ssh = new SshClient(connectionInfo);
            try
            {

                var str = "";

                Connect(server, ssh);
                foreach (var item in users)
                {
                    try
                    {

                        if (!string.IsNullOrEmpty(item.UserName))
                        {
                            //str = $"sudo pkill -u {item.UserName} \n";
                            str += $"sudo deluser --remove-home {item.UserName} \n";

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                var command2 = ssh.CreateCommand(str);
                command2.Execute();
                ssh.Disconnect();
            }
            catch (Exception ex)
            {

                // throw new ApiException(ex.Message + "=" + server.Url);
            }

        }

        private async Task BulkDeleteServerExpired(List<SSHKey> keys)
        {


            foreach (var item in NodeIpD)
            {
                DoSomethingDeleteExpired(item, keys);

            }

        }

        private async Task BulkDeleteServer(List<SSHKey> keys)
        {


            foreach (var item in NodeIpD)
            {
                DoSomethingDelete(item, keys);
            }

        }

        private void DoSomethingDeleteExpired(string ip, List<SSHKey> keys)
        {
            var server = new V2Server();

            server.IP = ip;
            server.Url = ip;
            server.UserName = "root";
            server.Password = "o^qw7^n3LdDhfs5O";
            DeleteUserFromServerExpired(keys, server);
        }
        private void DoSomethingDelete(string ip, List<SSHKey> keys)
        {
            var server = new V2Server();

            server.IP = ip;
            server.Url = ip;
            server.UserName = "root";
            server.Password = "o^qw7^n3LdDhfs5O";
            DeleteUserFromServer(keys, server);
        }

        public async Task BulkAddUserToServer(List<SSHKey> keys)
        {


            var NodeIps = NodeIpD;

            int chunkCount = keys.Count;
            if (!keys.Any())
            {
                var keyCountn = _db.SSHKeyInfos.Count(c => c.Enable && c.AccountType == AccountType.SSH && c.ExpireDate.Date >= DateTime.Now.Date);
                var skipCount = 100M;
                chunkCount = (int)Math.Ceiling(keyCountn > 100 ? keyCountn / skipCount : keyCountn);

                foreach (var item in NodeIps)
                {

                    var connectionInfo = new PasswordConnectionInfo(item, 1027, "root", "o^qw7^n3LdDhfs5O");
                    using var ssh = new SshClient(connectionInfo);
                    Connect(new V2Server(), ssh);

                    for (int i = 0; i < chunkCount; i++)
                    {
                        try
                        {


                            var getKeys = await _db.SSHKeyInfos.Where(c => c.Enable && c.ExpireDate.Date >= DateTime.Now.Date &&
                           c.AccountType == AccountType.SSH).Skip(i * Convert.ToInt32(skipCount)).Take(Convert.ToInt32(skipCount))
                           .ToListAsync();

                            await DoSomething(item, getKeys, ssh);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                }
            }


            else
            {
                foreach (var item in NodeIps)
                {
                    var connectionInfo = new PasswordConnectionInfo(item, 1027, "root", "o^qw7^n3LdDhfs5O");
                    using var ssh = new SshClient(connectionInfo);
                    Connect(new V2Server(), ssh);
                    await DoSomething(item, keys, ssh);

                }


            }




        }


        private int? GetV2Port(int userId, AccountType accountType)
        {
            var subId = accountType == AccountType.VMess ? 700 : 27000;
            if (userId == 71)
            {
                subId = accountType == AccountType.VMess ? 600 : 26000;
            }
            if (userId == 41)
            {
                subId = accountType == AccountType.VMess ? 500 : 25000;
            }

            if (accountType == AccountType.IRAN)
                subId = 38000;

            return subId;
        }






        private string ConnectPanel(int userId, AccountType accountType)
        {
            if (accountType == AccountType.IRAN)
                return "http://irv.iransshvpn.com:180/W5TFZcn3ia";

            var baseUrls = "http://v27.iransshvpn.com";

            if (userId == 41) //ramin
            {
                baseUrls = "http://v25.iransshvpn.com";
            }
            if (userId == 71)
            {
                baseUrls = "http://v26.iransshvpn.com";
            }
            return baseUrls;
        }


        private async Task DoSomething(string ip, List<SSHKey> keys, SshClient ssh)
        {
            try
            {



                var newKeys = new List<SSHKey>();
                string str = "";


                foreach (var item in keys)
                {


                    if (!string.IsNullOrEmpty(item.UserName) && !string.IsNullOrEmpty(item.Password))
                    {

                        str += $"sudo useradd -m -p  $(openssl passwd -1 {item.Password}) -s /bin/bash {item.UserName} \n ";

                    }

                }
                if (!str.IsNullOrEmpty())
                {
                    var command = ssh.CreateCommand(str);
                    command.Execute();
                }

                //}
            }
            catch (Exception ex)
            {

                throw new ApiException(ex.Message + "=" + ip);
            }

        }


        public async Task Charge(int keyId, int durationId, int userId)
        {
            try
            {


                var month = durationId / 30;
                var key = _db.SSHKeyInfos.Include(new[] { "User" })
                    .AsNoTracking().First(a => a.Id == keyId);

                //if (userId == 71 && key.AccountType == AccountType.SSH)
                //    throw new ApiException("امکان ارائه این پروتکل وجود ندارد");


                DateTime expireDate = key.ExpireDate.Date <= DateTime.Now.Date ?
                DateTime.UtcNow.AddMonths(month) :
                key.ExpireDate.AddMonths(month);


                if (expireDate.Date < DateTime.Now.Date)
                {
                    expireDate = DateTime.Now;
                }

                var input = new UpdateSSHKeyInput
                {
                    Password = key.Password.Trim(),
                    UsedTraffic = 0,
                    UserId = key.UserId,
                    Port = 1027,
                    Enable = true,
                    AccountType = key.AccountType,
                    ChargeDate = DateTime.UtcNow,
                    ExpireDate = expireDate.ToPeString("yyyy/MM/dd"),
                    Name = key.User != null ? key.User.Mobile : " ",
                    UserName = key.UserName.Trim(),
                    Server = key.Server,
                    V2Id = key.V2Id,
                    V2Port = key.V2Port,
                    V2Guid = key.V2Guid,
                    Code = key.Code,
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
                map.Id = key.Id;

                if (month == 1)
                {
                    map.TotalTraffic = input.UserId == 82 ? 40 : 55;
                }
                else if (month == 2)
                {
                    map.TotalTraffic = input.UserId == 82 ? 80 : 110;
                }
                else if (month == 3)
                {
                    map.TotalTraffic = input.UserId == 82 ? 120 : 165;
                }
                var keys = new List<SSHKey>()
            {
                map
            };



                if (input.DurationId <= 0)
                {
                    keys.First().Enable = false;

                    if (key.AccountType == AccountType.V2RAy || key.AccountType == AccountType.VMess || key.AccountType == AccountType.IRAN)
                    {
                        await CreateV2Ray(userId, keys, input.AccountType, AccountActionStatus.Delete);
                    }
                }

                else
                {

                    if (key.AccountType == AccountType.V2RAy || key.AccountType == AccountType.VMess || key.AccountType == AccountType.IRAN)
                    {
                        keys.First().UsedTraffic = 0;

                        await CreateV2Ray(userId, keys, input.AccountType, AccountActionStatus.Update);

                    }

                    if (key.AccountType == AccountType.SSH)
                    {
                        await BulkAddUserToServer(keys);
                        await base.UpdateAsync(key.Id, input);
                    }
                }

                if (key.User != null)
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
                _db.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task ChangeState(int id, int currentUserId, bool fromCharge = false)
        {
            var keyInfo = await _db.SSHKeyInfos.FirstOrDefaultAsync(a => a.Id == id);
            keyInfo.SSHCode = $"ssh://{keyInfo.UserName}:{keyInfo.Password}@a.iransshvpn.com:1027?LCHepgjuVVy6UQRcXWdT8MFUMaAm31Xu8huIC93UZkqH92e6+WtSSbKYEp0PHKy5#${keyInfo.UserName}";

            keyInfo.Enable = !keyInfo.Enable;

            if (fromCharge)
                keyInfo.Enable = true;


            var keys = new List<SSHKey>();

            if (!keyInfo.Enable)
            {

                if (keyInfo.AccountType == AccountType.V2RAy || keyInfo.AccountType == AccountType.VMess || keyInfo.AccountType == AccountType.IRAN)
                {


                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    { return true; };

                    using var httpClient = new HttpClient(clientHandler)
                    {

                        Timeout = TimeSpan.FromSeconds(360)
                    };

                    var baseUrls = ConnectPanel(keyInfo.UserId, keyInfo.AccountType);
                    var loginData = new
                    {
                        username = "admin",
                        password = "!Q@W3e4r"
                    };

                    if (keyInfo.AccountType == AccountType.IRAN)
                    {
                        loginData = new
                        {
                            username = "Mostafa",
                            password = "M0staf@136$"
                        };
                    }


                    var loginResponse = await httpClient.PostAsJsonAsync($"{baseUrls}/login", loginData);
                    loginResponse.EnsureSuccessStatusCode();
                    var sessionCookie = loginResponse.Headers.GetValues("Set-Cookie").ToString();
                    httpClient.DefaultRequestHeaders.Add("Cookie", sessionCookie);

                    httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
                    var panelresult = await httpClient.PostAsJsonAsync<Root>($"{baseUrls}/panel/inbound/list", null);

                    var contents = await panelresult.Content.ReadAsStringAsync();
                    var accounts = JsonConvert.DeserializeObject<Root>(contents);
                    var inbounds = accounts.obj.Where(c => c.enable).SelectMany(c => c.clientStats).FirstOrDefault(c => c.email == keyInfo.UserName);
                    if (inbounds != null)
                    {
                        //keyInfo.UsedTraffic = BytesToGigabytes(inbounds.down);
                        //_db.Update(keyInfo);
                        //_db.SaveChanges();
                    }
                    await CreateV2Ray(currentUserId, new List<SSHKey> { keyInfo }, keyInfo.AccountType, AccountActionStatus.Delete,true);
                    if (keyInfo.AccountType != AccountType.IRAN)
                    {
                        return;
                    }
                    _db.Update(keyInfo);
                }


                else if (keyInfo.AccountType == AccountType.SSH)
                {
                    await BulkDeleteServer(new List<SSHKey> { keyInfo });
                    _db.Update(keyInfo);

                }
                else if (keyInfo.AccountType == AccountType.Outline)
                {
                    _outlineVpnManager.DeleteAccessKey(keyInfo.UserName);
                    _db.Update(keyInfo);
                }
            }
            else
            {
                if (keyInfo.AccountType == AccountType.V2RAy || keyInfo.AccountType == AccountType.VMess || keyInfo.AccountType == AccountType.IRAN)
                {

                    if (keyInfo.DurationId == 30)
                    {
                        keyInfo.TotalTraffic = keyInfo.UserId == 82 ? 40 : 55;
                    }
                    else if (keyInfo.DurationId == 60)
                    {
                        keyInfo.TotalTraffic = keyInfo.UserId == 82 ? 80 : 110;
                    }
                    else if (keyInfo.DurationId == 90)
                    {
                        keyInfo.TotalTraffic = keyInfo.UserId == 82 ? 120 : 165;
                    }
                    await CreateV2Ray(currentUserId, new List<SSHKey> { keyInfo }, keyInfo.AccountType, AccountActionStatus.Create,true);
                }

                else if (keyInfo.AccountType == AccountType.SSH)
                {
                    await BulkAddUserToServer(new List<SSHKey> { keyInfo });

                    _db.Update(keyInfo);

                }

                else if (keyInfo.AccountType == AccountType.Outline)
                {
                    _outlineVpnManager.AddAccessKey(keyInfo.Id, keyInfo.UserName, 50000000000);
                    _db.Update(keyInfo);
                }

            }


            _db.SaveChanges();



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
            var keyInfo = await _db.SSHKeyInfos.FirstAsync(a => a.Id == id);


            var keys = new List<SSHKey>
            {
                keyInfo
            };

            if (keyInfo.AccountType == AccountType.SSH)
            {
                await BulkDeleteServer(keys);
            }


            if (keyInfo.AccountType == AccountType.V2RAy || keyInfo.AccountType == AccountType.VMess || keyInfo.AccountType == AccountType.IRAN)
            {
                await CreateV2Ray(keyInfo.UserId, keys, keyInfo.AccountType, AccountActionStatus.Delete);
            }

            //if (keyInfo.AccountType == AccountType.L2TP || keyInfo.AccountType == AccountType.OpenVPN)
            //{
            //    CreateSoftEather(keys, AccountActionStatus.Delete);
            //}

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


        //public async Task Recreate(string name)
        //{
        //    var keys = _db.SSHKeyInfos.Where(a => a.UserName.Contains(name)).ToList();
        //    foreach (var item in keys)
        //    {
        //        Thread.Sleep(1000);
        //        await ChangeState(item.Id, item.UserId);
        //    }
        //}

        public async Task Adjust()
        {
            try
            {


                //await  AjdustNoThread();
                //var items = _db.SSHKeyInfos.Where(c => c.ExpireDate.Date > DateTime.Now.Date && c.Enable).ToList();
                //await CreateV2Ray(items);

                await BulkAddUserToServer(new List<SSHKey>());
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task AdjustV2()
        {
            try
            {



                var items = _db.SSHKeyInfos.Where(c => c.ExpireDate.Date > DateTime.Now.Date && c.Enable).ToList();

                //await CreateV2Ray(41, items.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 41).ToList(), AccountType.V2RAy);
                //await CreateV2Ray(41, items.Where(c => c.AccountType == AccountType.VMess && c.UserId == 41).ToList(), AccountType.VMess);

                //await CreateV2Ray(71, items.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 71).ToList(), AccountType.V2RAy);
                //await CreateV2Ray(71, items.Where(c => c.AccountType == AccountType.VMess && c.UserId == 71).ToList(), AccountType.VMess);

                //await CreateV2Ray(77, items.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 77).ToList(), AccountType.V2RAy);
                //await CreateV2Ray(77, items.Where(c => c.AccountType == AccountType.VMess && c.UserId == 77).ToList(), AccountType.VMess);

                //await CreateV2Ray(77, items.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 76).ToList(), AccountType.V2RAy);
                //await CreateV2Ray(77, items.Where(c => c.AccountType == AccountType.VMess && c.UserId == 76).ToList(), AccountType.VMess);

            }
            catch (Exception ex)
            {


            }
        }
        public async Task UpdateUserTraffic()
        {

            await _cloudflareDnsUpdater.UpdateDnsRecordAsync();

            var users = new List<int>()
            {
                1,//27
                41,//v25
                71,//v25
            };
            foreach (var item in users)
            {

                try
                {


                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    { return true; };

                    using var httpClient = new HttpClient(clientHandler)
                    {

                        Timeout = TimeSpan.FromSeconds(360)
                    };

                    var baseUrls = ConnectPanel(item, AccountType.V2RAy);

                    var loginData = new
                    {
                        username = "admin",
                        password = "!Q@W3e4r"
                    };

                    var loginResponse = await httpClient.PostAsJsonAsync($"{baseUrls}/login", loginData);
                    loginResponse.EnsureSuccessStatusCode();
                    var sessionCookie = loginResponse.Headers.GetValues("Set-Cookie").ToString();
                    httpClient.DefaultRequestHeaders.Add("Cookie", sessionCookie);


                    httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
                    var panelresult = await httpClient.PostAsJsonAsync<Root>($"{baseUrls}/panel/inbound/list", null);

                    var contents = await panelresult.Content.ReadAsStringAsync();
                    var accounts = JsonConvert.DeserializeObject<Root>(contents);
                    var inbounds = accounts.obj.Where(c => c.enable);

                    foreach (var cliens in inbounds.Select(d => d.clientStats))
                    {
                        foreach (var item2 in cliens)
                        {

                            var key = _db.SSHKeyInfos.FirstOrDefault(c => c.UserName == item2.email);

                            if (item2.down > 42949672960)
                            {
                                item2.down += 9398809120;
                            }
                            if (key != null)
                            {
                                try
                                {
                                    var usage = BytesToGigabytes(item2.down);
                                    var calTraffic = (key.UsedTraffic - usage);
                                    if (item2.down > 1000 && key.UsedTraffic <= 0)
                                    {
                                        key.UsedTraffic = 1;
                                    }
                                    key.UsedTraffic = BytesToGigabytes(item2.down);
                                    if (key.DurationId == 30)
                                    {
                                        key.TotalTraffic = key.UserId == 83 ? 40 : 55;
                                    }
                                    else if (key.DurationId == 60)
                                    {
                                        key.TotalTraffic = key.UserId == 83 ? 80 : 110;
                                    }
                                    else if (key.DurationId == 90)
                                    {
                                        key.TotalTraffic = key.UserId == 83 ? 120 : 165;
                                    }
                                    else
                                    {
                                        key.TotalTraffic = 50;
                                    }
                                    if (key.TotalTraffic < key.UsedTraffic)
                                    {
                                        key.TrefficExpired = true;
                                    }
                                    _db.Update(key);
                                    _db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    Thread.Sleep(1000);
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }

            }

        }
        static int BytesToGigabytes(long bytes)
        {
            return Convert.ToInt32((double)bytes / (1024D * 1024D * 1024D));
        }


        public async Task DisableExpired()
        {

            var info = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            DateTimeOffset currentTime = TimeZoneInfo.ConvertTime(localServerTime, info);


            var keys = _db.SSHKeyInfos.OrderByDescending(c => c.ExpireDate).Where(c => (c.ExpireDate <= DateTime.Now || c.UsedTraffic > c.TotalTraffic)).ToList();


            try
            {

                await CreateV2Ray(41, keys.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 41).ToList(), AccountType.V2RAy, AccountActionStatus.Delete);
                await CreateV2Ray(41, keys.Where(c => c.AccountType == AccountType.VMess && c.UserId == 41).ToList(), AccountType.VMess, AccountActionStatus.Delete);
                await CreateV2Ray(71, keys.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 71).ToList(), AccountType.V2RAy, AccountActionStatus.Delete);
                await CreateV2Ray(71, keys.Where(c => c.AccountType == AccountType.VMess && c.UserId == 71).ToList(), AccountType.VMess, AccountActionStatus.Delete);
                await CreateV2Ray(77, keys.Where(c => c.AccountType == AccountType.V2RAy && c.UserId == 77 || c.UserId == 76).ToList(), AccountType.V2RAy, AccountActionStatus.Delete);
                await CreateV2Ray(77, keys.Where(c => c.AccountType == AccountType.VMess && c.UserId == 77 || c.UserId == 76).ToList(), AccountType.VMess, AccountActionStatus.Delete);


                foreach (var key in keys.Where(c => c.AccountType == AccountType.Outline))
                {
                    _outlineVpnManager.DeleteAccessKey(key.UserName);

                }
                //await CreateIranAccount(keys.Where(c => c.AccountType == AccountType.IRAN).ToList(), AccountActionStatus.Delete);
            }
            catch (Exception ex)
            {

            }
            await BulkDeleteServerExpired(keys.Where(c => c.AccountType == AccountType.SSH).ToList());

            foreach (var item in keys)
            {
                try
                {
                    var newItem = await _db.SSHKeyInfos.FirstOrDefaultAsync(a => a.Id == item.Id);
                    if (item.Enable)
                    {
                        newItem.Enable = false;
                        _db.Update(newItem);
                    }
                    if (item.ExpireDate.AddDays(15) < DateTime.UtcNow)
                    {
                        //_db.SSHKeyInfos.Remove(newItem);
                    }


                }
                catch (Exception ex)
                {

                }
                //}
            }
            await _db.SaveChangesAsync();




        }


        public async Task<GenerateSSHOutput> GetKeyDetails(int userId)
        {
            var userKeyInfo = _db.SSHKeyInfos.FirstOrDefault(c => c.UserId == userId);
            if (userKeyInfo == null)
            {
                return new GenerateSSHOutput();
            }
            return new GenerateSSHOutput
            {
                ExpireDate = userKeyInfo.ExpireDate.ToPeString("yyyy/MM/dd"),
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

                if (!filter.IsAdmin)
                {
                    query = query.Where(c => c.UserId == filter.UserId);
                }

                if (filter.UserName != null && filter.UserName.Length >= 3)
                    query = query.Where(a => a.UserName.Contains(filter.UserName));

                if (filter.Password != null && filter.Password.Length > 5)
                    query = query.Where(a => a.Password.Contains(filter.Password));

                if (filter.Name != null && filter.Name.Length > 4)
                    query = query.Where(a => a.Name.Contains(filter.Name));

                if (filter.Expired)
                {
                    query = query.Where(a => a.ExpireDate.Date <= DateTime.UtcNow.Date);
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

        private void Connect(V2Server server, SshClient ssh)
        {

            int attempts = 0;
            int _connectiontRetryAttempts = 50;
            do
            {
                try
                {
                    ssh.Connect();
                    attempts = _connectiontRetryAttempts;
                }
                catch (Renci.SshNet.Common.SshConnectionException ex)
                {

                    attempts++;
                    if (attempts >= _connectiontRetryAttempts)
                    {
                        throw new Exception("اتصال به سرور : " + ssh.ConnectionInfo.Host);
                    }
                }
            } while (attempts < _connectiontRetryAttempts && !ssh.IsConnected);

        }


        private string GenerateUser()
        {
            var user = _db.SSHKeyInfos.Max(c => c.Id);
            if (user < 100)
                user += 100;

            return $"u{user + 300}";
        }


        public async void ChangeServer(SSHKey sshKey, V2Server newServer, V2Server oldServer)
        {

            var server = _db.V2Servers.Include(a => a.SSHKeys).FirstOrDefault(a => a.Id == newServer.Id);
            //sshKey.V2Server = newServer;

            if (server.SSHKeys.Count(a => a.Enable) >= server.Capacity)
                throw new ApiException("ظرفیت سرور تکمیل است");


            var lst = new List<SSHKey>
            {
                sshKey
            };

            //if (server.HasLoadBalance)
            //{
            await BulkDeleteServer(lst);
            //}
            //else
            //{
            //    DeleteUserFromServer(lst, oldServer);
            //}
            Thread.Sleep(1000);
            if (sshKey.Enable)
            {
                //if (server.HasLoadBalance)
                //{
                await BulkAddUserToServer(lst);
                //}
                //else
                //{
                //    await AddUserToServer(lst, newServer);
                //}


            }
        }


        public async Task<int> CreateV2Ray(int currenUserId, List<SSHKey> sSHKeys, AccountType accountType, AccountActionStatus status = AccountActionStatus.Create,bool isSync =false)
        {
            if (!sSHKeys.Any())
                return 0;

            if (accountType == AccountType.IRAN)
            {
                await CreateIranAccount(sSHKeys, status,isSync);
                return 0;
            }
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
            { return true; };

            using var httpClient = new HttpClient(clientHandler)
            {

                Timeout = TimeSpan.FromSeconds(7)
            };

            var panelUrl = ConnectPanel(currenUserId, accountType);




            var loginData = new
            {
                username = "admin",
                password = "!Q@W3e4r"
            };

            StringContent queryString = new StringContent(JsonConvert.SerializeObject(loginData), UnicodeEncoding.UTF8, "application/json");


            EntityEntry<SSHKey>? entity = null;



            var loginResponse = await httpClient.PostAsJsonAsync($"{panelUrl}/login", loginData);
            loginResponse.EnsureSuccessStatusCode();
            var sessionCookie = loginResponse.Headers.GetValues("Set-Cookie").ToString();
            httpClient.DefaultRequestHeaders.Add("Cookie", sessionCookie);

            foreach (var item in sSHKeys)
            {

                item.V2Port = GetV2Port(item.UserId, accountType);


                var formData = new Dictionary<string, string>();
                var number = GetUserNumber(currenUserId);

                if (status == AccountActionStatus.Delete)
                {

                    var secSub = accountType == AccountType.VMess ? number.VmessSubId : number.SubId;
                    var secUrl = panelUrl;

                    try
                    {
                        var url = $"{secUrl}/panel/inbound/{secSub}/delClient/{item.V2Guid}";
                        var postResponse = await httpClient.PostAsync($"{url}", null);
                        postResponse.EnsureSuccessStatusCode();
                    }

                    catch (Exception ex)
                    {


                    }




                    item.AccountType = accountType;
                    entity = _db.SSHKeyInfos.Update(item);
                    _db.SaveChanges();

                }
                else
                {
                    if (item.V2Guid.IsNullOrEmpty())
                        item.V2Guid = Guid.NewGuid().ToString();


                    if (accountType == AccountType.VMess)
                    {
                        var config = new
                        {
                            v = number.VmessSubId,
                            ps = item.UserName,
                            add = $"v{number.Domain}.iransshvpn.com",
                            port = number.VmessPort,
                            id = $"{item.V2Guid}",
                            scy = "auto",
                            net = "tcp",
                            type = "none",
                            tls = "none"
                        };

                        string json = JsonConvert.SerializeObject(config);
                        string base64Config = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

                        item.Code = $"vmess://{base64Config}";

                        formData = new Dictionary<string, string>
{
    { "id", number.VmessSubId.ToString() },
    { "settings", "{\"clients\":" +
    "[" +
    "{\"flow\":\"\"," +
    "\"id\":\"" + item.V2Guid + "\"," +
    "\"email\":\"" + item.UserName + "\"," +
    "\"limitIp\":0," +
    "\"totalGB\":\"" +(item.DurationId/30) * 53687091200 + "\"," +
    "\"expiryTime\":\"" +0 +"\"," +
    "\"enable\":true," +
    "\"tgId\":0," +

    "\"reset\":0" +
    "}" +
    "]" +
    "}"
                    },

};

                    }


                    if (accountType == AccountType.V2RAy)
                    {
                        number = GetUserNumber(currenUserId);
                        item.Code = $"vless://{item.V2Guid}@v{number.Domain}.iransshvpn.com:{number.Port}?type=ws&path=%2F&host=&security=none#{item.UserName}";

                        formData = new Dictionary<string, string>
        {
            { "id", number.SubId.ToString() },
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
                    }


                    // Encode the form data
                    var content = new FormUrlEncodedContent(formData);
                    try
                    {

                        var url = $"{panelUrl}/panel/inbound/addClient";
                        // Perform POST request to /panel/inbound/add
                        var postResponse = await httpClient.PostAsync($"{url}", content);
                        postResponse.EnsureSuccessStatusCode();
                        var contents = await postResponse.Content.ReadAsStringAsync();

                        var jsonObject = JObject.Parse(contents);

                        var success = (bool)jsonObject["success"];

                        if (!((string)jsonObject["msg"]).Contains("Duplicate") && !success)
                        {
                            throw new ApiException((string)jsonObject["msg"]);
                        }
                        else
                        {
                            item.AccountType = accountType;
                            if (_db.SSHKeyInfos.Any(c => c.Id == item.Id))
                            {
                                entity = _db.SSHKeyInfos.Update(item);
                                _db.SaveChanges();
                            }
                            else
                            {
                                entity = _db.SSHKeyInfos.Add(item);
                                _db.SaveChanges();
                            }
                            try
                            {
                                var result = _db.SaveChanges();
                                item.Id = entity.Entity.Id;
                            }
                            catch (Exception ex)
                            {

                                throw;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }


            }



            return sSHKeys.First().Id;
        }

        public async Task<string> CreateIranAccount(List<SSHKey> sSHKeys, AccountActionStatus status, bool isSync = false)
        {
            var url = "http://irv.iransshvpn.com:180/W5TFZcn3ia";

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
                if (!isSync && status != AccountActionStatus.Delete)
                {
                    if (item.V2Guid.IsNullOrEmpty())
                        item.V2Guid = Guid.NewGuid().ToString();

                    item.IsSynced = isSync;
                    item.Code = $"vless://{item.V2Guid}@irv.iransshvpn.com:38000?type=ws&path=%2F&host=&security=none#{item.UserName}";
                    item.V2Port = 38000;
                }
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
                        if (isSync)
                        {
                            _db.Update(item);
                        }
                        else
                        {
                            _db.Add(item);
                        }
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
            return sSHKeys.First().Code;
        }

        private ConfigDateOutput GetUserNumber(int userId)
        {

            var data = new ConfigDateOutput()
            {
                Domain = 7,
                Port = 27000,
                SubId = 3,
                VmessSubId = 6,
                VmessPort = 700,
            };
            if (userId == 71)//danial
            {
                data.Domain = 6;
                data.Port = 26000;
                data.SubId = 2;
                data.VmessSubId = 5;
                data.VmessPort = 600;
            }
            if (userId == 41)//ramin
            {
                data.Domain = 5;
                data.Port = 25000;
                data.SubId = 1;
                data.VmessSubId = 4;
                data.VmessPort = 500;
            }
            return data;
        }

    }
    public enum AccountActionStatus
    {
        Create,
        Update,
        Delete,
    }

    public class ClientStat
    {
        public int id { get; set; }
        public int inboundId { get; set; }
        public bool enable { get; set; }
        public string email { get; set; }
        public object up { get; set; }
        public long down { get; set; }
        public long expiryTime { get; set; }
        public object total { get; set; }
        public int reset { get; set; }
    }

    public class Obj
    {
        public int id { get; set; }
        public long up { get; set; }
        public long down { get; set; }
        public long total { get; set; }
        public string remark { get; set; }
        public bool enable { get; set; }
        public long expiryTime { get; set; }
        public List<ClientStat> clientStats { get; set; }
        public string listen { get; set; }
        public int port { get; set; }
        public string protocol { get; set; }
        public string settings { get; set; }
        public string streamSettings { get; set; }
        public string tag { get; set; }
        public string sniffing { get; set; }
    }

    public class Root
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public List<Obj> obj { get; set; }
    }






    public class AccessKey
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("accessUrl")]
        public string AccessUrl { get; set; }
    }

    public class AccessKeyListOutput
    {
        [JsonPropertyName("accessKeys")]
        public List<AccessKey> AccessKeys { get; set; }
    }


}

