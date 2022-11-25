using Outline.Api.Entity;
using Outline.Api.Services.UserServices.Dto;
using Outline.Api.Shared;

namespace Outline.Api.Services.PlanServices
{
    public interface IPlanService : IBaseService<int,
        UpdatePlanInput,
        CreatePlanInput,
        GetPlanOutput,
        GetPlanListOutput,
        PlanFilterInput>
    {

        Task IsDelete(int id, string fullName);

        //Task AddComplexToPlan(AddComplexToPlan input);

        Task ChangeState(int id, string fullName);
    }
}