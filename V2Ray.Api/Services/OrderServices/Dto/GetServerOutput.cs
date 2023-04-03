using System.Collections.Generic;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.OrderServices.Dto
{
    public class GetOrderOutput : EntityDto<int>
    {

        public string CardNumber { get; set; }
        public OrderStateEnum Status { get; set; }
        public string Mobile { get; set; }

        public string KeyUserName { get; set; }
    }
}