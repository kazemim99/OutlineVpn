using System.ComponentModel.DataAnnotations;

namespace Outline.Api.Services.OTP.DTO
{
    public class OtpVerifyIn
    {
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Code { get; set; }
    }
}