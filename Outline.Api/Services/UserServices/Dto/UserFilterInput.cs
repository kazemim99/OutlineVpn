using Outline.Api.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Outline.Api.Services.UserServices.Dto
{
    public class UserFilterInput : PaginationModelInput
    {
        public string Mobile { get; set; }

        public int[] RoleId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool? UserState { get; set; }
        public int UserId { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class AddComplexToUser
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ComplexId { get; set; }
    }
}