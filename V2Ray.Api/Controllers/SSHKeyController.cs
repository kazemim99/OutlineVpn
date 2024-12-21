using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using V2Ray.Api.Services.SSHKeyServices;
using V2Ray.Api.Services.SSHKeyServices.Dto;

namespace V2Ray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SSHKeyController : CustomBaseController
    {
        private readonly ISSHKeyService _service;

        public SSHKeyController(ISSHKeyService service)
        {
            _service = service;
        }



       


        [HttpGet("all-sshkeys")]
        [Authorize]
        public async Task<ApiResponse> Filter()
        {
            try
            {
                var result = await _service.GetAllAsync(new SSHKeyFilterInput
                {
                    ItemsPerPage = 100,
                    IsAdmin = IsAdmin,
                    UserId = UserId
                });
                return new ApiResponse(result);
            }
            catch (Exception ex)
            {

                throw;
            }
            //filter.SSHKeyId = SSHKeyId;
            //filter.IsAdmin = IsAdmin;

        }


        [HttpPost("set-user")]
        [Authorize]
        public async Task<ApiResponse> SetUser([FromBody] SetPasswordModel model)
        {

            await _service.SetUser(UserId, model);
            return new ApiResponse();
        }

        [HttpGet("delete-expired")]
        [Authorize]
        public async Task<ApiResponse> DeleteExpired()
        {
            //filter.SSHKeyId = SSHKeyId;
            //filter.IsAdmin = IsAdmin;
            var result = await _service.GetAllAsync(new SSHKeyFilterInput
            {
                Expired = true
            });
            return new ApiResponse(result);
        }
        [HttpGet("filter")]
        [Authorize]
        public async Task<ApiResponse> Filter([FromQuery] SSHKeyFilterInput filter)
        {
            try
            {

                filter.UserId = UserId;
                filter.IsAdmin = IsAdmin;
                filter.SortDesc = true;
                var result = await _service.GetAllAsync(filter);
                return new ApiResponse(result);
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("{id:int}")]
        [Authorize]

        public async Task<ApiResponse> Update([FromRoute] int id, [FromBody] UpdateSSHKeyInput input)
        {
            input.UserId = UserId;
            input.IsAdmin = IsAdmin;

            await _service.UpdateAsync(id, input);
            return new ApiResponse();
        }


        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("recreate/{name}")]

        public async Task<ApiResponse> Recreate([FromRoute] string name)
        {
            //await _service.Recreate(name);
            return new ApiResponse();
        }



        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("adjust")]

        public async Task<ApiResponse> Adjust()
        {
            await _service.Adjust();
            return new ApiResponse();
        }

        [HttpPut("adjustV2")]

        public async Task<ApiResponse> AdjustV2()
        {
            await _service.AdjustV2();
            return new ApiResponse();
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("change-state/{id:int}")]
        [Authorize]

        public async Task<ApiResponse> ChangeState([FromRoute] int id)
        {
            await _service.ChangeState(id,UserId);
            return new ApiResponse();
        }



        //[HttpGet("create-test-ssh")]
        //[Authorize]
        //public async Task<ApiResponse> Create()
        //{
        //    await _service.GenerateSshFromClient(UserId);
        //    return new ApiResponse();
        //}

        [HttpGet("user-key-details")]
        [Authorize]
        public async Task<ApiResponse> KeyDetails()
        {
            var details = await _service.GetKeyDetails(UserId);
            return new ApiResponse(details);
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromBody] CreateSSHKeyInput input)
        {
            input.UserId = UserId;
            input.IsAdmin = IsAdmin;
            input.Name = FullName;
            await _service.GenerateSshFromAdmin(input);
            return new ApiResponse();
        }
        

        /// <summary>
        /// دریافت اطلاعات یک کاربر
        /// </summary>
        ///
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ApiResponse> Get([FromRoute] int id)
        {
            var result = await _service.GetById(id, new[] { "Orders" });

            return new ApiResponse(result);
        }


        [Authorize]
        [HttpPost("change-password/{id}")]
        public async Task<ApiResponse> ChangePassowrd([FromRoute] int id)
        {
            await _service.ChangePassowrd(id);
            return new ApiResponse();
        }


        /// <summary>
        /// ویرایش پروفایل یک کاربر
        /// </summary>
        ///
        [Authorize]
        [HttpPost("charge/{id}/{durationId}")]
        public async Task<ApiResponse> Charge([FromRoute] int id, [FromRoute] int durationId)
        {
            await _service.Charge(id, durationId, UserId);
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
            await _service.Delete(id);
            return new ApiResponse();
        }

    }

}