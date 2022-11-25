using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Outline.Api.Services.UserServices.Dto
{
    public class CreateApiUrlInput 
    {

        [Required]
        public string Title { get; set; }

        [Required]
        public string? Url { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string IP { get; set; }

        public bool State { get; set; }


    }
}