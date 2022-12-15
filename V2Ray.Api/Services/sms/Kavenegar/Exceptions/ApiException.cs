using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.sms.Kavenegar.Exceptions
{
    public class ApiException : KavenegarException
    {
        private readonly MetaCode _result;

        public ApiException(string message, int code)
         : base(message)
        {
            _result = (MetaCode)code;
        }

        public MetaCode Code
        {
            get { return _result; }
        }
    }
}