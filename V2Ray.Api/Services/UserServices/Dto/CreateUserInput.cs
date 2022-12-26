using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace V2Ray.Api.Services.UserServices.Dto
{
    public class CreateUserInput
    {
        [DataMember]
        [RegularExpression(@"\b[a-zA-Z0-9]{0,}([.]?[a-zA-Z0-9]{1,})[@](gmail.com|outlook.com|hotmail.com|yahoo.com)\b", ErrorMessage = "ایمیل وارد شده معتبر نیست")]
        public string Email { get; set; }

        [DataMember]
        [MinLength(8, ErrorMessage = "طول رمز عبور حداقل 8 کارکتر میباشد")]
        [MaxLength(32, ErrorMessage = "طول رمز عبور حداکثر 32 کارکتر میباشد")]
        public string Password { get; set; }

        [Compare("Password",ErrorMessage ="تکرار رمز عبور اشتباه است")]
        public string ConfirmPassword { get; set; }


        public bool IsAdmin { get; set; } = false;

        [DataMember(IsRequired =false)]
        public string? IP { get; set; }

        public bool Enable { get; set; }

    }
}