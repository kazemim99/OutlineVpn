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
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Net;

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
        private static int version = 19;


        private readonly IMapper _mapper;
        private List<string> NodeIpD = new List<string>
        {
            "79.137.203.191",
            "77.105.146.61",
            "77.221.157.27",
            //"85.192.63.139",
            "85.192.63.147",
            "109.120.176.132",
            //"z1.iranv2ray.com",
            //"z2.iranv2ray.com",
            //"z3.iranv2ray.com",
            //"z4.iranv2ray.com",
            //"z5.iranv2ray.com"
        };



        public SSHKeyService(IMapper mapper, DB db) : base(mapper, db)
        {
            _db = db;
            _mapper = mapper;

        }

        public async Task GenerateSshFromAdmin(CreateSSHKeyInput input)
        {

            //var server = _db.V2Servers.Include(a => a.SSHKeys).FirstOrDefault(a => a.Id == input.ServerId);


            //if (server.SSHKeys.Count(a => a.Enable) >= server.Capacity)
            //    throw new ApiException("ظرفیت سرور تکمیل است");


            if (input.Count > 10)
                throw new ApiException("امکان ساخت بیشتر از ده اکانت همزمان  وجود ندارد");

            var user = _db.Users.Include(c => c.SSHKeyInfos).First(c => c.Id == input.UserId);
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
                keys.Add(new SSHKey
                {

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
                }); ;

                int id = 0;
                if (input.AccountType == AccountType.OutLine)
                {
                    id = await CreateV2Ray(keys, AccountActionStatus.Create, AccountType.OutLine);

                }
                if (input.AccountType == AccountType.V2RAy)
                {
                    id = await CreateV2Ray(keys, AccountActionStatus.Create);
                }
                if (input.AccountType == AccountType.SSH)
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

            input.DurationId = key.DurationId;
            input.UserId = key.UserId;
            input.Enable = key.Enable;
            input.ChargeDate = key.ChargeDate;
            input.ExpireDate = key.ExpireDate.AddDays(input.ExtraDayId).ToPeString("yyyy/MM/dd");
            input.Server = key.Server;
            input.V2Guid = key.V2Guid;
            input.V2Id = key.V2Id;
            input.Code = key.Code;

            //if (input.MultiUser > key.MultiUser)
            //{

            //    var order = _db.Orders.Include(c => c.SSHKey).First(c => c.SSHKey.Id == id);
            //    order.MultiUser = input.MultiUser;
            //    _db.Update(order);
            //    _db.SaveChanges();
            //}
            key.ExpireDate = input.ExpireDate.ToGeo().AddDays(input.ExtraDayId);
            var keys = new List<SSHKey>() { key };
            if (input.AccountType != key.AccountType)
            {
                if (key.AccountType == AccountType.V2RAy)
                {

                    await CreateV2Ray(keys, AccountActionStatus.Delete);
                }
                if (key.AccountType == AccountType.OutLine)
                {
                    await CreateV2Ray(keys, AccountActionStatus.Delete);
                }

                if (key.AccountType == AccountType.SSH)
                {
                    await BulkDeleteServer(keys);
                }
            }

            if (input.AccountType == AccountType.V2RAy)
            {
                await CreateV2Ray(keys, AccountActionStatus.Create);
                return;
            }
            if (input.AccountType == AccountType.OutLine)
            {
                await CreateV2Ray(keys, AccountActionStatus.Create, AccountType.OutLine);
            }
            if (input.AccountType == AccountType.SSH)
            {
                await BulkAddUserToServer(keys);
                input.AccountType = AccountType.SSH;
                await base.UpdateAsync(id, input, include);

            }

        }


        public override async Task<GetSSHKeyOutput> GetById(int id, params string[] include)
        {
            var result = await base.GetById(id, include);


            return result;
        }

        private void DeleteUserFromServerExpired(List<SSHKey> users, V2Server server)
        {
            var i = 0;

            var connectionInfo = new PasswordConnectionInfo(server.Url, 1027, server.UserName, server.Password);
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

            var connectionInfo = new PasswordConnectionInfo(server.Url, 1027, server.UserName, server.Password);
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

            //var r = keys.Any(c =>
            //       c.V2Server.Url.ToLower().StartsWith("r") || c.V2Server.Url.ToLower().StartsWith("ic1"));

            //var d = keys.Any(c =>
            // c.V2Server.Url.ToLower().StartsWith("d"));
            var NodeIps = new List<string>();

            NodeIps = NodeIpD;

            await Task.Run(() => Parallel.ForEach(NodeIps, s =>
            {
                DoSomethingDeleteExpired(s, keys);
            }));

        }

        private async Task BulkDeleteServer(List<SSHKey> keys)
        {


            var NodeIps = NodeIpD;


            //CreateSoftEather(keys, AccountActionStatus.Delete);

            await Task.Run(() => Parallel.ForEach(NodeIps, s =>
            {
                DoSomethingDelete(s, keys);
            }));

        }

        private void DoSomethingDeleteExpired(string ip, List<SSHKey> keys)
        {
            var server = new V2Server();

            server.IP = ip;
            server.Url = ip;
            server.UserName = "root";
            server.Password = "!Q@W3e4r";
            DeleteUserFromServerExpired(keys, server);
        }
        private void DoSomethingDelete(string ip, List<SSHKey> keys)
        {
            var server = new V2Server();

            server.IP = ip;
            server.Url = ip;
            server.UserName = "root";
            server.Password = "!Q@W3e4r";
            DeleteUserFromServer(keys, server);
        }

        public async Task BulkAddUserToServer(List<SSHKey> keys)
        {


            var NodeIps = NodeIpD;
            await Task.Run(() => Parallel.ForEach(NodeIps, s =>
            {
                DoSomething(s, keys);
            }));



        }

        public async Task<int> CreateV2Ray(List<SSHKey> sSHKeys, AccountActionStatus status = AccountActionStatus.Create, AccountType accountType = AccountType.V2RAy)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
            { return true; };

            using var httpClient = new HttpClient(clientHandler)
            {

                Timeout = TimeSpan.FromSeconds(7)
            };

            var baseUrl = await ConnectPanel(httpClient);

            EntityEntry<SSHKey>? entity = null;

            foreach (var item in sSHKeys)
            {
                if (item.V2Id > 0)
                {
                    await httpClient.PostAsync($"{baseUrl}/panel/inbound/del/{item.V2Id}", null);
                    item.V2Id = 0;
                    status = AccountActionStatus.Create;
                }
                item.V2Port = GetV2Port(item.UserId);
                Thread.Sleep(1000);

                var formData = new Dictionary<string, string>();

                if (status == AccountActionStatus.Delete)
                {
                    var postResponse = await httpClient.PostAsync($"{baseUrl}/panel/inbound/{GetSubId(item.UserId)}/delClient/{item.V2Guid}", null);
                    postResponse.EnsureSuccessStatusCode();

                }
        //        else if (status == AccountActionStatus.Update)
        //        {
        //            if (item.V2Guid.IsNullOrEmpty() || item.Code.IsNullOrEmpty())
        //            {
        //                throw new Exception("مشکل شناسه درست در پنل");
        //            }

        //            formData = new Dictionary<string, string>
        //{
        //    { "id", GetSubId(item.UserId) },
        //    { "settings", "{\"clients\":" +
        //    "[" +
        //    "{\"id\":\"" + item.V2Guid + "\"," +
        //    "\"flow\":\"\"," +
        //    "\"email\":\"" + item.UserName + "\"," +
        //    "\"limitIp\":0," +
        //    "\"totalGB\":\"" +(item.DurationId/30) * 52212254720 + "\"," +
        //    "\"expiryTime\":\"" + item.ExpireDate.ToTimeStamp() +"\"," +
        //    "\"enable\":\"" + item.Enable.ToString().ToLower() + "\"," +
        //    "\"tgId\":\"\"," +
        //    "\"subId\":\"jp186e28nard05qn\"," +
        //    "\"reset\":0}]" +
        //    "}"
        //                    },

        //};


        //            var content = new FormUrlEncodedContent(formData);

        //            var postResponse = await httpClient.PostAsync($"{baseUrl}/panel/inbound/updateClient/{item.V2Guid}", content);

        //            postResponse.EnsureSuccessStatusCode();
        //            var contents = await postResponse.Content.ReadAsStringAsync();
        //            int number = new Random().Next(1, 10);
        //            item.Code = $"vless://{item.V2Guid}@v{number}.napstar.ir:{item.V2Port}?type=ws&security=none#{item.UserName}";

        //            _db.SSHKeyInfos.Update(item);
        //        }
                else
                {
                    if (item.V2Guid.IsNullOrEmpty())
                        item.V2Guid = Guid.NewGuid().ToString();

                    //if (item.V2Port == null || item.V2Port == 0)
                    //{
                    //    item.V2Port = RandomPort();
                    //};

                    int number = new Random().Next(1, 10);
                    if (accountType == AccountType.V2RAy)
                    {
                        item.Code = $"vless://{item.V2Guid}@v{number}.napstar.ir:{item.V2Port}?type=ws&security=none#{item.UserName}";

                        formData = new Dictionary<string, string>
        {
            { "id", GetSubId(item.UserId) },
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

                    // Perform POST request to /panel/inbound/add
                    var postResponse = await httpClient.PostAsync($"{baseUrl}/panel/inbound/addClient", content);
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
                        item.AccountType = AccountType.V2RAy;
                        if (_db.SSHKeyInfos.Any(c => c.Id == item.Id))
                        {
                            entity = _db.SSHKeyInfos.Update(item);
                        }
                        else
                        {
                            entity = _db.SSHKeyInfos.Add(item);
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


            }
            return sSHKeys.First().Id;
        }

        private int? GetV2Port(int userId)
        {
            var subId = 25000;
            if (userId == 71)
            {
                subId = 26000;
            }
            if (userId == 37 || userId == 77)
            {
                subId = 27000;
            }
            return subId;
        }

        private string GetSubId(int userId)
        {
            var subId = "215";
            if (userId == 71)
            {
                subId = "217";
            }
            if (userId == 37 || userId == 77)
            {
                subId = "218";
            }
            return subId;
        }

        private string GetExpireTime(int durationId)
        {
            var expire = "2592000000";
            if (durationId == 60)
            {
                expire = "5184000000";
            }
            if (durationId == 90)
            {
                expire = "7776000000";
            }
            return expire;
        }

        private int RandomPort()
        {
            var existProt = true;
            var randomPort = 0;
            while (existProt)
            {
                randomPort = new Random().Next(3000, 50000);
                existProt = _db.SSHKeyInfos.Any(c => c.V2Port == randomPort);

            }
            return randomPort;
        }

        private async Task<string> ConnectPanel(HttpClient httpClient)
        {
            var random = new Random().Next(1, 10);
            string baseUrl = $"http://v20.napstar.ir:1028";

            var loginData = new
            {
                username = "admin",
                password = "!Q@W3e4r"
            };

            //List<KeyValuePair<string, string>> loginData = new List<KeyValuePair<string, string>>();
            //loginData.Add(new KeyValuePair<string, string>("username", "admin"));
            //loginData.Add(new KeyValuePair<string, string>("password", "!Q@W3e4r"));
            //StringContent queryString = new StringContent(JsonConvert.SerializeObject(loginData), UnicodeEncoding.UTF8, "application/json");

            int attempts = 0;
            int _connectiontRetryAttempts = 9;

            try
            {

                var loginResponse = await httpClient.PostAsJsonAsync($"{baseUrl}/login", loginData);
                loginResponse.EnsureSuccessStatusCode();
                var sessionCookie = loginResponse.Headers.GetValues("Set-Cookie").ToString();
                httpClient.DefaultRequestHeaders.Add("Cookie", sessionCookie);

                attempts = _connectiontRetryAttempts;
            }
            catch (Exception ex)
            {
                httpClient.DefaultRequestHeaders.Clear();
                throw new ApiException("عدم ارتباط با پنل لطفا مجدد تلاش کنید تلاش کنید");
            }






            return baseUrl;
        }

        public async Task AjdustNoThread()
        {

            var keyCountn = _db.SSHKeyInfos.Count(c => c.Enable && c.ExpireDate.Date >= DateTime.Now.Date);
            var skipCount = 100M;
            var chunkCount = (int)Math.Ceiling(keyCountn > 100 ? keyCountn / skipCount : keyCountn);

            for (int i = 0; i < chunkCount; i++)
            {
                var newKeys = _db.SSHKeyInfos.Where(c => c.Enable && c.ExpireDate.Date >= DateTime.Now.Date && c.Code == String.Empty).Skip(i * Convert.ToInt32(skipCount)).Take(Convert.ToInt32(skipCount)).ToList();
                await CreateV2Ray(newKeys);
                CreateSoftEather(newKeys);
            }
        }


        private void DoSomething(string ip, List<SSHKey> keys)
        {
            try
            {

                var keyCountn = keys.Count > 0 ? keys.Count : _db.SSHKeyInfos.Count(c => c.Enable && c.ExpireDate.Date >= DateTime.Now.Date);
                var skipCount = 100M;
                var chunkCount = (int)Math.Ceiling(keyCountn > 100 ? keyCountn / skipCount : keyCountn);
                var connectionInfo = new PasswordConnectionInfo(ip, 1027, "root", "!Q@W3e4r");
                using var ssh = new SshClient(connectionInfo);
                Connect(new V2Server(), ssh);

                for (int i = 0; i < chunkCount; i++)
                {

                    var newKeys = new List<SSHKey>();
                    string str = "";
                    if (!keys.Any())
                        newKeys = _db.SSHKeyInfos.Where(c => c.Enable && c.ExpireDate.Date >= DateTime.Now.Date).Skip(i * Convert.ToInt32(skipCount)).Take(Convert.ToInt32(skipCount)).ToList();
                    else
                        newKeys = keys;

                    foreach (var item in newKeys)
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

                }
                //}
            }
            catch (Exception ex)
            {

                throw new ApiException(ex.Message + "=" + ip);
            }

        }

        private async Task AddUserToServer(List<SSHKey> users, V2Server server)
        {
            try
            {


                string str = "";
                CreateSoftEather(users, AccountActionStatus.Update);

                foreach (var item in users)
                {
                    if (!string.IsNullOrEmpty(item.UserName) && !string.IsNullOrEmpty(item.Password))
                    {

                        str += $"sudo useradd -m -p  $(openssl passwd -1 {item.Password}) -s /bin/bash {item.UserName} \n ";
                    }
                }
                var connectionInfo = new PasswordConnectionInfo(server.Url, 1027, server.UserName, server.Password);
                using var ssh = new SshClient(connectionInfo);

                Connect(server, ssh);

                var comm = str;
                var command = ssh.CreateCommand(comm);
                command.Execute();
                ssh.Disconnect();
            }
            catch (Exception ex)
            {

                //    throw new ApiException(ex.Message + "=" + server.IP);

            }

        }

        public async Task Charge(int keyId, int durationId, int userId)
        {

            var key = await _db.SSHKeyInfos.Include(new[] { "User" })
                .FirstAsync(a => a.Id == keyId);
            //if (key.ExpireDate.Date > DateTime.UtcNow.Date.AddDays(10))
            //    throw new ApiException("تاریخ اعتبار به پایان نرسیده");
            //if (key == null)
            //    await GenerateSshFromClient(userId);
            DateTime expireDate = key.ExpireDate.Date <= DateTime.Now.Date ?
                DateTime.UtcNow.AddDays(durationId + 1) :
                key.ExpireDate.AddDays(durationId + 1);




            var input = new UpdateSSHKeyInput
            {
                Password = key.Password.Trim(),

                UserId = key.UserId,
                Port = 1027,
                Enable = true,
                AccountType = key.AccountType,
                ChargeDate = DateTime.UtcNow,
                ExpireDate = durationId <= 0 ? DateTime.Now.ToPeString("yyyy/MM/dd") : expireDate.ToPeString("yyyy/MM/dd"),
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
            var keys = new List<SSHKey>();
            keys.Add(map);



            if (input.DurationId <= 0)
            {
                input.Enable = false;
                if (key.AccountType == AccountType.OutLine)
                    await CreateV2Ray(keys, AccountActionStatus.Delete, AccountType.OutLine);

                if (key.AccountType == AccountType.V2RAy)
                    await CreateV2Ray(keys, AccountActionStatus.Delete);

                if(key.AccountType == AccountType.SSH)
                    await base.UpdateAsync(key.Id, input);
            }

            else
            {

                if (key.AccountType == AccountType.OutLine)
                    await CreateV2Ray(keys, AccountActionStatus.Update, AccountType.OutLine);

                if (key.AccountType == AccountType.V2RAy)
                    await CreateV2Ray(keys, AccountActionStatus.Update);

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

        public async Task ChangeState(int id, bool fromCharge = false)
        {

            var keyInfo = await _db.SSHKeyInfos.FirstOrDefaultAsync(a => a.Id == id);

            keyInfo.Enable = !keyInfo.Enable;
            if (fromCharge)
                keyInfo.Enable = true;


            var keys = new List<SSHKey>();

            if (!keyInfo.Enable)
            {
                if (keyInfo.AccountType == AccountType.V2RAy)
                {
                    await CreateV2Ray(new List<SSHKey> { keyInfo }, AccountActionStatus.Delete, AccountType.V2RAy);
                }

                if (keyInfo.AccountType == AccountType.SSH)
                {
                    await BulkDeleteServer(new List<SSHKey> { keyInfo });
                }

            }
            else
            {

                if (keyInfo.AccountType == AccountType.V2RAy)
                {
                    await CreateV2Ray(new List<SSHKey> { keyInfo }, AccountActionStatus.Create);
                }

                if (keyInfo.AccountType == AccountType.SSH)
                    await BulkAddUserToServer(new List<SSHKey> { keyInfo });


            }
            _db.Update(keyInfo);
            _db.SaveChanges();



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
            var keyInfo = await _db.SSHKeyInfos.FirstAsync(a => a.Id == id);


            var keys = new List<SSHKey>
            {
                keyInfo
            };

            if (keyInfo.AccountType == AccountType.SSH)
            {
                await BulkDeleteServer(keys);
            }


            if (keyInfo.AccountType == AccountType.V2RAy)
            {
                await CreateV2Ray(keys, AccountActionStatus.Delete);
            }

            if (keyInfo.AccountType == AccountType.OutLine)
            {
                await CreateV2Ray(keys, AccountActionStatus.Delete);
            }

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

        public async Task Adjust()
        {
            try
            {


                //await  AjdustNoThread();
                var items = _db.SSHKeyInfos.Where(c => c.V2Id > 0 && c.Code != null).ToList();
                await CreateV2Ray(items);


                //await BulkAddUserToServer(new List<SSHKey>());
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public async Task DisableExpired()
        {
            //await ChangeId();

            var info = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            DateTimeOffset currentTime = TimeZoneInfo.ConvertTime(localServerTime, info);


            var keys = _db.SSHKeyInfos.Where(c => c.ExpireDate <= DateTime.Now).ToList();
            await BulkDeleteServerExpired(keys);
            //var tt = keys.GroupBy(a => a.ServerId, (id, key) => new { Keys = key.ToList(), serverId = id });
            //foreach (var key in tt)
            //{
            //    try
            //    {
            //        var server = _db.V2Servers.FirstOrDefault(a => a.Id == key.serverId);
            //        if (server != null)
            //        {

            //            if (server.HasLoadBalance)
            //            {

            //                // await BulkAddUserToServer(key.Keys);
            //            }
            //            else
            //            {
            //                DeleteUserFromServerExpired(key.Keys, server);
            //            }
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //    }
            //}
           
            
            await CreateV2Ray(keys, AccountActionStatus.Delete);

            foreach (var item in keys)
            {
                try
                {
                    if (item.ExpireDate.AddDays(20) < DateTime.UtcNow)
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
                //if (filter.ServerId != null)
                //{
                //    query = query.Where(a => a.ServerId == filter.ServerId);
                //}


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
            int _connectiontRetryAttempts = 20;
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

            return $"u{user}";
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

        public async Task CreateV2RayNotExist(string userName)
        {
            var user = await _db.SSHKeyInfos.FirstAsync(c => c.UserName == userName);
            await CreateV2Ray(new List<SSHKey>
            {
                user
            });
        }

        public async Task CreateSoftEatherNotExist(string userName)
        {
            var user = await _db.SSHKeyInfos.FirstAsync(c => c.UserName == userName);
            CreateSoftEather(new List<SSHKey> { user }, AccountActionStatus.Create);
        }

        public void CreateSoftEather(List<SSHKey> users, AccountActionStatus actionStatus = AccountActionStatus.Create)
        {

            string host = "212.33.202.66";
            string username = "root";
            string password = "!Q@W3e4r";

            using (var sshClient = new SshClient(host, username, password))
            {
                try
                {

                    sshClient.Connect();

                    if (sshClient.IsConnected)
                    {

                        void DeleteUser()
                        {
                            foreach (var item in users)
                            {
                                ExecuteVpnCommand("UserDelete", item.UserName, sshClient);
                            }
                        }

                        // Create new user
                        void CreateUser()
                        {
                            foreach (var item in users)
                            {
                                var group = "none";
                                if (item.DurationId > 1)
                                {
                                    group = "default";
                                    if (item.MultiUser == 2)
                                    {
                                        group = "default-2";
                                    }
                                    if (item.MultiUser == 3)
                                    {
                                        group = "default-3";
                                    }

                                }

                                ExecuteVpnCommand("UserCreate", $"{item.UserName} /GROUP:{group} /REALNAME:none /NOTE:none", sshClient);
                                ExecuteVpnCommand("UserPasswordSet", $"{item.UserName} /PASSWORD:{item.Password}", sshClient);
                                ExecuteVpnCommand("UserExpiresSet", $@"{item.UserName} /EXPIRES:""{item.ExpireDate.ToString("yyyy/MM/dd HH:mm:ss")}""", sshClient);
                            }
                        }

                        if (actionStatus == AccountActionStatus.Delete)
                        {
                            DeleteUser();
                        }
                        else if (actionStatus == AccountActionStatus.Create)
                        {
                            CreateUser();
                        }
                        else
                        {
                            DeleteUser();
                            CreateUser();
                        }



                        sshClient.Disconnect();
                    }
                    else
                    {
                        throw new Exception("SSH connection failed.");
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message);
                }
            }
        }
        private void ExecuteVpnCommand(string cmd, string args, SshClient sshClient)
        {
            string vpnCmdPath = "/usr/local/vpnserver/vpncmd"; // Path to vpncmd executable
            string vpnHost = "localhost";          // VPN Server hostname or IP address
            string vpnPort = "5555";               // VPN Server management port
            string vpnHub = "vpn";             // VPN Hub name
            var cmdStr = $"sudo {vpnCmdPath} /server {vpnHost}:{vpnPort} /hub:{vpnHub} /password: /cmd:{cmd} {args}";
            using (var command = sshClient.CreateCommand(cmdStr))
            {
                var result = command.Execute();
            }
        }

        public static string Encode(string method, string password, string host, int port)
        {
            // Encode the password to Base64
            string encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

            // Construct the Shadowsocks URI
            string uri = $"ss://{method}:{encodedPassword}@{host}:{port}";

            return uri;
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
        public int up { get; set; }
        public int down { get; set; }
        public int expiryTime { get; set; }
        public int total { get; set; }
        public int reset { get; set; }
    }

    public class Obj
    {
        public int id { get; set; }
        public int up { get; set; }
        public int down { get; set; }
        public int total { get; set; }
        public string remark { get; set; }
        public bool enable { get; set; }
        public object expiryTime { get; set; }
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
}

