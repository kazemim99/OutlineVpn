using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.Cities.Dto
{
    public class CityFilterInput : PaginationModelInput
    {
        public string? Title { get; set; }
    }

}