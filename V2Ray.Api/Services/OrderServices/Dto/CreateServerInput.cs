using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace V2Ray.Api.Services.OrderServices.Dto
{
    public class CreateOrderInput
    {
        [JsonIgnore]
        public int UserId { get; set; }
        [Required]
        public string CardNumber { get; set; }
    }
}