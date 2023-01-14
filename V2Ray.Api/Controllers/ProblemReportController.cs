using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using V2Ray.Api.Services.Cities.Dto;
using V2Ray.Api.Services.Cities;
using V2Ray.Api.Services.ProblemReports;
using V2Ray.Api.Services.ProblemReports.Dto;

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

        [HttpGet("all-cities")]
        [Authorize]
        public async Task<ApiResponse> Filter()
        {
            //filter.ProblemReportId = ProblemReportId;
            //filter.IsAdmin = IsAdmin;
            var result = await _service.GetAllAsync(new ProblemReportFilterInput
            {
                ItemsPerPage = 100
            });
            return new ApiResponse(result);
        }

        [HttpGet("Cities")]
        [Authorize]
        public async Task<ApiResponse> Filter([FromQuery] ProblemReportFilterInput filter)
        {
            //filter.ProblemReportId = ProblemReportId;
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

        public async Task<ApiResponse> Update([FromRoute] int id, [FromBody] UpdateProblemReportInput input)
        {
            await _service.UpdateAsync(id, input);

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
           var result =   await _service.GetById(id);

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