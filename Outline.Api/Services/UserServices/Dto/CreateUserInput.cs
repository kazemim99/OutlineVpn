using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Outline.Api.Services.UserServices.Dto
{
    public class CreateUserInput 
    {

        public List<int> Servers { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        //public string Code { get; set; }
        [DataMember(IsRequired = false)]
        public string? Email { get; set; }

        [Required]
        public string Mobile { get; set; }

        //public string Phone { get; set; }

        public bool IsAdmin { get; set; }

        //[Required]
        //public string Password { get; set; }


        //[Required]
        //[Compare("Password")]
        //public string ConfirmPassword { get; set; }
        [DataMember(IsRequired = false)]

        [System.Text.Json.Serialization.JsonIgnore]
        public string? Avatar { get; set; }

        public bool UserState { get; set; }

        [DataMember(IsRequired =false)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string? CreatorFullName { get; set; } 
        
        [DataMember(IsRequired =false)]
        [System.Text.Json.Serialization.JsonIgnore]
        public int UserKeyId { get; set; }

        [DataMember(IsRequired = false)]
        [System.Text.Json.Serialization.JsonIgnore]
        public string? AccessUrl { get;  set; }
        
        public double InitCapacity { get;  set; }
    }
}