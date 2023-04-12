namespace V2Ray.Api.Services.Server.Dto
{
    public class GetServerListOutput
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string IP { get; set; }
        public string Url { get; set; }
        public bool HasLicense { get; set; }
        public int KeyCount { get; set; }
        public string TitleCount { get; set; }
    }
}