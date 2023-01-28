using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.Orders.Dto
{
    public class OrderFilterInput : PaginationModelInput
    {
        public string? Title { get; set; }
        public OrderStateEnum Status { get; set; }
        public int? UserId { get;  set; }
    }

}