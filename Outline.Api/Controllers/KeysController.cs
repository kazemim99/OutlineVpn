using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outline.Api.ViewModels;
using OutlineVpn;

namespace Outline.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class KeysController : CustomBaseController
    {

        private readonly OutlineApi _outline;
        public KeysController()
        {
            _outline = new OutlineApi("https://13.232.11.178:44751/w7BTeKeVYCIwb8jPIu94eA");
        }
       
        [HttpPost]
        [AllowAnonymous]
        public ApiResponse Create([FromBody] CreateUserViewModel input)
        {
          var output =  _outline.CreateKey(30);
            _outline.RenameKey(output.Id, input.PhoneNumber);
            return new ApiResponse();
        }

        /// <summary>
        /// ویرایش پروفایل یک کاربر
        /// </summary>
        ///
        [Authorize]
        [HttpGet("capacity/{mobile}")]
        public async Task<ApiResponse> Capacity([FromRoute] string mobile)
        {
            var capacity = 0;
            double? bytes=0;
            var data2 = _outline.GetKeys(); // Get all transferred data
            var user = data2.FirstOrDefault(a => a.Name.Contains(mobile));
            if(user != null)
             bytes = _outline.GetTransferredData().FirstOrDefault(a => a.Id == user.Id)?.UsedBytes;

            if(user != null)
                capacity = Convert.ToInt32(user.UsedBytes / Math.Pow(1024, 2));
            return new ApiResponse(capacity);
        }
    }
}
