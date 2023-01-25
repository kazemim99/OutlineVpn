using V2Ray.Api.Controllers;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.Orders.Dto;

namespace V2Ray.Api.Services.Orders
{
    public interface IOrderservice : IBaseService<int,
        UpdateOrderInput,
        CreateOrderInput,
        GetOrderOutput,
        GetOrderListOutput,
        OrderFilterInput>
    {
    }
}