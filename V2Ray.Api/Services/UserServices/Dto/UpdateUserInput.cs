namespace V2Ray.Api.Services.UserServices.Dto
{
    public class UpdateUserInput : CreateUserInput
    {
        //public new string Password { get; set; }

        //public new string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Enable { get; set; }

        
        public new string? Password { get; set; }

        public new string? ConfirmPassword { get; set; }
        
    }
}