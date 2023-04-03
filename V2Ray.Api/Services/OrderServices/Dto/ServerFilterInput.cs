using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.OrderServices.Dto
{
    public class OrderFilterInput : PaginationModelInput
    {
        public string? KeyUserName { get; set; }
        public string? Mobile { get; set; }
    }

}