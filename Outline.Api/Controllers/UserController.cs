using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outline.Api.Database;
using Outline.Api.Services.UserServices;
using Outline.Api.Services.UserServices.Dto;

namespace Outline.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController :  CustomBaseController
    {

        private readonly IUserService _service;

        public UserController(IUserService userService)
        {
            _service = userService;
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

        [HttpPost]
        [Authorize]

        public async Task<ApiResponse> Create([FromForm] CreateUserInput input)
        {
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("UserAvatars");
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
                    input.Avatar = dbPath;
                }
            }
            input.CreatorFullName = FullName;
            await _service.InsertAsync(input);
            return new ApiResponse();
        }


        /// <summary>
        /// ویرایش یک کاربر جدید
        /// </summary>
        ///
        [HttpPut("{userId:int}")]
        [Authorize]

        public async Task<ApiResponse> Update([FromRoute] int userId, [FromForm] UpdateUserInput input)
        {
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine( "UserAvatars");
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
                    input.Avatar = dbPath;
                }
            }
            input.CreatorFullName = FullName;

            await _service.UpdateAsync(userId, input);
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
            var include = new[] { "Complexes" };
            var result = await _service.GetById(userId, include);
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
        
    
    }
}
