using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.ProblemReports.Dto
{
    public class ProblemReportFilterInput : PaginationModelInput
    {
        public string? CreatedAt { get; set; }
    }

}