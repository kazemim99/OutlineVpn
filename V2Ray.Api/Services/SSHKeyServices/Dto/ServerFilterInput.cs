using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.SSHKeyServices.Dto
{
    public class SetPasswordModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class SSHKeyFilterInput : PaginationModelInput
    {
        public string? UserName { get; set; }
        public bool Expired { get;  set; }
        public string? Name { get; set; }

        public int? ServerId { get; set; }

    }

}