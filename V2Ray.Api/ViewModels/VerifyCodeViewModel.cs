using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace V2Ray.Api.ViewModels
{

    public class HomePageViewModel
    {
        public double? ConsumedTraffic { get; set; }
        public double InitTraffic { get; set; }
        public double RaminingTraffic { get; set; }
    }
    public class CreateUserViewModel
    {
        public string Email { get; set; }
    }
    public class VerifyCodeViewModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        [RegularExpression(@"\b^(09|9)+([0-9]){9}$\b", ErrorMessage = "ایمیل وارد شده معتبر نیست")]
        public string Mobile { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}