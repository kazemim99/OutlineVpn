namespace V2Ray.Api.Services.SSHKeys.Dto
{
    public class GetSSHKeyListOutput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ExpireDate { get; set; }
        public string CreatedAt { get; set; }
        public string Email
        {
            get; set;
        }
    }
}
