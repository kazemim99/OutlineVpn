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
        public bool Enable { get; set; }

        public bool IsActive { get; set; }

        public string Token { get; set; }

        public bool HasLoadBalance { get; set; }

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

        public int? UserId { get; set; }
    }
}