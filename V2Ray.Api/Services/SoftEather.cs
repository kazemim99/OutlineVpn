

using Microsoft.EntityFrameworkCore;
using Renci.SshNet;
using System.Net;
using V2Ray.Api.Database;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.SSHKeyServices;


public interface ISoftEather
{
    void CreateSoftEather(List<SSHKey> users, AccountActionStatus actionStatus = AccountActionStatus.Create);
}
public class SoftEather: ISoftEather
{

    private readonly DB _db;

    public SoftEather(DB db)
    {
        _db = db;
    }

    public async Task CreateSoftEatherNotExist(string userName)
    {
        var user = await _db.SSHKeyInfos.FirstAsync(c => c.UserName == userName);
        CreateSoftEather(new List<SSHKey> { user }, AccountActionStatus.Create);
    }

    public void CreateSoftEather(List<SSHKey> users, AccountActionStatus actionStatus = AccountActionStatus.Create)
    {

        string host = "l.iransshvpn.com";
        string username = "master";
        string password = "!Q@W3e4r";
        IPAddress[] addresses = Dns.GetHostAddresses(host);



        using (var sshClient = new SshClient(addresses[0].ToString(), username, password))
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

  
}