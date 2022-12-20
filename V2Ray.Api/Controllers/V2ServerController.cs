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
using Twilio;
using Twilio.Rest.Api.V2010.Account;

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
            //filter.ServerId = ServerId;
            //filter.IsAdmin = IsAdmin;
            var result = await _service.GetAllAsync(filter, new[] { "City.Country"});
            return new ApiResponse(result);
        }  
        
        [HttpGet("all-servers")]
        [Authorize]
        public async Task<ApiResponse> AllServers()
        {
            //filter.ServerId = ServerId;
            //filter.IsAdmin = IsAdmin;
            var result = await _service.GetAllAsync(new ServerFilterInput
            {
            ItemsPerPage = 100
            });
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
        [Authorize]
        public async Task<ApiResponse> Get([FromRoute] int id)
        {

            //string accountSid = "AC11836c184b6e2ba4910f58a20e520546";
            //string authToken = "7610683dc85e912f82c2e5a1291f98c6";

            //TwilioClient.Init(accountSid, authToken);

            //var message = MessageResource.Create(
            //    body: "Hi there",
            //    from: new Twilio.Types.PhoneNumber("+14155552345"),
            //    to: new Twilio.Types.PhoneNumber("+989123135143")
            //);

            //Console.WriteLine(message.Sid);
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

        [HttpPost("Create_Bulk_Key")]
        public async Task<ApiResponse> CreateKey([FromBody] CreateBuldKeyInput input)
        {
            //var V2Keys =await _service.GetAllAsync(new V2KeyFilterInput
            //{
            //    ItemsPerPage = 99999
            //});

            await _service.CreateKey(input.Count, input.Customer);

            return new ApiResponse();

        }


    }



}