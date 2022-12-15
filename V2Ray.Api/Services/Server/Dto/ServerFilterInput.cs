using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.Server.Dto
{
    public class ServerFilterInput : PaginationModelInput
    {
        public string? Title { get; set; }
    }

}