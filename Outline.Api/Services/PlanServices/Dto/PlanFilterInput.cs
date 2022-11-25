using Newtonsoft.Json;
using Outline.Api.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Outline.Api.Services.UserServices.Dto
{
    public class PlanFilterInput : PaginationModelInput
    {
        [DataMember(IsRequired = false)]
        public string? Title { get; set; }
    }

}