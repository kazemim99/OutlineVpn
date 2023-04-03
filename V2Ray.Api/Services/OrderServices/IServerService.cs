using V2Ray.Api.Controllers;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.OrderServices.Dto;
using static V2Ray.Api.Services.OrderServices.OrderService;

namespace V2Ray.Api.Services.OrderServices
{
    public interface IOrderService : IBaseService<int,
        UpdateOrderInput,
        CreateOrderInput,
        GetOrderOutput,
        GetOrderListOutput,
        OrderFilterInput>
    {
       
    }
}