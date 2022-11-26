using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outline.Api.Database;
using Outline.Api.Services.sms;
using Outline.Api.Services.sms.Rahyab;
using Outline.Api.Services.UserServices;
using Outline.Api.Services.UserServices.Dto;
using OutlineVpn;

namespace Outline.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : CustomBaseController
    {
        const string URL_PRIFIX = "https://s3.amazonaws.com/outline-vpn/invite.html#";
        private readonly IUserService _service;
        private readonly IRahyabSmsSender _rahyabSmsSender;
        private readonly IOutlineApi _outline;
        public UserController(IUserService userService, IRahyabSmsSender rahyabSmsSender, IOutlineApi outline)
        {
            _service = userService;
            _rahyabSmsSender = rahyabSmsSender;
            _outline = outline;
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
                    input.Avatar = dbPath;
                }
            }
            input.CreatorFullName = FullName;
            var server = _service.UserServer(userId);

            if (server.Id == input.ServerId)
            {
                await _outline.SetUrl(input.ServerId);

                var keyId = _outline.GetKeys().FirstOrDefault(c => c.Name == input.Mobile);
                input.UserKeyId = keyId.Id;
                _outline.AddDataLimit(keyId.Id, Convert.ToInt64(input.InitCapacity));

            }
            else
            {
                await DelteKey(server.Id, input.Mobile);
                var capa = _outline.Capacity(input.Mobile);
                await _outline.SetUrl(input.ServerId);
                var output = _outline.CreateKey();
                input.UserKeyId = output.Id;
                _outline.RenameKey(output.Id, input.Mobile);
                var gig = Convert.ToDouble(capa);
                var bytecap = Convert.ToInt64(input.InitCapacity) - Convert.ToInt64(gig);
                var gigcap = Convert.ToInt64(bytecap * 1000d * 1000d * 1000d);
                _outline.AddDataLimit(output.Id, gigcap);
                await _rahyabSmsSender.SendAsync(new RahyabSendSmsReques
                {
                    destinationAddress = input.Mobile,
                    message = $"سرور اوت لاین  => {URL_PRIFIX}{output.AccessUrl}"
                });

                input.AccessUrl = output.AccessUrl;
            }

            await _service.UpdateAsync(userId, input);

            return new ApiResponse();
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromForm] CreateUserInput input)
        {
            await _outline.SetUrl(input.ServerId);
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
                    input.Avatar = dbPath;
                }
            }

            if (!input.IsAdmin)
            {
                var output = _outline.CreateKey();
                input.UserKeyId = output.Id;
                _outline.RenameKey(output.Id, input.Mobile);
                var gig = Convert.ToInt64(input.InitCapacity * 1000d * 1000d * 1000d);
                _outline.AddDataLimit(output.Id, gig);
                await _rahyabSmsSender.SendAsync(new RahyabSendSmsReques
                {
                    destinationAddress = input.Mobile,
                    message = $"سرور اوت لاین  => {URL_PRIFIX}{output.AccessUrl}"
                });

                input.AccessUrl = output.AccessUrl;

            }
            input.CreatorFullName = FullName;
            await _service.InsertAsync(input);

            return new ApiResponse();
        }


        [HttpGet("sendAccessKey/{id}")]
        public async Task<ApiResponse> SendAccessKey([FromRoute] int id)
        {
            var user = await _service.GetById(id);
            await _outline.SetUrl(user.ServerId);
            if (string.IsNullOrEmpty(user.AccessUrl))
            {
                var accessUrl = _outline.GetAccessUrl(user.Mobile);
                await _service.SetAccessKey(id, accessUrl);
            }
            await _rahyabSmsSender.SendAsync(new RahyabSendSmsReques
            {
                destinationAddress = user.Mobile,
                message = $"سرور اوت لاین  => {URL_PRIFIX}{user.AccessUrl}",
            });
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
            await _outline.SetUrl(result.ServerId);

            result.CunsumedTraffic = Convert.ToDouble(_outline.Capacity(result.Mobile));
         
            await _service.UpdateConsumedTraffic(result.CunsumedTraffic, userId);

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
            //result.RemainigCapacity = _outline.Capacity(result.Mobile);
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
        [HttpDelete("change-server/{id}/{url}")]
        public async Task<ApiResponse> ChangeServer([FromRoute] int id, [FromRoute] string url)
        {
            var user = await _service.GetById(id);
            await _outline.SetUrl(user.ServerId);

            var key = _outline.GetKeys().FirstOrDefault(a => a.Name == user.Mobile);
            if (key != null)
                _outline.DeleteKey(key.Id);

            await _service.Delete(id);
            return new ApiResponse();
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
            await DelteKey(user.ServerId, user.Mobile);

            await _service.Delete(id);
            return new ApiResponse();
        }

        private async Task DelteKey(int serverId, string mobile)
        {
            await _outline.SetUrl(serverId);

            var key = _outline.GetKeys().Where(a => a.Name == mobile);
            if (key.Any())
            {
                foreach (var item in key)
                {
                    _outline.DeleteKey(item.Id);
                } 
            }
        }

    }
}
