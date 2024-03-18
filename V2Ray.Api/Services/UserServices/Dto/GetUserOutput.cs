using System.Collections.Generic;
using V2Ray.Api.Entity;

namespace V2Ray.Api.Services.UserServices.Dto
{
    public class GetUserOutput : EntityDto<int>
    {
        public int ServerId { get; set; }

        public string FirstName { get; set; }

        public string AccessUrl { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsAdmin { get; set; }

        public string Mobile { get; set; }

        public string Avatar { get; set; }

        public string Phone { get; set; }

        public bool Enable { get; set; }

        public string[] ComplexRoles { get; set; }
        public double CunsumedTraffic { get; internal set; }
        public double InitCapacity { get; set; }
        public bool NeedConfirm { get;  set; }
        public bool FreeAccount { get;  set; }
    }

    public class UserRoleComplexesOutput
    {
        public string ComplexName { get; set; }

        public List<ComplexRoleOutName> Roles { get; set; }
    }

    public class ComplexRoleOutName
    {
        public string RoleName { get; set; }
    }
}