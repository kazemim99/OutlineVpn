using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.Orders.Dto
{
    public class UpdateOrderInput : CreateOrderInput
    {
        public int Id { get; set; }
        public OrderStateEnum Status { get; set; }
    }
}