using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using V2Ray.Api.Services.OrderServices.Dto;
using V2Ray.Api.Services.sms;
using V2Ray.Api.Services.OrderServices;

namespace V2Ray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : CustomBaseController
    {
        private readonly IOrderService _service;
        public OrderController(IOrderService OrderService, IRahyabSmsSender rahyabSmsSender)
        {
            _service = OrderService;
        }


        [HttpGet("Orders")]
        [Authorize]

        public async Task<ApiResponse> Filter([FromQuery] OrderFilterInput filter)
        {
            //filter.OrderId = OrderId;
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

        public async Task<ApiResponse> Update([FromRoute] int id, [FromForm] UpdateOrderInput input)
        {
            await _service.UpdateAsync(id,input);

            return new ApiResponse();
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromForm] CreateOrderInput input)
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
            await _service.Delete(id);
            return new ApiResponse();
        }


    }
}
