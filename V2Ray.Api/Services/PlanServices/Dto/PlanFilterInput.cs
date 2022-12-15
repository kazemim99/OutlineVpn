using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.PlanServices.Dto
{
    public class PlanFilterInput : PaginationModelInput
    {
        [DataMember(IsRequired = false)]
        public string? Title { get; set; }
    }

}