using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace V2Ray.Api.Services.Server.Dto
{
    public class CreateServerInput 
    {
        [Required]
        public string UserName { get; set; }
        public int Capacity { get; set; }


        [Required]
        public string Password { get; set; }

        [Required]
        public int Port { get; set; }

        [Required]
        public string IP { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Url { get; set; }
    }
}