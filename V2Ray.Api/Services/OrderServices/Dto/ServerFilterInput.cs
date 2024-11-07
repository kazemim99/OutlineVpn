using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.OrderServices.Dto
{
    public class OrderFilterInput : PaginationModelInput
    {
        public int? UserId { get; set; }
        public string? From { get; set; }
        public string? To { get;  set; }
        public int? DurationId { get;  set; }
        public bool IsAdmin { get;  set; }

        public DateTime? FromGeo => From == null ? null : From.ToGeo();
        public DateTime? ToGeo => To == null ? null : To.ToGeo();
    }

}