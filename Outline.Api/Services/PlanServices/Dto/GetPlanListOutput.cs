namespace Outline.Api.Services.UserServices.Dto
{
    public class GetPlanListOutput
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int Price { get; set; }
        public string Image { get; set; }

        public int Period { get; set; }
        public bool PlanState { get; set; }
        public int TrafficCapacity { get; set; }

    }
}