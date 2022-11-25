using Outline.Api.Entity;
using System.Collections.Generic;

namespace Outline.Api.Services.UserServices.Dto
{
    public class GetApiUrlOutput : EntityDto<int>
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public string IP { get; set; }
        public bool State { get; set; }
        public string? Url { get; set; }


    }
}