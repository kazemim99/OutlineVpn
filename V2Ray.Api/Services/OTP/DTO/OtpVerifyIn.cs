using System.ComponentModel.DataAnnotations;

namespace V2Ray.Api.Services.OTP.DTO
{
    public class OtpVerifyIn
    {
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Code { get; set; }
    }
}