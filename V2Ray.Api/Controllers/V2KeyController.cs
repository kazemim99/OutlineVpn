using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using V2Ray.Api.Services.Server.Dto;
using V2Ray.Api.Services.Server;

namespace V2Ray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class V2KeyController : CustomBaseController
    {
        private readonly IServerService _service;

        public V2KeyController(IServerService service)
        {
            _service = service;
        }

        [HttpGet("Servers")]
        [Authorize]
        public async Task<ApiResponse> Filter([FromQuery] ServerFilterInput filter)
        {
            //filter.ServerId = ServerId;
            //filter.IsAdmin = IsAdmin;
            var result = await _service.GetAllAsync(filter);
            return new ApiResponse(result);
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("{id:int}")]
        [Authorize]

        public async Task<ApiResponse> Update([FromRoute] int id, [FromBody] UpdateServerInput input)
        {
            await _service.UpdateAsync(id, input);

            return new ApiResponse();
        }


        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromBody] CreateServerInput input)
        {
            await _service.InsertAsync(input);
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
            var result = await _service.GetById(id);

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
            await _service.SoftDelete(id);

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

        [HttpPost("Create_Key")]
        public async Task<ApiResponse> CreateKey([FromBody] CreateKeyInput input)
        {
            //var servers =await _service.GetAllAsync(new ServerFilterInput
            //{
            //    ItemsPerPage = 99999
            //});

          await  _service.CreateKey(input.Count,input.Customer);

            return new ApiResponse();

        }
      
    }
   
}