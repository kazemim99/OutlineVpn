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
        public async Task DeleteFromVPS(string username)
        {
            var sshKey = _db.SSHKeyInfos.Where(a => a.UserName == username).Select(c => c.ServerId).FirstOrDefault();
            var connectionInfo =await GetConnectionInfo(sshKey.Value);
            using (var ssh = new SshClient(connectionInfo))
            {
                ssh.Connect();
                var date = DateTime.Now.AddMonths(1).ToString("d");
                var command = ssh.CreateCommand($"deluser --remove-home {username} && deluser {username}");
                command.Execute();

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

                input.Password = input.Password.IsNullOrEmpty() ? CreatePassword(8):input.Password;
                input.Port = 1027;
                CreateSSHUser(input, false);
                await base.InsertAsync(input);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task GenerateSshFromClient(int userId)
        {
            var user = await _db.Users.Include(a => a.SSHKeyInfos).FirstAsync(a => a.Id == userId);
            if (user.SSHKeyInfos.Any())
                throw new ApiException("شما قبلا از یوزر تست استفاده کرده اید");

            var input = new CreateSSHKeyInput
            {
                Password = CreatePassword(8),
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
            var keyInfo =await _db.SSHKeyInfos.FirstOrDefaultAsync(a=>a.Id == id);
            keyInfo.Enable = !keyInfo.Enable;
            if (!keyInfo.Enable)
            {
                await DeleteFromVPS(keyInfo.UserName);
            }
            else
            {
                CreateSSHUser(new CreateSSHKeyInput
                {
                    UserName = keyInfo.UserName,
                    ExpireDate = keyInfo.ExpireDate,
                    Password = keyInfo.Password,
                    Port = 1027
                },false);
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
            var connectionInfo =await GetConnectionInfo(input.ServerId);

            using var ssh = new SshClient(connectionInfo);
            ssh.Connect();



            if (!isUpdate)
            {
                //var addPort = $"echo -e '{input.UserName}  hard    maxlogins   1' >> /etc/security/limits.conf";
                //var addPortCommand = ssh.CreateCommand(addPort);
                //addPortCommand.Execute();
            }
            //var date = DateTime.Now.AddMonths(1).ToString("d");
            var comm = $"useradd -m -p  $(openssl passwd -1 {input.Password}) -s /bin/bash {input.UserName}";
            var command = ssh.CreateCommand(comm);
            command.Execute();


            //command = ssh.CreateCommand("systemctl restart ssh.service");
            //command.Execute();

            ssh.Disconnect();
        }

        private async  Task<PasswordConnectionInfo> GetConnectionInfo(int serverId)
        {
            var sshKey =await _db.V2Servers.Where(a=>a.Id == serverId)
                .FirstOrDefaultAsync();

            return new PasswordConnectionInfo(sshKey.Url, sshKey.Port, sshKey.UserName, sshKey.Password);
        }

        private string CreatePassword(int length)
        {
            const string valid = "abcdefghjkmnopqrstuvwxyzABCDEFGHJKMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new();
            Random rnd = new();
            res.Append("Va");

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
            key.ExpireDate = DateTime.Now.AddMonths(1);
            _db.SSHKeyInfos.Update(key);
            _db.SaveChanges();
        }

        public override IQueryable<SSHKey> Filter(SSHKeyFilterInput filter)
        {
            var query = _db.SSHKeyInfos.AsQueryable();

            if (filter.UserName != null )
                query = query.Where(a => a.UserName.Contains(filter.UserName));

            return query;
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
