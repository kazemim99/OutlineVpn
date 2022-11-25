using Newtonsoft.Json;
using Outline.Api.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Outline.Api.Services.UserServices.Dto
{
    public class ApiUrlFilterInput : PaginationModelInput
    {
        public string? Title { get; set; }
    }

}