using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.OrderServices.Dto
{
    public class GetOrderListOutput
    {
        public string CardNumber { get; set; }
        public string Status { get; set; }
        public string Mobile { get; set; }

        public string KeyUserName { get; set; }

    }
}