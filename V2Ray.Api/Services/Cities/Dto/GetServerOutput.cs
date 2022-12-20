using System.Collections.Generic;
using V2Ray.Api.Entity;

namespace V2Ray.Api.Services.Cities.Dto
{
    public class GetCityOutput : EntityDto<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}