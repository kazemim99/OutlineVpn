using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using V2Ray.Api.Services.V2Keys;
using V2Ray.Api.Services.V2Keys.Dto;

namespace V2Ray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class V2KeyController : CustomBaseController
    {
        private readonly IV2KeyService _v2KeyService;

        public V2KeyController(IV2KeyService service)
        {
            _v2KeyService = service;
        }

        [HttpGet("filter")]
        [Authorize]
        public async Task<ApiResponse> Filter([FromQuery] V2KeyFilterInput filter)
        {
            //filter.V2KeyId = V2KeyId;
            //filter.IsAdmin = IsAdmin;
            filter.SortDesc = true;
            var result = await _v2KeyService.GetAllAsync(filter, new[] { "V2Server" });
            return new ApiResponse(result);
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("{id:int}")]
        [Authorize]

        public async Task<ApiResponse> Update([FromRoute] int id, [FromBody] UpdateV2KeyInput input)
        {
            await _v2KeyService.UpdateAsync(id, input);

            return new ApiResponse();
        }

        [HttpGet("generateKey/{count}")]
        [Authorize]

        public async Task<ApiResponse> GenerateKey([FromRoute] int count)
        {
            count = 1;
            //filter.UserId = UserId;
            //filter.IsAdmin = IsAdmin;
            await _v2KeyService.GenerateUserKey(count,UserId);
            return new ApiResponse();
        }
        [HttpGet("user-key-details")]
        [Authorize]
        public async Task<ApiResponse> UserKeyDetails()
        {
            //filter.UserId = UserId;
            //filter.IsAdmin = IsAdmin;
            var result = await _v2KeyService.UserKeyDetails(UserId);
            return new ApiResponse(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromBody] CreateV2KeyInput input)
        {
            input.UserId = UserId;
            await _v2KeyService.InsertAsync(input);
            return new ApiResponse();
        }
        [HttpPost("SwapServerKeys")]
        [Authorize]
        public async Task<ApiResponse> SwapServerKeys([FromBody] SwapServerKeysInput input)
        {
            await _v2KeyService.SwapServerKeysAsync(input);
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
           var result =   await _v2KeyService.GetById(id);

            return new ApiResponse(result);
        }

        


        /// <summary>
        /// ویرایش پروفایل یک کاربر
        /// </summary>
        ///
        [Authorize]
        [HttpDelete("{serverId}/{keyId}")]
        public async Task<ApiResponse> Delete([FromRoute] int serverId, [FromRoute] int keyId)
        {
            await _v2KeyService.DeleteKey(serverId,keyId);

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