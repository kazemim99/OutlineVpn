namespace V2Ray.Api.Services.SSHKeyServices.Dto
{
    public class GetSSHKeyListOutput
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ServerName { get; set; }
        public string Server { get; set; }
        public string Code { get; set; }
        public bool Enable { get; set; }
        public string Password { get; set; }
        public string ExpireDate { get; set; }
        public string CreatedAt { get; set; }
        public AccountType AccountType { get; set; }

        public string Name
        {
            get; set;
        }
        public string ChargeDate { get; set; }
    }
}
