using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace V2Ray.Api.Services.Orders.Dto
{
    public class CreateOrderInput
    {
        [JsonIgnore]
        public int UserId { get; set; }
        [JsonIgnore]
        public int Amount { get; set; } = 50000;
        [Required]
        public string CardNumber { get; set; }

        [Required]
        [StringLength(maximumLength:20,ErrorMessage ="طول شماره تراکنش نامعتبر است ",MinimumLength =5)]
        public string TranactionNumber { get; set; }
    }
}