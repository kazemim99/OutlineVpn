namespace V2Ray.Api.Entity
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
}