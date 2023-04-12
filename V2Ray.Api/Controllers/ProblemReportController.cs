using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using V2Ray.Api.Services.ProblemReportServices.Dto;
using V2Ray.Api.Services.ProblemReportServices;

namespace V2Ray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProblemReportController : CustomBaseController
    {
        private readonly IProblemReportservice _service;

        public ProblemReportController(IProblemReportservice service)
        {
            _service = service;
        }


        [HttpGet]
        [Authorize]
        public async Task<ApiResponse> Filter([FromQuery] ProblemReportFilterInput filter)
        {
            if (!IsAdmin)
            {
                filter.UserId = UserId;
            }
            var result = await _service.GetAllAsync(filter, new[] { "User" });
            return new ApiResponse(result);
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("{id:int}")]
        [Authorize]

        public async Task<ApiResponse> Update([FromRoute] int id, [FromBody] UpdateProblemReportInput input)
        {
            await _service.UpdateAsync(id, input);

            return new ApiResponse();
        }
        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("sendAnswer/{id:int}")]
        [Authorize]

        public async Task<ApiResponse> SendAnswer([FromRoute] int id, [FromBody] SendAnswerInput model)
        {
            await _service.SendAnswerAsync(id, model);

            return new ApiResponse();
        }


        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromBody] CreateProblemReportInput input)
        {
            input.UserId = UserId;

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