using System.Collections.Generic;
using V2Ray.Api.Entity;

namespace V2Ray.Api.Services.PlanServices.Dto
{
    public class GetPlanOutput : EntityDto<int>
    {

        public string Title { get; set; }
        public string? Descrption { get; set; }

        public int Price { get; set; }
        public int Period { get; set; }
        public bool PlanState { get; set; }
        public int TrafficCapacity { get; set; }

        public string Image { get; set; }
    }
}