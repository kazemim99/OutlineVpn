using V2Ray.Api.Controllers;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.ProblemReports.Dto;

namespace V2Ray.Api.Services.ProblemReports
{
    public interface IProblemReportservice : IBaseService<int,
        UpdateProblemReportInput,
        CreateProblemReportInput,
        GetProblemReportOutput,
        GetProblemReportListOutput,
        ProblemReportFilterInput>
    {
    }
}