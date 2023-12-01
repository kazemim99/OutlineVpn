using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.OrderServices.Dto
{
    public class OrderFilterInput : PaginationModelInput
    {
        public int? UserId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get;  set; }
        public int? DurationId { get;  set; }
        public bool IsAdmin { get;  set; }
    }

}