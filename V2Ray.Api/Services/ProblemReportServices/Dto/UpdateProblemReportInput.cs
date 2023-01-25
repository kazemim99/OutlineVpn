namespace V2Ray.Api.Services.ProblemReportServices.Dto
{
    public class UpdateProblemReportInput : CreateProblemReportInput
    {
    }
    public class SendAnswerInput
    {
        public string Answer { get; set; }
    }
}