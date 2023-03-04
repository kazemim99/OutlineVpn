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

        public async Task Swapp()
        {
            var users = await _db.SSHKeyInfos.Where(a => a.Enable && (a.UserName.Contains("user-4"))).ToListAsync();
          

            foreach (var item in users)
            {
                Thread.Sleep(500);
                
                var connectionInfo = new PasswordConnectionInfo("ssh1.iranv2ray.com", 1027, "root", "!Q@W#E$R5t6y7u8i");
                using var ssh = new SshClient(connectionInfo);
                ssh.Connect();

                var comm = $"useradd -m -p  $(openssl passwd -1 {item.Password}) -s /bin/bash {item.UserName}";
                var command = ssh.CreateCommand(comm);
                command.Execute();
                ssh.Disconnect();

            }
        }
        public async Task DeleteFromVPS(string username)
        {
            var sshKey = await _db.SSHKeyInfos.Where(a => a.UserName == username).Select(c => c.ServerId).FirstOrDefaultAsync();
            var connectionInfo = GetConnectionInfo(sshKey.Value);
            using (var ssh = new SshClient(connectionInfo))
            {
                ssh.Connect();
                var date = DateTime.Now.AddDays(31).ToString("d");
                var com1 = $"killall -u {username}";
                var command = ssh.CreateCommand(com1);
                command.Execute();

                var com2 = $"deluser --remove-home -f {username}";
                var command2 = ssh.CreateCommand(com2);
                command2.Execute();

                //command = ssh.CreateCommand("rm create.txt");
                //command.Execute();

                ssh.Disconnect();
            }
        }
        public override async Task UpdateAsync(int id, UpdateSSHKeyInput input, params string[] include)
        {
            var user = await _db.SSHKeyInfos.Where(a => a.Id == id).Select(c => new { c.Password, c.UserName, c.UserId }).FirstAsync();
            input.Password = input.Password;
            input.UserId = user.UserId;
            await DeleteFromVPS(user.UserName);
            var map = _mapper.Map<CreateSSHKeyInput>(input);
            CreateSSHUser(map, true);
            await base.UpdateAsync(id, input, include);
        }
        public async Task GenerateSshFromAdmin(CreateSSHKeyInput input)
        {
            try
            {
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
            return $"user-{user}";
        }

        public async Task GenerateSshFromClient(int userId)
        {
            var user = await _db.Users.Include(a => a.SSHKeyInfos).FirstAsync(a => a.Id == userId);
            if (user.SSHKeyInfos.Any())
                throw new ApiException("شما قبلا از یوزر تست استفاده کرده اید");

            var input = new CreateSSHKeyInput
            {
                Password = CreatePassword(),
                Port = 1027,
                ExpireDate = DateTime.Now.AddDays(3).ToPersianDate(),
                UserId = userId,
                UserName = user.Email.Split('@')[0]
            };
            CreateSSHUser(input, false);
            await base.InsertAsync(input);

        }

        public async Task ChangeState(int id)
        {
            var keyInfo = await _db.SSHKeyInfos.FirstOrDefaultAsync(a => a.Id == id);
            keyInfo.Enable = !keyInfo.Enable;
            if (!keyInfo.Enable)
            {
                await DeleteFromVPS(keyInfo.UserName);
            }
            else
            {
                CreateSSHUser(new CreateSSHKeyInput
                {
                    ServerId = keyInfo.ServerId.Value,
                    UserName = keyInfo.UserName,
                    ExpireDate = keyInfo.ExpireDate,
                    Password = keyInfo.Password,
                    Count = 1,
                    Port = 1027
                }, false);
            }
            _db.Update(keyInfo);
            _db.SaveChanges();

        }
        public async Task<GenerateSSHOutput> GetUserSSHKey(int userId)
        {
            var user = await _db.SSHKeyInfos.Include("V2Server").FirstOrDefaultAsync(a => a.UserId == userId);
            if (user == null)
                return new GenerateSSHOutput();
            return new GenerateSSHOutput
            {
                UserName = user.UserName,
                Password = user.Password,
                HostName = user.V2Server.Url,
                Port = user.V2Server.Port,
                ExpireDate = user.ExpireDate.ToPeString("yyyy/MM/dd")
            };
        }
        private async void CreateSSHUser(CreateSSHKeyInput input, bool isUpdate)
        {
            try
            {


                var connectionInfo = GetConnectionInfo(input.ServerId);

                using var ssh = new SshClient(connectionInfo);
                ssh.Connect();

                var comm = $"useradd -m -p  $(openssl passwd -1 {input.Password}) -s /bin/bash {input.UserName}";
                var command = ssh.CreateCommand(comm);
                command.Execute();

                ssh.Disconnect();
            }
            catch (Exception)
            {
                throw new ApiException("ارتباط با سرور برقرار نشد");
            }
        }

        public PasswordConnectionInfo GetConnectionInfo(int serverId)
        {
            var sshKey = _db.V2Servers.FirstOrDefault(a => a.Id == serverId);
            var result = new PasswordConnectionInfo(sshKey.Url, sshKey.Port, sshKey.UserName, sshKey.Password);
            return result;
        }

        private string CreatePassword()
        {
            int length = 4;
            const string valid = "1369";
            StringBuilder res = new();
            Random rnd = new();
            res.Append("Vacp");

            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString().Trim();
        }
        public override async Task Delete(int id)
        {
            var keyInfo = _db.SSHKeyInfos.First(a => a.Id == id);
            await DeleteFromVPS(keyInfo.UserName);
            await base.Delete(id);
        }

        public async Task ChargeOneMonth(string email)
        {
            var key = await _db.SSHKeyInfos.Include(a => a.User).FirstAsync(a => a.User.Email == email);
            key.ExpireDate = DateTime.Now.AddDays(30);
            _db.SSHKeyInfos.Update(key);
            _db.SaveChanges();
        }

        public override IQueryable<SSHKey> Filter(SSHKeyFilterInput filter)
        {
            var query = _db.SSHKeyInfos.AsQueryable();

            if (filter.UserName != null)
                query = query.Where(a => a.UserName.Contains(filter.UserName));
            if (filter.Expired)
            {
                query = query.Where(a => a.ExpireDate.Date < DateTime.Now.Date);
            }

            return query;
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
    }
}
