namespace V2Ray.Api.Entity
{
    public class Plan : FullAuditEntity<int>, ISoftDelete
    {
        public string Title { get; set; }
        public string? Descrption { get; set; }
        public int Price { get; set; }
        public int Period { get; set; }
        public bool PlanState { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }
        public int TrafficCapacity { get; set; }
    }
}