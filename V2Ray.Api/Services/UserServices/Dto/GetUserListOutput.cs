namespace V2Ray.Api.Services.UserServices.Dto
{
    public class GetUserListOutput
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Code { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string[] Complexes { get; set; }

        public bool Enable { get; set; }
        public string FullName { get;  set; }
    }
}