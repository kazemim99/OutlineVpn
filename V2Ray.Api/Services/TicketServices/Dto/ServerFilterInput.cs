using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.MessageServices.Dto
{
    public class MessageFilterInput : PaginationModelInput
    {
        public string? UserName { get; set; }
        public bool Expired { get;  set; }
    }

}