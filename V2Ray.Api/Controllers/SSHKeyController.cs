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
        [HttpGet("filter")]
        [Authorize]
        public async Task<ApiResponse> Filter([FromQuery] SSHKeyFilterInput filter)
        {
            //filter.SSHKeyId = SSHKeyId;
            //filter.IsAdmin = IsAdmin;~
            filter.SortDesc = true;
            var result = await _service.GetAllAsync(filter, new[] {"User"});
            return new ApiResponse(result);
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("{id:int}")]
        [Authorize]

        public async Task<ApiResponse> Update([FromRoute] int id, [FromBody] UpdateSSHKeyInput input)
        {
            await _service.UpdateAsync(id, input);

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
            var result =await _service.GetUserSSHKey(UserId);
            return new ApiResponse(result);
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
           var result =   await _service.GetById(id, new[] {"User"});

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
        //[HttpPost("DeleteKey")]
        //public async Task<ApiResponse> DeleteKey(CreateKeyInput input)
        //{

        //    var httpClient = GetCookie(input);
        //    var re = await httpClient.PostAsJsonAsync($"https://{input.Url}:{input.Port}/xui/inbound/del/{input.Id}", new { }); ;
        //    var ttt = await re.Content.ReadAsStringAsync();
        //    var root = JsonConvert.DeserializeObject<Root>(ttt);
        //    return new ApiResponse(root);
        //}
        //[HttpPost("GetKeys")]
        //public async Task<ApiResponse> GetKeys(CreateKeyInput input)
        //{

        //    var httpClient = GetCookie(input);
        //    var re = await httpClient.PostAsJsonAsync($"https://{input.Url}:{input.Port}/xui/inbound/list", new { }); ;
        //    var ttt = await re.Content.ReadAsStringAsync();
        //    var root = JsonConvert.DeserializeObject<Root>(ttt);
        //    return new ApiResponse(root);
        //}

      
      
    }
   
}