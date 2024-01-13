namespace V2Ray.Api.Services.Server.Dto
{
    public class CustomerInfoOutput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string ExpireDate { get; set; }

    }
    public class GetServerListOutput
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string IP { get; set; }
        public string Url { get; set; }
        public bool Enable { get; set; }
        public int KeyCount { get; set; }
        public bool IsActive { get; set; }
        public bool HasLoadBalance { get; set; }
        public string TitleCount => $"{Title}({KeyCount})";
    }
}