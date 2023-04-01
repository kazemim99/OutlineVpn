using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace V2Ray.Api.Services.TicketServices.Dto
{
    public class CreateMessageInput
    {
        public string Text { get; set; }
        [JsonIgnore]
        public int SenderId { get; set; }
        public string Attachment { get; set; }
        public int TicketId { get; set; }
        public bool IsAdmin { get; set; }
    }
}