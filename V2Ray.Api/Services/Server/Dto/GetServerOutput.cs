using System.Collections.Generic;
using V2Ray.Api.Entity;

namespace V2Ray.Api.Services.Server.Dto
{
    public class GetServerOutput : EntityDto<int>
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public int CityId { get; set; }
        public string IP { get; set; }
        public bool IsActive { get; set; }
        public string? Url { get; set; }


    }
}