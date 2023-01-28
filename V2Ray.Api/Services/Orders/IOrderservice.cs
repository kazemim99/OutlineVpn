using V2Ray.Api.Controllers;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.Orders.Dto;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.Orders
{
    public interface IOrderService : IBaseService<int,
        UpdateOrderInput,
        CreateOrderInput,
        GetOrderOutput,
        GetOrderListOutput,
        OrderFilterInput>
    {
        Task ChangeStatus(int id,string email, OrderStateEnum stateId);
    }
}