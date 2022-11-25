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

namespace Outline.Api.Services.ApiUrlServices
{
    public class ApiUrlService : BaseService<ApiUrl,
        int,
        UpdateApiUrlInput,
        CreateApiUrlInput,
        GetApiUrlOutput,
        GetApiUrlListOutput,
        ApiUrlFilterInput>,
        IApiUrlService
    {
        private readonly DB _db;

        private readonly IMapper _mapper;

        public ApiUrlService(DB db, IMapper mapper) : base(mapper, db)
        {
            _mapper = mapper;
            _db = db;
        }


        //public async Task AddComplexToApiUrl(AddComplexToApiUrl input)
        //{
        //    var entity = _mapper.Map<ComplexApiUrl>(input);
        //    _db.ComplexApiUrls.Add(entity);
        //    await _db.SaveChangesAsync();
        //}

        public override async Task UpdateAsync(int id, UpdateApiUrlInput input, params string[] include)
        {
            try
            {
                var ApiUrl = await _db.ApiUrls.FirstOrDefaultAsync(a => a.Id == id);
                if (ApiUrl == null)
                    throw new ApiException(AppErrors.ApiUrlNotFound);


                var map = _mapper.Map<ApiUrl>(input);
              
                map.Id = id;
            


                _db.ApiUrls.Update(map);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task InsertAsync(CreateApiUrlInput input)
        {
            var map = _mapper.Map<CreateApiUrlInput, ApiUrl>(input);
          ;

            await _db.AddAsync(map);
            await _db.SaveChangesAsync();
        }

        public async Task ChangeState(int id, string fullName)
        {
            var ApiUrl = _db.ApiUrls.FirstOrDefault(a => a.Id == id);
            ApiUrl.State = !ApiUrl.State;
            _db.Update(ApiUrl);

            var stateString = ApiUrl.State ? "فعال" : "غیر فعال";

            await _db.SaveChangesAsync();
        }

        public override IQueryable<ApiUrl> Filter(ApiUrlFilterInput filter)
        {
            var query = _db.ApiUrls.AsQueryable();

            if (!filter.Title.IsNullOrEmpty())
                query = query.Where(a => a.Title.Contains(filter.Title));

          


            return query;
        }


        public async Task IsDelete(int id, string fullName)
        {
            var ApiUrl = await _db.ApiUrls.FirstAsync(a => a.Id == id);
            ApiUrl.IsDeleted = true;
            _db.Update(ApiUrl);

            await _db.SaveChangesAsync();
        }
       
    }
}