using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using V2Ray.Api.Services.Orders.Dto;
using V2Ray.Api.Services.Orders;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : CustomBaseController
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet("filter")]
        [Authorize]
        public async Task<ApiResponse> Filter([FromQuery] OrderFilterInput filter)
        {
            if (!IsAdmin)
            {
                filter.UserId = UserId;
            }
            var result = await _service.GetAllAsync(filter, "User");
            return new ApiResponse(result);
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("{id:int}")]
        [Authorize]

        public async Task<ApiResponse> Update([FromRoute] int id, [FromBody] UpdateOrderInput input)
        {
            await _service.UpdateAsync(id, input);

            return new ApiResponse();
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("change-state/{id}/{emai}/{stateId:int}")]
        [Authorize]

        public async Task<ApiResponse> Update([FromRoute] int id,[FromRoute] string email, [FromRoute] OrderStateEnum stateId)
        {
            await _service.ChangeStatus(id,email, stateId);

            return new ApiResponse();
        }


        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromBody] CreateOrderInput input)
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