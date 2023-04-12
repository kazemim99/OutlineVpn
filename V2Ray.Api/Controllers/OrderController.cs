using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using V2Ray.Api.Services.OrderServices.Dto;
using V2Ray.Api.Services.sms;
using V2Ray.Api.Services.OrderServices;
using V2Ray.Api.Services.SSHKeyServices;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : CustomBaseController
    {
        private readonly IOrderService _service;
        private readonly ISSHKeyService _iSSHKeyService;
        public OrderController(IOrderService OrderService, IRahyabSmsSender rahyabSmsSender, ISSHKeyService iSSHKeyService)
        {
            _service = OrderService;
            _iSSHKeyService = iSSHKeyService;
        }


        [HttpGet("filter")]
        [Authorize]

        public async Task<ApiResponse> Filter([FromQuery] OrderFilterInput filter)
        {
            //filter.OrderId = OrderId;
            //filter.IsAdmin = IsAdmin;
            
            var result = await _service.GetAllAsync(filter, new[] { "SSHKey", "User" });
            return new ApiResponse(result);
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("{id:int}")]
        [Authorize]

        public async Task<ApiResponse> Update([FromRoute] int id, [FromForm] UpdateOrderInput input)
        {
            await _service.UpdateAsync(id, input);

            return new ApiResponse();
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromBody] CreateOrderInput input)
        {
            input.UserId = input.UserId;

            await _service.InsertAsync(input);
            await _iSSHKeyService.GenerateSshFromClient(UserId);

            return new ApiResponse();
        }


        [HttpPut("change-state/{id}/{stateId}")]
        [Authorize]
        public async Task<ApiResponse> ChangeState([FromRoute] int id,[FromRoute] OrderStateEnum stateId)
        {
            await _service.ChangeState(id,stateId);

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
            await _service.Delete(id);
            return new ApiResponse();
        }


    }
}
