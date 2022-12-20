using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using V2Ray.Api.Services.Cities.Dto;
using V2Ray.Api.Services.Cities;

namespace V2Ray.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : CustomBaseController
    {
        private readonly ICitieservice _service;

        public CityController(ICitieservice service)
        {
            _service = service;
        }

        [HttpGet("all-cities")]
        [Authorize]
        public async Task<ApiResponse> Filter()
        {
            //filter.CityId = CityId;
            //filter.IsAdmin = IsAdmin;
            var result = await _service.GetAllAsync(new CityFilterInput
            {
                ItemsPerPage = 100
            });
            return new ApiResponse(result);
        }

        [HttpGet("Cities")]
        [Authorize]
        public async Task<ApiResponse> Filter([FromQuery] CityFilterInput filter)
        {
            //filter.CityId = CityId;
            //filter.IsAdmin = IsAdmin;
            var result = await _service.GetAllAsync(filter);
            return new ApiResponse(result);
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("{id:int}")]
        [Authorize]

        public async Task<ApiResponse> Update([FromRoute] int id, [FromBody] UpdateCityInput input)
        {
            await _service.UpdateAsync(id, input);

            return new ApiResponse();
        }


        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromBody] CreateCityInput input)
        {
            await _service.InsertAsync(input);
            return new ApiResponse();
        }


        /// <summary>
        /// دریافت اطلاعات یک کاربر
        /// </summary>
        ///
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ApiResponse> Get([FromRoute] int id)
        {
           var result =   await _service.GetById(id);

            return new ApiResponse(result);
        }

        


        /// <summary>
        /// ویرایش پروفایل یک کاربر
        /// </summary>
        ///
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ApiResponse> Delete([FromRoute] int id)
        {
            await _service.SoftDelete(id);

            return new ApiResponse();
        }
        //[HttpPost("DeleteKey")]
        //public async Task<ApiResponse> DeleteKey(CreateKeyInput input)
        //{

        //    var httpClient = GetCookie(input);
        //    var re = await httpClient.PostAsJsonAsync($"https://{input.Url}:{input.Port}/xui/inbound/del/{input.Id}", new { }); ;
        //    var ttt = await re.Content.ReadAsStringAsync();
        //    var root = JsonConvert.DeserializeObject<Root>(ttt);
        //    return new ApiResponse(root);
        //}
        //[HttpPost("GetKeys")]
        //public async Task<ApiResponse> GetKeys(CreateKeyInput input)
        //{

        //    var httpClient = GetCookie(input);
        //    var re = await httpClient.PostAsJsonAsync($"https://{input.Url}:{input.Port}/xui/inbound/list", new { }); ;
        //    var ttt = await re.Content.ReadAsStringAsync();
        //    var root = JsonConvert.DeserializeObject<Root>(ttt);
        //    return new ApiResponse(root);
        //}

      
      
    }
   
}