using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
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

        public PublicDataController(IUserService userService, IRahyabSmsSender rahyab)
        {
            _userService = userService;
            _rahyab = rahyab;
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


        [HttpGet("get-accounType")]
        [Authorize]
        public ApiResponse AccountType()
        {
            var result = Enum.GetValues(typeof(AccountType))
                .Cast<AccountType>()
                .Select(t => new OptionItem { Id = ((int)t), Text = t.GetDescription() });
            return new ApiResponse(result);
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