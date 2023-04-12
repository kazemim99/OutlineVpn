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
            //filter.SSHKeyId = SSHKeyId;
            //filter.IsAdmin = IsAdmin;
            var result = await _service.GetAllAsync(new SSHKeyFilterInput
            {
                ItemsPerPage = 100
            });
            return new ApiResponse(result);
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
                filter.SortDesc = true;
                var result = await _service.GetAllAsync(filter, new[] { "V2Server" });
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
            await _service.Recreate(name);

            return new ApiResponse();
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("swap")]

        public async Task<ApiResponse> Recreate()
        {
            await _service.Swapp();

            return new ApiResponse();
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("swap2/{url}")]

        public async Task<ApiResponse> Recreate2([FromRoute] string url)
        {
            await _service.Swapp2(url);

            return new ApiResponse();
        }



        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("adjust/{serverId:int}")]
        [Authorize]

        public async Task<ApiResponse> Adjust([FromRoute] int serverId)
        {
            await _service.Adjust(serverId);

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
            await _service.ChangeState(id);

            return new ApiResponse();
        }



        [HttpGet("create-test-ssh")]
        [Authorize]
        public async Task<ApiResponse> Create()
        {
            await _service.GenerateSshFromClient(UserId);
            return new ApiResponse();
        }

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
            var result = await _service.GetById(id, new[] { "V2Server", "Orders" });

            return new ApiResponse(result);
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