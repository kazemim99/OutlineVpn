using System.Collections.Generic;
using V2Ray.Api.Entity;

namespace V2Ray.Api.Services.TicketServices.Dto
{
    public class GetMessageOutput : EntityDto<int>
    {
        public string Text { get; set; }
        public string Sender { get; set; }
        public string CreateAt { get; set; }
        public List<string> Attachments { get; set; }
    }
}