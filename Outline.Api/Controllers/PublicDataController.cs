using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outline.Api.Services.UserServices;

namespace PowerBox.Api.Controllers
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


        public PublicDataController( IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// لیست وضعیتها کاربر
        /// </summary>
        ///
        //[HttpGet("user-states")]
        //[Authorize]
        //public ApiResponse UserStates()
        //{
        //    var result = Enum.GetValues(typeof(UserStateEnum))
        //        .Cast<UserStateEnum>()
        //        .Select(t => new OptionItem { Id = ((int)t), Text = t.GetDescription() });
        //    return new ApiResponse(result);
        //}

     

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