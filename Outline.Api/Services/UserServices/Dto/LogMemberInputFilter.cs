using Outline.Api.Extensions;

namespace Outline.Api.Services.UserServices.Dto
{
    public class LogMemberInputFilter : PaginationModelInput
    {
        public int? DeviceId { get; set; }

        public int? ComplexId { get; set; }
    }
}