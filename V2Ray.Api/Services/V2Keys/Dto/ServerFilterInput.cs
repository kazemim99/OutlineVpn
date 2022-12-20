using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.V2Keys.Dto
{
    public class V2KeyFilterInput : PaginationModelInput
    {
        public string? Title { get; set; }
    }

}