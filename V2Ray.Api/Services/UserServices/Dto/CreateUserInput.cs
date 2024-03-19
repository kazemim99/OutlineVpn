using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace V2Ray.Api.Services.UserServices.Dto
{
    public class GetCodeInput
    {
        public string Mobile { get; set; }
        public string LoginToken { get; set; }

    }
    public class CreateUserInput
    {
        [DataMember]
        [Required]
        [RegularExpression(@"\b^(09|9)+([0-9]){9}$\b", ErrorMessage = "موبایل وارد شده معتبر نیست")]
        public string Mobile { get; set; }

        [DataMember]
        [MinLength(8, ErrorMessage = "طول رمز عبور حداقل 8 کارکتر میباشد")]
        [MaxLength(32, ErrorMessage = "طول رمز عبور حداکثر 32 کارکتر میباشد")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "تکرار رمز عبور اشتباه است")]
        public string ConfirmPassword { get; set; }


        public bool IsAdmin { get; set; } = false;

        //[DataMember(IsRequired =false)]
        //public string? IP { get; set; }

        public bool Enable { get; set; }


        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int? AccountLimit { get; set; }

        public int? OneAccountPrice { get; set; }
        public int? TwoAccountPrice { get; set; }
        public int? ThreeAccountPrice { get; set; }


        //[Required]
        //public string LoginToken { get; set; }

    }
}