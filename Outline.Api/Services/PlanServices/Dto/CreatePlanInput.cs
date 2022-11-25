using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Outline.Api.Services.UserServices.Dto
{
    public class CreatePlanInput 
    {
        [Required]

        public string Title { get; set; }

        [DataMember(IsRequired = false)]

        public string? Descrption { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Period { get; set; }
        public bool PlanState { get; set; }

        [DataMember(IsRequired = false)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string? Image { get; set; }

        public int TrafficCapacity { get; set; }
    }
}