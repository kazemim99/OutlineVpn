using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace V2Ray.Api.Services.Server.Dto
{
    public class CreateServerInput : IValidatableObject
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public bool Swapped { get; set; }

        [Required]
        public List<string> IPs { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int Port { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public int CityId { get; set; }


        public bool IsActive { get; set; }

        public bool IsMain { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // some other random test
            if (this.CityId <= 0)
            {
                results.Add(new ValidationResult("شناسه شهر نباید صفر باشد"));
            }

            if (this.Port <= 0)
            {
                results.Add(new ValidationResult("پورت باید بزرگتر 100 باشید"));
            }
            return results;
        }
    }
}