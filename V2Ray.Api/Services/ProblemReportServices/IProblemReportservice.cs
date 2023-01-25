using V2Ray.Api.Controllers;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.ProblemReportServices.Dto;

namespace V2Ray.Api.Services.ProblemReportServices
{
    public interface IProblemReportservice : IBaseService<int,
        UpdateProblemReportInput,
        CreateProblemReportInput,
        GetProblemReportOutput,
        GetProblemReportListOutput,
        ProblemReportFilterInput>
    {
        Task SendAnswerAsync(int id, SendAnswerInput input);
    }
}