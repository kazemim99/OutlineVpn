using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.SSHKeys.Dto
{
    public class SSHKeyFilterInput : PaginationModelInput
    {
        public string? Email { get; set; }
    }

}