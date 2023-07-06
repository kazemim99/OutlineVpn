using System.Collections.Generic;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;
using V2Ray.Api.Shared;

namespace V2Ray.Api.Services.OrderServices.Dto
{
    public class GetOrderOutput : EntityDto<int>
    {

        public OrderStateEnum Status { get; set; }
        public string KeyUserName { get; set; }
        public string CreateAt { get; set; }
        public int Amount { get; set; }
        public string Creator { get; set; }
    }
    
}