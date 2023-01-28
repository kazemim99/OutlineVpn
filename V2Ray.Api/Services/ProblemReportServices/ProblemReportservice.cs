using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using System.Text;
using V2Ray.Api.Database;
using V2Ray.Api.Shared;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Controllers;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;
using V2Ray.Api.Services.ProblemReportServices.Dto;

namespace V2Ray.Api.Services.ProblemReportServices
{
    public class ProblemReportservice : BaseService<ProblemReport,
        int,
        UpdateProblemReportInput,
        CreateProblemReportInput,
        GetProblemReportOutput,
        GetProblemReportListOutput,
        ProblemReportFilterInput>,
        IProblemReportservice
    {
        private readonly DB _db;

        private readonly IMapper _mapper;
        public ProblemReportservice(IMapper mapper, DB db) : base(mapper, db)
        {
            _db = db;
        }
        public override async Task InsertAsync(CreateProblemReportInput input)
        {
            if (_db.ProblemReports.Any(a => a.UserId == input.UserId && a.State == ProblemReportEnum.Sended))
                throw new ApiException("قبلا از جانب شما گزارشی دریافت شده و درحال انجام است");


            await base.InsertAsync(input);
        }

        public async Task SendAnswerAsync(int id, SendAnswerInput input)
        {
            var answer = await _db.ProblemReports.FirstAsync(a => a.Id == id);
            answer.Answer = input.Answer;
            answer.State = ProblemReportEnum.Answerd;
            _db.ProblemReports.Update(answer);
            _db.SaveChanges();
        }

        public override IQueryable<ProblemReport> Filter(ProblemReportFilterInput filter)
        {
            var query = _db.ProblemReports.AsQueryable();

            if (filter.UserId != null)
                query = query.Where(a => a.UserId == filter.UserId);

            return query;
        }
    }
}
