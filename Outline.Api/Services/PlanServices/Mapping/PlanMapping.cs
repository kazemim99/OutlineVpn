using AutoMapper;
using Outline.Api.Entity;
using Outline.Api.Services.UserServices.Dto;

namespace Outline.Api.Mapping
{
    public class PlanMapping : Profile
    {
        public PlanMapping()
        {
            CreateMap<Plan, GetPlanListOutput>()
                   .ForMember(a => a.Image,
                    c => c.MapFrom(d => GetImage(d)));

            CreateMap<Plan, GetPlanOutput>()
                   .ForMember(a => a.Image,
                    c => c.MapFrom(d => GetImage(d)));

            CreateMap<CreatePlanInput, Plan>();

            //CreateMap<AddComplexToPlan, ComplexPlan>();

            CreateMap<UpdatePlanInput, Plan>();
        }



        private string GetImage(Plan a)
        {
            if (a.Image == null) return "";
            return $"api/publicData/get-file/{a.Image.Replace('\\', '*')}";
        }
    }
}