using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.sms;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;
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
        [HttpGet("get-file/{path}")]
        public async Task<ActionResult> GetFile(string path)
        {
            path = path.Replace('*', '\\');
            var stream = await OpenReadStreamAsync(path);

            string contentType = "";
            var prefex = path.Split('.')[1];
            if (prefex.Contains("jpeg") || prefex.Contains("jpg") || prefex.Contains("png")) ;
            contentType = "image/jpeg";
            if (prefex.Contains("text"))
                contentType = "text/*";
            if (prefex.Contains("doc"))
                contentType = "application/msword";
            if (prefex.Contains("docx"))
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            return File(stream, contentType);
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