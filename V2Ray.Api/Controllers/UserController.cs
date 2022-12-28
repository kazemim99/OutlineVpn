using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using V2Ray.Api.Services.sms;
using V2Ray.Api.Services.sms.Rahyab;
using V2Ray.Api.Services.UserServices;
using V2Ray.Api.Services.UserServices.Dto;

namespace V2Ray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : CustomBaseController
    {
        private readonly IUserService _service;
        private readonly IRahyabSmsSender _rahyabSmsSender;
        public UserController(IUserService userService, IRahyabSmsSender rahyabSmsSender)
        {
            _service = userService;
            _rahyabSmsSender = rahyabSmsSender;
        }



      

        [HttpGet("users")]
        [Authorize]

        public async Task<ApiResponse> Filter([FromQuery] UserFilterInput filter)
        {
            //filter.UserId = UserId;
            //filter.IsAdmin = IsAdmin;
            var result = await _service.GetAllAsync(filter);
            return new ApiResponse(result);
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("{userId:int}")]
        [Authorize]

        public async Task<ApiResponse> Update([FromRoute] int userId, [FromForm] UpdateUserInput input)
        {

            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "Images", "UserAvatars");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(pathToSave);

                if (file.Length > 0)
                {
                    var fileName = file.FileName;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    await using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }
            await _service.UpdateAsync(userId, input);

            return new ApiResponse();
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromForm] CreateUserInput input)
        {
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "Images", "UserAvatars");

                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(pathToSave);

                if (file.Length > 0)
                {
                    var fileName = file.FileName;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    //input.Avatar = dbPath;
                }
            }
            await _service.InsertAsync(input);

            return new ApiResponse();
        }
        /// <summary>
        /// ویرایش پروفایل یک کاربر
        /// </summary>
        ///
        [Authorize]
        [HttpPut("change-state/{id}")]
        public async Task<ApiResponse> ChangeState([FromRoute] int id)
        {
          await  _service.ChangeState(id);
            return new ApiResponse();
        }

        /// <summary>
        /// دریافت اطلاعات یک کاربر
        /// </summary>
        ///
        [HttpGet("{userId:int}")]
        [Authorize]
        public async Task<ApiResponse> Get([FromRoute] int userId)
        {
            var result = await _service.GetById(userId);
            return new ApiResponse(result);
        }
        /// <summary> 
        /// اطلاعات پروفایل یک کاربر
        /// </summary>
        ///
        [Authorize]
        [HttpGet("profile")]
        public async Task<ApiResponse> Profile()
        {
            var result = await _service.GetById(UserId);
            return new ApiResponse(result);
        }
        /// <summary>
        /// ویرایش پروفایل یک کاربر
        /// </summary>
        ///
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<ApiResponse> Profile([FromForm] UpdateUserInput input)
        {
            return await Update(UserId, input);
        }

        /// <summary>
        /// ویرایش پروفایل یک کاربر
        /// </summary>
        ///
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ApiResponse> Delete([FromRoute] int id)
        {
            var user = await _service.GetById(id);
            await _service.Delete(id);
            return new ApiResponse();
        }
    }
}
