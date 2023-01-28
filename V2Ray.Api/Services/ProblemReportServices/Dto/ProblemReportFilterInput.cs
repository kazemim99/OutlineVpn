using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.ProblemReportServices.Dto
{
    public class ProblemReportFilterInput : PaginationModelInput
    {
        public string? CreatedAt { get; set; }
        public int? UserId { get;  set; }
    }

}