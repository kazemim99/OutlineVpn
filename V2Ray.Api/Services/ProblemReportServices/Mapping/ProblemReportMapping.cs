using AutoMapper;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.ProblemReportServices.Dto;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.ProblemReportServices.Mapping
{
    public class ProblemReportMapping : Profile
    {
        public ProblemReportMapping()
        {
            CreateMap<ProblemReport, GetProblemReportListOutput>()
               .ForMember(a => a.Operator, c => c.MapFrom(b => b.Operator.GetDescription()))
               .ForMember(a => a.ReturnMoney, c => c.MapFrom(b => b.ReturnMoney ? "بازگشت وجه":""))
               .ForMember(a => a.OS, c => c.MapFrom(b => b.OS.GetDescription()))
               .ForMember(a => a.State, c => c.MapFrom(b => b.State.GetDescription()))
               .ForMember(a => a.UserName, c => c.MapFrom(b => b.User.Email));

            CreateMap<ProblemReport, GetProblemReportOutput>()
                .ForMember(a => a.Operator, c => c.MapFrom(b => b.Operator.GetDescription()))
               .ForMember(a => a.UserName, c => c.MapFrom(b => b.User.Email));

            CreateMap<CreateProblemReportInput, ProblemReport>()
                .ForMember(a => a.State, c => c.MapFrom(b => ProblemReportEnum.Sended));

            CreateMap<UpdateProblemReportInput, ProblemReport>();
        }

    }
}