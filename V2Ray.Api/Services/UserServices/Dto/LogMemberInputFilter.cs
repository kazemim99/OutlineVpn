using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.UserServices.Dto
{
    public class LogMemberInputFilter : PaginationModelInput
    {
        public int? DeviceId { get; set; }

        public int? ComplexId { get; set; }
    }
}