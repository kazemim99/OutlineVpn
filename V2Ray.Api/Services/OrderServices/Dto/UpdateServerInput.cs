using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.OrderServices.Dto
{
    public class UpdateOrderInput : CreateOrderInput
    {
        public OrderStateEnum Status { get; set; }
    }
}