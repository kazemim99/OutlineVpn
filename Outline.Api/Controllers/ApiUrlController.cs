using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outline.Api.Database;
using Outline.Api.Services.sms;
using Outline.Api.Services.sms.Rahyab;
using Outline.Api.Services.ApiUrlServices;
using OutlineVpn;
using Outline.Api.Services.UserServices.Dto;
using Outline.Api.Services.ApiUrlServices;

namespace Outline.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiUrlController : CustomBaseController
    {
        private readonly IApiUrlService _service;
        private readonly IOutlineApi _outline;

        public ApiUrlController(IApiUrlService service, IOutlineApi outline)
        {
            _service = service;
            _outline = outline;
        }

        [HttpGet("ApiUrls")]
        [Authorize]
        public async Task<ApiResponse> Filter([FromQuery] ApiUrlFilterInput filter)
        {
            //filter.ApiUrlId = ApiUrlId;
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

        public async Task<ApiResponse> Update([FromRoute] int id, [FromBody] UpdateApiUrlInput input)
        {
            await _service.UpdateAsync(id, input);

            return new ApiResponse();
        }


        [HttpGet("setApiUrl/{id}")]
        public async Task<ApiResponse> SetApiUrl([FromRoute] int id)
        {
            await _outline.SetUrl(id);
            return new ApiResponse();
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromBody]CreateApiUrlInput input)
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


    }
}
