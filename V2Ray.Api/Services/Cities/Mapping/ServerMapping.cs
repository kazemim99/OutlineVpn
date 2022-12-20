using AutoMapper;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.Cities.Dto;

namespace V2Ray.Api.Services.Cities.Mapping
{
    public class CityMapping : Profile
    {
        public CityMapping()
        {
            CreateMap<City, GetCityListOutput>();

            CreateMap<City, GetCityOutput>();

            CreateMap<CreateCityInput, City>();

            CreateMap<UpdateCityInput, City>();
        }

    }
}