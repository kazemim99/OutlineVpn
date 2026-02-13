using AutoMapper.Execution;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
using System.Text;
using V2Ray.Api.Database;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.sms;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;
using V2Ray.Api.Services.SSHKeyServices.Dto;
using V2Ray.Api.Services.UserServices;
using V2Ray.Api.Shared;
using V2Ray.Api.Shared.ShadowUriGenerator;

namespace V2Ray.Api.Controllers
{

    /// <summary>
    /// Api های عمومی
    /// </summary>
    ///
    [ApiController]
    [Route("api/[controller]")]
    public class PublicDataController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRahyabSmsSender _rahyab;
        private readonly DB _db;

        public PublicDataController(IUserService userService, IRahyabSmsSender rahyab, DB db)
        {
            _userService = userService;
            _rahyab = rahyab;
            _db = db;
        }


        [HttpGet("get-os")]
        [Authorize]
        public ApiResponse GetOS()
        {
            var result = Enum.GetValues(typeof(OSEnum))
                .Cast<OSEnum>()
                .Select(t => new OptionItem { Id = ((int)t), Text = t.GetDescription() });
            return new ApiResponse(result);
        }



        //[HttpGet("profile/{guid}")]
        //public ApiResponse GetProfile([FromRoute] string guid)
        //{
        //    var user = _db.SSHKeyInfos.First(c => c.V2Guid == guid || c.UserName == guid);

        //    var response = new
        //    {
        //        configString = user.AccountType == Services.SSHKeyServices.Dto.AccountType.V2RAy ? user.Code : user.SSHCode,
        //        username = user.UserName,
        //        expireDate = user.ExpireDate.ToPeString(),
        //        trafficUsage = user.UsedTraffic,
        //        totalTraffic = user.TotalTraffic,
        //        password  =user.Password,
        //        server = $"{GetServer(user.UserId)}.iransshvpn.com",
        //        selectedProtocol = user.AccountType,
           
        //    };
        //    return new ApiResponse(response);
        //}

        [HttpGet("profile/{guid}")]
        public ApiResponse GetTotorianApp([FromRoute] AccountType account)
        {
            var response = new { };
            return new ApiResponse(response);
        }

