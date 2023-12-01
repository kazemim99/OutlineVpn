using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace V2Ray.Api.Services.UserServices.Dto
{

    public class CustomerLoginDto
    {
        [DataMember]
        [Required]
        public string UserName { get; set; }

        public string Password { get; set; }
    }
    public class LoginDto
    {
        [DataMember]
        [Required]
        [RegularExpression(@"\b^(09|9)+([0-9]){9}$\b", ErrorMessage = "موا وارد شده معتبر نیست")]
        public string Mobile { get; set; }


        [MinLength(8, ErrorMessage = "طول رمز عبور حداقل 8 کارکتر میباشد")]
        [MaxLength(32, ErrorMessage = "طول رمز عبور حداکثر 32 کارکتر میباشد")]
        public string Password { get; set; }
    }
}


