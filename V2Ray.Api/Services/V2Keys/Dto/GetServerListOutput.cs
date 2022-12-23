namespace V2Ray.Api.Services.V2Keys.Dto
{
    public class GetV2KeyListOutput
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string User { get; set; }
        public long PrimaryCapacity { get; set; }
        public long UsedCapacity { get; set; }
        public string ExpireDate { get; set; }
        public bool State { get; set; }
        public string ClientId { get;  set; }
    }
}