        /// <summary>
        /// Get account information by username or VLESS URL
        /// VLESS URL format: vless://[GUID]@[server]:[port]?[params]#[username]
        /// Example: vless://9604b197-1335-41b0-9583-db7defcd0a55@v7.iransshvpn.com:27000?type=ws&path=%2F&host=&security=none#u16323
        /// - GUID: Client identifier (V2Ray UUID)
        /// - username: Appears after '#' symbol in the URL fragment
        /// </summary>
        [HttpGet("account-info/{username}")]
        public ApiResponse GetAccountInfo([FromRoute] string username)
        {
            // Extract username from VLESS URL if provided
            // VLESS format: vless://[guid]@[server]:[port]?[params]#[username]
            string extractedUsername = username;

            if (username.StartsWith("vless://", StringComparison.OrdinalIgnoreCase))
            {
                var fragmentIndex = username.IndexOf('#');
                if (fragmentIndex > 0 && fragmentIndex < username.Length - 1)
                {
                    // Extract username from the fragment (part after #)
                    extractedUsername = username.Substring(fragmentIndex + 1);
                    // URL decode in case of encoded characters
                    extractedUsername = Uri.UnescapeDataString(extractedUsername);
                }
            }

            var account = _db.SSHKeyInfos.FirstOrDefault(c => c.UserName == extractedUsername);

            if (account == null)
                throw new ApiException("حساب کاربری یافت نشد");

            var isExpired = account.ExpireDate < DateTime.Now;
            var daysRemaining = (account.ExpireDate - DateTime.Now).Days;

            string expireDateMessage = "";
            if (isExpired)
            {
                expireDateMessage = "حساب شما منقضی شده است";
            }
            else if (daysRemaining <= 3)
            {
                expireDateMessage = $"حساب شما {daysRemaining} روز دیگر منقضی می‌شود";
            }
            else
            {
                expireDateMessage = $"{daysRemaining} روز باقی مانده";
            }

            string trafficMessage = "";
            if (account.TrefficExpired)
            {
                trafficMessage = "ترافیک شما تمام شده است";
            }
            else
            {
                var remainingTraffic = account.TotalTraffic - account.UsedTraffic;
                trafficMessage = $"{remainingTraffic:F2} GB باقی مانده";
            }

            var response = new
            {
                username = account.UserName,
                guid = account.V2Guid, // Client identifier (V2Ray UUID) used in VLESS URL
                usedTraffic = $"{account.UsedTraffic:F2} GB",
                totalTraffic = $"{account.TotalTraffic:F2} GB",
                createDate = account.CreatedAt.ToPeString(),
                expireDate = account.ExpireDate.ToPeString(),
                expireDateMessage = expireDateMessage,
                trafficMessage = trafficMessage,
                isExpired = isExpired,
                trafficExpired = account.TrefficExpired,
                enable = account.Enable,
                enableMessage = account.Enable ? "فعال" : "غیرفعال",
                configUrl = account.Code // Full VLESS configuration URL
            };

            return new ApiResponse(response);
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
        [HttpGet("ChangeConfig/{guid}/{account}")]
        public ApiResponse ChangeConfig([FromRoute] string guid, [FromRoute] AccountType account)
        {
            var user = _db.SSHKeyInfos.First(c => c.V2Guid == guid || c.UserName == guid);

            user.AccountType = account;
            //if (account == Services.SSHKeyServices.Dto.AccountType.SSH)
            //{
            //    user.SSHCode = $"ssh://{user.UserName}:%2{user.Password}@a.iransshvpn.com:1027?LCHepgjuVVy6UQRcXWdT8MFUMaAm31Xu8huIC93UZkqH92e6+WtSSbKYEp0PHKy5#migrate";
            //}
            _db.Update(user);
            _db.SaveChanges();

            return new ApiResponse();
        }


        [HttpGet("get-accounType")]
        public ApiResponse AccountType()
        {
            var result = Enum.GetValues(typeof(AccountType))
                .Cast<AccountType>()
                .Select(t => new OptionItem { Id = ((int)t), Text = t.GetDescription() });
            return new ApiResponse(result);
        }


        [HttpGet("get-wireguard-config-file/{userName}")]
        public IActionResult GetConfig([FromRoute] string userName)
        {
            // Generate the configuration content
            var account = _db.SSHKeyInfos.First(c => c.UserName == userName);

            string configData = GenerateClientConfig(account.WireGuardPrivateKey);
            configData = configData.Replace("\r\n", "");

            // Convert configuration content to byte array
            byte[] bytes = Encoding.UTF8.GetBytes(configData);

            // Return the configuration as a downloadable file
            return File(bytes, "text/plain", "client.conf");
        }
        static string GenerateClientConfig(string privateKey)
        {
            return @$"[Interface]
                        PrivateKey = {privateKey}
                        Address = 176.66.66.2/32
                        DNS = 8.8.8.8
                        
                        [Peer]
                        PublicKey = TYzkMeJzKvbvvZYCrFLKVT3FZQ6wwZRR3gYstZsHzXk=
                        Endpoint =46.245.64.66:55825
                        AllowedIPs = 0.0.0.0/0
                        PersistentKeepalive = 25".Trim();
        }
        [HttpGet("get-operations")]
        [Authorize]
        public ApiResponse GetOpreations()
        {
            var result = Enum.GetValues(typeof(OperatorEnum))
                .Cast<OperatorEnum>()
                .Select(t => new OptionItem { Id = ((int)t), Text = t.GetDescription() });
            return new ApiResponse(result);
        }


        /// <summary>
        /// نمایش فایلها در قسمت در فرانت با اسفتاده از آدرس
        /// </summary>
        ///
        [HttpGet("get-file-p")]
        public async Task<ActionResult> GetFileP()
        {

            string filePath = "Resources/open.ovpn";
            string fileName = "open.ovpn";
            var stream = await OpenReadStreamAsync(filePath);

            string contentType = "";
            var prefex = filePath.Split('.')[1];

            contentType = GetMimeType(fileName);


            return File(stream, contentType);
        }


        /// <summary>
        /// نمایش فایلها در قسمت در فرانت با اسفتاده از آدرس
        /// </summary>
        ///
        [HttpGet("get-file")]
        public async Task<FileStreamResult> GetFile()
        {

            string filePath = "Resources/open.ovpn";
            string fileName = "open.ovpn";
            var stream = await OpenReadStreamAsync(filePath);

            string contentType = "";
            var prefex = filePath.Split('.')[1];

            contentType = GetMimeType(fileName);


            return File(stream, contentType, "open.ovpn");
        }

        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        private async Task<Stream> OpenReadStreamAsync(string file)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), file);
            var memory = new MemoryStream();
            if (!System.IO.File.Exists(path))
                throw new ApiException("File Not Exist");
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            return memory;
        }
    }
}