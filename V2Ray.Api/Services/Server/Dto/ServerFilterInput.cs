using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.Server.Dto
{
    public class ServerFilterInput : PaginationModelInput
    {
        public string? Title { get; set; }
        public int UserId { get; set; }
        public bool IsAdmin { get; set; }
        public bool? Enable { get; internal set; }
    }

}