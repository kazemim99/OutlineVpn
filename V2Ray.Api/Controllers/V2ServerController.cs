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
    public class V2ServerController : CustomBaseController
    {
        private readonly IServerService _service;

        public V2ServerController(IServerService service)
        {
            _service = service;
        }

        [HttpGet("filter")]
        [Authorize]
        public async Task<ApiResponse> Filter([FromQuery] ServerFilterInput filter)
        {
            filter.UserId = UserId;
            filter.IsAdmin = IsAdmin;
            var result = await _service.GetAllAsync(filter, new[] { "SSHKeys" });

            return new ApiResponse(result);
        }

        [HttpGet("all-servers")]
        [Authorize]
        public async Task<ApiResponse> AllServers()
        {

            var result = await _service.GetAllAsync(new ServerFilterInput
            {
                UserId = UserId,
                IsAdmin = IsAdmin,
                ItemsPerPage = 100
            }, new[] { "SSHKeys" });
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
            if (!IsAdmin)
                input.UserId = UserId;

            await _service.InsertAsync(input);
            return new ApiResponse();
        }

        /// <summary>
        /// دریافت اطلاعات یک کاربر
        /// </summary>
        ///
        [HttpPut("change-state/{id:int}")]
        [Authorize]
        public async Task<ApiResponse> ChageState([FromRoute] int id)
        {
            await _service.ChangeState(id);

            return new ApiResponse();
        }

        /// <summary>
        /// دریافت اطلاعات یک کاربر
        /// </summary>
        ///
        [HttpGet("{id:int}")]
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
            await _service.Delete(id);

            return new ApiResponse();
        }

    }



}