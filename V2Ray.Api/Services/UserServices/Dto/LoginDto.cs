using System.ComponentModel.DataAnnotations;

namespace V2Ray.Api.Services.UserServices.Dto
{
    public class LoginDto
    {
        [RegularExpression(@"\b[a-zA-Z0-9]{0,}([.]?[a-zA-Z0-9]{1,})[@](gmail.com|outlook.com|hotmail.com|yahoo.com)\b", ErrorMessage ="ایمیل وارد شده معتبر نیست")]
        public string Email { get; set; }

        [MinLength(8, ErrorMessage = "طول رمز عبور حداقل 8 کارکتر میباشد")]
        [MaxLength(32, ErrorMessage = "طول رمز عبور حداکثر 32 کارکتر میباشد")]
        public string Password { get; set; }

        //[Required]
        //public string LoginToken { get; set; }
    }
}


