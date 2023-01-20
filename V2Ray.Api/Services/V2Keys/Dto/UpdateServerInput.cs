namespace V2Ray.Api.Services.V2Keys.Dto
{
    public class UpdateV2KeyInput : CreateV2KeyInput
    {
    }

    public class UserKeyDetailsOutput
    {
        public bool FreeAccount { get; set; }
        public string ExpireTime { get; set; }
        public long Up { get; set; }
        public long Down { get; set; }
        public string Key { get;  set; }
        public string ClientKeyId { get;  set; }
        public int Traffic { get;  set; }
    }
    public class GenerateSSHOutput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}