using System.Collections.Generic;

namespace Outline.Api.Entity
{

    public class FailedSms : AuditEntity<int>
    {
        public string DestinationAddress { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string Company { get; set; }
        public string Number { get; set; }
        public string MessageId { get; set; }
        public string ExceptionMessage { get; set; }
        public bool Sent
        {
            get; set;

        }
    }

    public class User : FullAuditEntity<int>, ISoftDelete
    {
        public User()
        {

        }
        public int? UserKeyId { get; set; }
        public bool UsedTest { get; set; }
        public double CunsumedTraffic { get; set; }
        public double InitCapacity { get; set; }
        public string FirstName { get; set; }
        public string AccessUrl { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string Mobile { get; set; }
        public string? Avatar { get; set; }

        //public string Password { get; set; }

        public bool IsAdmin { get; set; }
        public bool UserState { get; set; }
        public bool IsDeleted { get; set; }
        public ApiUrl Server { get; set; }
        public int? ServerId { get; set; }
        public ICollection<UserRole> Roles { get; set; }

    }
}