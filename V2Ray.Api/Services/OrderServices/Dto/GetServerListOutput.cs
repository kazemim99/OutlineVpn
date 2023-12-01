using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;
using V2Ray.Api.Shared;

namespace V2Ray.Api.Services.OrderServices.Dto
{
    public class GetOrderListOutput
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Duration { get; set; }
        public string KeyUserName { get; set; }
        public string CreatedAt { get; set; }
        public int Amount { get; set; }
        public string Creator { get; set; }
  
        public IEnumerable<OptionItem> Statuses { get; set; }
        public object ExpireDate { get; internal set; }
    }
    public class OrdersCountOutput
    {
        public int ThreeMonthCount { get; set; }
        public int OneMonthCount { get; set; }
        public int TwoMonthCount { get; set; }
        public int UnknownCount { get;  set; }
    }
}