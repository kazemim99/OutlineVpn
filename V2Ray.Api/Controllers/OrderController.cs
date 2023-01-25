using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using V2Ray.Api.Services.Orders.Dto;
using V2Ray.Api.Services.Orders;

namespace V2Ray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : CustomBaseController
    {
        private readonly IOrderservice _service;

        public OrderController(IOrderservice service)
        {
            _service = service;
        }

        [HttpGet("all-orders")]
        [Authorize]
        public async Task<ApiResponse> Filter()
        {
            var result = await _service.GetAllAsync(new OrderFilterInput
            {
                ItemsPerPage = 100
            });
            return new ApiResponse(result);
        }

        [HttpGet("orders")]
        [Authorize]
        public async Task<ApiResponse> Filter([FromQuery] OrderFilterInput filter)
        {
            var result = await _service.GetAllAsync(filter);
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


        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromBody] CreateOrderInput input)
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