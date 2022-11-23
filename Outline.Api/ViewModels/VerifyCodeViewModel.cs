using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Outline.Api.ViewModels
{

    public class HomePageViewModel
    {
        public double? ConsumedTraffic { get; set; }
        public double InitTraffic { get; set; }
        public double RaminingTraffic { get; set; }
    }
    public class CreateUserViewModel
    {
        public string PhoneNumber { get; set; }
    }
    public class VerifyCodeViewModel
    {
        public string Code { get; set; }

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