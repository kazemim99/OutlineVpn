using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Outline.Api.Common;
using Outline.Api.Database;
using Outline.Api.Shared;
using Outline.Api.Services.sms;
using Outline.Api.Services.OTP;
using Outline.Api.Entity;
using Outline.Api.Extensions;
using Outline.Api.Services.sms.Rahyab;
using Outline.Api.Services.UserServices.Dto;

namespace Outline.Api.Services.PlanServices
{
    public class PlanService : BaseService<Plan,
        int,
        UpdatePlanInput,
        CreatePlanInput,
        GetPlanOutput,
        GetPlanListOutput,
        PlanFilterInput>,
        IPlanService
    {
        private readonly DB _db;

        private readonly IMapper _mapper;

        public PlanService(DB db, IMapper mapper) : base(mapper, db)
        {
            _mapper = mapper;
            _db = db;
        }


        //public async Task AddComplexToPlan(AddComplexToPlan input)
        //{
        //    var entity = _mapper.Map<ComplexPlan>(input);
        //    _db.ComplexPlans.Add(entity);
        //    await _db.SaveChangesAsync();
        //}

        public override async Task UpdateAsync(int id, UpdatePlanInput input, params string[] include)
        {
            try
            {
                var Plan = await _db.Plans.FirstOrDefaultAsync(a => a.Id == id);
                if (Plan == null)
                    throw new ApiException(AppErrors.PlanNotFound);


                var map = _mapper.Map<Plan>(input);
              
                map.Id = id;
            

                if (!string.IsNullOrEmpty(map.Image))
                    map.Image = input.Image;
                else
                    map.Image = Plan.Image;


                _db.Plans.Update(map);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task InsertAsync(CreatePlanInput input)
        {
            var map = _mapper.Map<CreatePlanInput, Plan>(input);
          ;

            await _db.AddAsync(map);
            await _db.SaveChangesAsync();
        }

        public async Task ChangeState(int id, string fullName)
        {
            var Plan = _db.Plans.FirstOrDefault(a => a.Id == id);
            Plan.PlanState = !Plan.PlanState;
            _db.Update(Plan);

            var stateString = Plan.PlanState ? "فعال" : "غیر فعال";

            await _db.SaveChangesAsync();
        }

        public override IQueryable<Plan> Filter(PlanFilterInput filter)
        {
            var query = _db.Plans.AsQueryable();

            if (!filter.Title.IsNullOrEmpty())
                query = query.Where(a => a.Title.Contains(filter.Title));

          


            return query;
        }


        public async Task IsDelete(int id, string fullName)
        {
            var Plan = await _db.Plans.FirstAsync(a => a.Id == id);
            Plan.IsDeleted = true;
            _db.Update(Plan);

            await _db.SaveChangesAsync();
        }
       
    }
}