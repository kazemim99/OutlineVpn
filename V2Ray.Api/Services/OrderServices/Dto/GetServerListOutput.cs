using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;
using V2Ray.Api.Shared;

namespace V2Ray.Api.Services.OrderServices.Dto
{
    public class GetOrderListOutput
    {
        public int Id { get; set; }
        public string Status { get; set; }

        public string KeyUserName { get; set; }
        public string CreatedAt { get; set; }
        public int Amount { get; set; }
        public string Creator { get; set; }
        public IEnumerable<OptionItem> Statuses { get; set; }


    }
    public class OrdersCountOutput
    {
        public int WaitingCount { get; set; }
        public int AllCount { get; set; }
    }
}