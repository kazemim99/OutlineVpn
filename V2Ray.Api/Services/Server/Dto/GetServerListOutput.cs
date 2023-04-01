namespace V2Ray.Api.Services.Server.Dto
{
    public class GetServerListOutput
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string IP { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public object KeyCount { get;  set; }
    }
}