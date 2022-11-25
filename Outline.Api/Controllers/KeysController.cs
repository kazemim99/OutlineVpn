using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outline.Api.Services.UserServices;
using Outline.Api.ViewModels;
using OutlineVpn;

namespace Outline.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class KeysController : CustomBaseController
    {

        private readonly OutlineApi _outline;
        private readonly IUserService _userService;
        public KeysController(IUserService userService)
        {
            _outline = new OutlineApi();
            _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public ApiResponse Create([FromBody] CreateUserViewModel input)
        {
            var output = _outline.CreateKey();
            _outline.RenameKey(output.Id, input.PhoneNumber);
            return new ApiResponse();
        }

        /// <summary>
        /// ویرایش پروفایل یک کاربر
        /// </summary>
        ///
        [Authorize]
        [HttpGet("consumed-traffic")]
        public async Task<ApiResponse> ConsumedTraffic()
        {
            var consumedTraffic = _outline.Capacity(Mobile);
            var user = await _userService.GetById(UserId);
            var result = new HomePageViewModel
            {
                ConsumedTraffic = Convert.ToDouble(consumedTraffic),
                InitTraffic = user.InitCapacity,
                RaminingTraffic = user.InitCapacity - Convert.ToDouble(consumedTraffic)
            };
            return new ApiResponse(result);
        }
    }
}
