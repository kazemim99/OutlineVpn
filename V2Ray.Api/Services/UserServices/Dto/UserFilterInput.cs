using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.UserServices.Dto
{
    public class UserFilterInput : PaginationModelInput
    {

        [DataMember(IsRequired = false)]
        public string? FirstName { get; set; }

        [DataMember(IsRequired = false)]
        public string? LastName { get; set; }

        [DataMember(IsRequired = false)]
        public bool? UserState { get; set; }

        [DataMember(IsRequired = false)]
        public int? UserId { get; set; }

        [DataMember(IsRequired = false)]
        public bool? IsAdmin { get; set; }

        [DataMember(IsRequired = false)]
        public string? Email { get;  set; }
    }

}