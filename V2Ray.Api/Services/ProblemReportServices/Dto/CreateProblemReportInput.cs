using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.ProblemReportServices.Dto
{
    public class CreateProblemReportInput : IValidatableObject
    {
        [JsonIgnore]
        public int UserId { get; set; }

        [Required]

        public OperatorEnum Operator { get; set; }
        [Required]
        public OSEnum OS { get; set; }

        [StringLength(300, ErrorMessage = "حداکثر طول توضیحات 300 کارکتر میباشد", MinimumLength = 0)]
        public string? Despriction { get; set; }
        public bool ReturnMoney { get;  set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // some other random test
            if (OS <= 0)
            {
                results.Add(new ValidationResult("سیستم عامل را وارد نمایید"));
            }

            if (Operator <= 0)
            {
                results.Add(new ValidationResult("اپراتور را وارد نمایید    "));
            }
            return results;
        }
    }
}