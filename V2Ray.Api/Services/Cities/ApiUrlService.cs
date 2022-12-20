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
using V2Ray.Api.Services.Cities.Dto;

namespace V2Ray.Api.Services.Cities
{
    public class CitieService : BaseService<City,
        int,
        UpdateCityInput,
        CreateCityInput,
        GetCityOutput,
        GetCityListOutput,
        CityFilterInput>,
        ICitieservice
    {
        private readonly DB _db;

        private readonly IMapper _mapper;
        public CitieService(IMapper mapper,DB db) :base(mapper,db)
        {

        }
    }
}
