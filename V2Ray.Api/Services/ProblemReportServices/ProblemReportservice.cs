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
using V2Ray.Api.Services.sms;

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
        private readonly IRahyabSmsSender _sms;

        public ProblemReportservice(IMapper mapper, DB db, IRahyabSmsSender sms) : base(mapper, db)
        {
            _db = db;
            _sms = sms;
        }
        public override async Task InsertAsync(CreateProblemReportInput input)
        {
            if (_db.ProblemReports.Any(a => a.UserId == input.UserId && a.State == ProblemReportEnum.Sended))
                throw new ApiException("قبلا از جانب شما گزارشی دریافت شده و درحال انجام است");
            if (input.ReturnMoney)
            {
                if(!_db.Orders.Any(a=>a.UserId == input.UserId && a.Status == OrderStateEnum.Confirmed))
                {
                    throw new ApiException("شما هیچ تراکنش فعالی ندارید");
                }

                if (!_db.Orders.Any(a => a.UserId == input.UserId && a.Status == OrderStateEnum.Confirmed && a.CreatedAt.Date > DateTime.Now.AddDays(10).Date))
                {
                    throw new ApiException("بیش از ده روز از خرید شما گذشته");
                }
            }

            await _sms.SendAsync(new sms.Rahyab.RahyabSendSmsReques
            {
                message = $"Prombler Report : {input.ReturnMoney} {input.Operator.GetDescription()}",
                destinationAddress = "09123135143"
            });
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
