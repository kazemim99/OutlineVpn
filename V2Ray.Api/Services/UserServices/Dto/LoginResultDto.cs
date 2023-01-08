using System.Collections.Generic;

namespace V2Ray.Api.Services.UserServices.Dto
{
    public class LoginResultDto:UserDetailDto
    {
        public JwtToken JwtToken { get; set; }

        public RefreshToken RefreshToken { get; set; }

      

        //public IEnumerable<string> Permissions { get; internal set; }
    }

    public  class UserDetailDto
    {
          public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public bool IsAdmin { get; set; }

    public int Id { get; set; }
    public bool FreeAccount { get; set; }
    public bool NeedConfirm { get; internal set; }
}
}