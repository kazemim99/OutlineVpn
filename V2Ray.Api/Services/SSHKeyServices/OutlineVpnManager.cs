using Renci.SshNet;
using System.Runtime.Intrinsics.X86;
using System.Text;
using Telegram.Bot.Requests;
using V2Ray.Api.Database;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.SSHKeyServices.Dto;

namespace V2Ray.Api.Services.SSHKeyServices
{

    public interface IOutlineVpnManager
    {
        AddAccessKeyOutOut AddAccessKey(int id, string userName, long bytes);
        void DeleteAccessKey(string accessKeyId);
        void UpdateAccessKey(string accessKeyId, string newName, int newDataLimitBytes);
    }
    public class OutlineVpnManager : IOutlineVpnManager
    {
        private string host = "65.109.134.138";
        private string username = "root";
        private string password = "o^qw7^n3LdDhfs5O";
        private readonly DB _db;

        public OutlineVpnManager(DB db)
        {
            _db = db;
        }

        public AddAccessKeyOutOut AddAccessKey(int id,string userName, long bytes)
        {
            var output = new AddAccessKeyOutOut();
            var user = _db.SSHKeyInfos.FirstOrDefault(c => c.UserName == userName  && c.Password.Length > 10);
           
            if (user == null) {
                output.Pass = Guid.NewGuid().ToString();

            }
            using (var client = new SshClient(host,1027, username, password))
            {
                client.Connect();

                // Command to add a new access key to shadowbox_config.json
                string addCommand = "jq '.accessKeys += [{\"id\": \""+ id + "\", \"name\": \""+ userName + "\", \"password\": \""+ output.Pass + "\", \"port\": 14190, \"encryptionMethod\": \"chacha20-ietf-poly1305\", \"dataLimit\": {\"bytes\": " + bytes + "}}]' /opt/outline/persisted-state/shadowbox_config.json > /opt/outline/persisted-state/temp_config.json && mv /opt/outline/persisted-state/temp_config.json /opt/outline/persisted-state/shadowbox_config.json";
                client.RunCommand(addCommand);

                // Restart the Outline service
                client.RunCommand("sudo docker restart shadowbox");

                client.Disconnect();
            }
            if (user == null)
            {
                var phrase = $"chacha20-ietf-poly1305:{output.Pass}";

                byte[] plainBytes = Encoding.UTF8.GetBytes(phrase);
                string encodedString = Convert.ToBase64String(plainBytes);
                var code = $"ss://{encodedString}@ss.iransshvpn.com:14190/?outline=1";
                output.Code = code;
            }
            output.Pass = password;
            return output;
        }

        public void DeleteAccessKey(string accessKeyId)
        {
            var connectionInfo = new PasswordConnectionInfo(host, 1027, "root", "o^qw7^n3LdDhfs5O");

            using (var client = new SshClient(connectionInfo))
            {
                client.Connect();

                // Command to delete an access key by id
                string deleteCommand = $"jq 'del(.accessKeys[] | select(.name == \"{accessKeyId}\"))' /opt/outline/persisted-state/shadowbox_config.json > /opt/outline/persisted-state/temp_config.json && mv /opt/outline/persisted-state/temp_config.json /opt/outline/persisted-state/shadowbox_config.json";
                
                var command = client.CreateCommand(deleteCommand);
                command.Execute();

                // Restart the Outline service
                client.RunCommand("sudo docker restart shadowbox");

                client.Disconnect();
            }
        }

        public void UpdateAccessKey(string accessKeyId, string newName, int newDataLimitBytes)
        {
            using (var client = new SshClient(host, username, password))
            {
                client.Connect();

                // Command to update an access key's name and data limit
                string updateCommand = $"jq '(.accessKeys[] | select(.id == \"{accessKeyId}\") | .name) |= \"{newName}\" | (.accessKeys[] | select(.id == \"{accessKeyId}\") | .dataLimit.bytes) |= {newDataLimitBytes}' /opt/outline/persisted-state/shadowbox_config.json > /opt/outline/persisted-state/temp_config.json && mv /opt/outline/persisted-state/temp_config.json /opt/outline/persisted-state/shadowbox_config.json";
                var command = client.CreateCommand(updateCommand);
                command.Execute();

                // Restart the Outline service
                client.RunCommand("sudo docker restart shadowbox");

                client.Disconnect();
            }
        }
    }


}

