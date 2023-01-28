using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;
using V2Ray.Api.Shared;

namespace V2Ray.Api.Services.Orders.Dto
{
    public class GetOrderListOutput
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string CreateAt { get; set; }
        public string CardNumber { get; set; }
        public string TranactionNumber { get; set; }
        public int StateId { get; set; }
        public string StatusString { get; set; }
        public List<OptionItem> Statuses { get; set; }
    }
}