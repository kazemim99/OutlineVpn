using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Outline.Api.Controllers
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

        public int DeviceId
        {
            get
            {
                return int.Parse(User.Claims.First(x => x.Type.ToLower() == "DeviceId".ToLower()).Value);
            }
        }
    }
}