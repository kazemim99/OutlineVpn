namespace V2Ray.Api.Shared
{
    public static class AppConst
    {
    }

    public static class AppErrors
    {
        public static string UserNotFound = "کاربری با این نام کاربری یافت نشد";

        public static string WrongPassword = "رمز عبور اشتباه است";

        public static string UserAlreadyExists = "کاربری با این ایمیل قبلا ثبت گردیده";

        public static string NotMatchPass = "تکرار رمز عبور اشتباه است";

        public static string UserDeactive = "کاربر غیر فعال است";

        public static string PlanNotFound = "پلن غیر فعال است";
        public static string ServerNotFound = "سرور یافت نشد";
        public static string CityNotFound = "شهر یافت نشد";
    }

    public static class DefaultUserConst
    {
        public const string Mobile = "09123135143";

        public const string Email = "kazemi.mst@gmail.com";

        public const string Phone = "88998899";

        public const string FirstName = "ادمین";

        public const string LastName = "ادمینی";

        public const string Avatar = "https://i.pravatar.cc/300";

        public const string Password = "1q2w3e4r";
    }
}