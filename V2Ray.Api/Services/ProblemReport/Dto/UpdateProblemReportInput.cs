namespace V2Ray.Api.Services.ProblemReports.Dto
{
    public class UpdateProblemReportInput : CreateProblemReportInput
    {
    }
    public class SendAnswerInput
    {
        public string Answer { get; set; }
    }
}