using V2Ray.Api.Controllers;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.Cities.Dto;

namespace V2Ray.Api.Services.Cities
{
    public interface ICitieservice : IBaseService<int,
        UpdateCityInput,
        CreateCityInput,
        GetCityOutput,
        GetCityListOutput,
        CityFilterInput>
    {
    }
}