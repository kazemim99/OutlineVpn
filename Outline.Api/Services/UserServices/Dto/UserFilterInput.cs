using Newtonsoft.Json;
using Outline.Api.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Outline.Api.Services.UserServices.Dto
{
    public class UserFilterInput : PaginationModelInput
    {
        [DataMember(IsRequired = false)]
        public string? Mobile { get; set; }

       

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
    }

}