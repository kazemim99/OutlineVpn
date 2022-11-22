using Outline.Api.Entity;
using System.Collections.Generic;

namespace Outline.Api.Services.UserServices.Dto
{
    public class GetUserOutput : EntityDto<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsAdmin { get; set; }

        public string Mobile { get; set; }

        public string Avatar { get; set; }

        public string Phone { get; set; }

        public bool UserState { get; set; }

        public string[] ComplexRoles { get; set; }
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