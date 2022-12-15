using V2Ray.Api.Entity;
using V2Ray.Api.Services.PlanServices.Dto;

namespace V2Ray.Api.Services.PlanServices
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