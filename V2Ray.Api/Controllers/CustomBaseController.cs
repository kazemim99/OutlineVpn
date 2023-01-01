using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace V2Ray.Api.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        public int UserId
        {
            get
            {
                return int.Parse(User.Claims.First(x => x.Type == "UserId").Value);
            }
        }

        public int FreeAcount
        {
            get
            {
                return int.Parse(User.Claims.First(x => x.Type == "FreeAcount").Value);
            }
        }


        public string Mobile
        {
            get
            {
                return User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            }
        }
        public bool IsAdmin
        {
            get
            {
                return bool.Parse(User.Claims.First(x => x.Type == "IsAdmin").Value);
            }
        }
        public string FullName
        {
            get
            {
                var result = User.Claims.First(x => x.Type == "fullName");
                return result.Value;
            }
        }

    }
}