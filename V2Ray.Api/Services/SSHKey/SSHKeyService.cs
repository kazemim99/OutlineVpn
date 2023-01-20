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
using V2Ray.Api.Services.SSHKeys.Dto;
using V2Ray.Api.Services.V2Keys.Dto;
using Renci.SshNet;

namespace V2Ray.Api.Services.SSHKeys
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

        }
        public async Task DeleteFromVPS(int id)
        {
            var user = await _db.SSHKeyInfos.FirstAsync(a => a.Id == id);
            var connectionInfo = GetConnectionInfo();
            using (var ssh = new SshClient(connectionInfo))
            {
                ssh.Connect();
                var date = DateTime.Now.AddMonths(1).ToString("d");
                var command = ssh.CreateCommand($"deluser --remove-home {user.UserName}");
                command.Execute();

                //command = ssh.CreateCommand("rm create.txt");
                //command.Execute();

                ssh.Disconnect();
            }
        }
        public override async Task UpdateAsync(int id, UpdateSSHKeyInput input, params string[] include)
        {
            await DeleteFromVPS(id);
            var map = _mapper.Map<CreateSSHKeyInput>(input);
            CreateSSHUser(map);
            await base.UpdateAsync(id, input, include);
        }
        public async Task GenerateSshFromAdmin(CreateSSHKeyInput input)
        {
            try
            {

           
                input.Password = CreatePassword(8);
                CreateSSHUser(input);
                await base.InsertAsync(input);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<GenerateSSHOutput> GenerateSshFromClient(int userId)
        {
            var user = await _db.Users.Include(a => a.SSHKeyInfos).FirstAsync(a => a.Id == userId);
            if (user.SSHKeyInfos.Any())
                throw new ApiException("شما قبلا از یوزر تست استفاده کرده اید");

            var input = new CreateSSHKeyInput
            {
                Password = CreatePassword(8),
                ExpireDate = DateTime.Now.AddDays(3),
                UserId = userId,
                UserName = user.Email.Split('@')[0]
            };
            CreateSSHUser(input);

            return new GenerateSSHOutput
            {
                UserName = input.UserName,
                Password = input.Password,
                HostName = "iranv2ray.com",
                Port = 1027,
                ExpireDate = input.ExpireDate.ToPersianDate().ToPeString()
            };

        }
        private void CreateSSHUser(CreateSSHKeyInput input)
        {
            var connectionInfo = GetConnectionInfo();

            using var ssh = new SshClient(connectionInfo);
            ssh.Connect();
            var date = DateTime.Now.AddMonths(1).ToString("d");
            var command = ssh.CreateCommand($"useradd -m -p $(openssl passwd -1 {input.Password}) -s /bin/bash -G sudo {input.UserName}");
            command.Execute();

            //command = ssh.CreateCommand("rm create.txt");
            //command.Execute();

            ssh.Disconnect();
        }

        private static PasswordConnectionInfo GetConnectionInfo()
        {
            return new PasswordConnectionInfo("ssh1.iranv2ray.com", 1027, "root", "!Q@W#E$R5t6y7u8i");
        }

        private string CreatePassword(int length)
        {
            const string valid = "abcdefghjkmnopqrstuvwxyzABCDEFGHJKMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            StringBuilder res = new();
            Random rnd = new();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
