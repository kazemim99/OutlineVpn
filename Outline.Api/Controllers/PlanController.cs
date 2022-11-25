using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outline.Api.Database;
using Outline.Api.Services.sms;
using Outline.Api.Services.sms.Rahyab;
using Outline.Api.Services.PlanServices;
using OutlineVpn;
using Outline.Api.Services.UserServices.Dto;

namespace Outline.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanController : CustomBaseController
    {
        private readonly IPlanService _service;
        public PlanController(IPlanService PlanService, IRahyabSmsSender rahyabSmsSender)
        {
            _service = PlanService;
        }


        [HttpGet("Plans")]
        [Authorize]

        public async Task<ApiResponse> Filter([FromQuery] PlanFilterInput filter)
        {
            //filter.PlanId = PlanId;
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

        public async Task<ApiResponse> Update([FromRoute] int id, [FromForm] UpdatePlanInput input)
            {
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "Images", "PlanImages");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(pathToSave);

                if (file.Length > 0)
                {
                    var fileName = file.FileName;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    await using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    input.Image = dbPath;
                }
            }
            await _service.UpdateAsync(id, input);

            return new ApiResponse();
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromForm] CreatePlanInput input)
        {
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "Images", "PlanImages");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(pathToSave);

                if (file.Length > 0)
                {
                    var fileName = file.FileName;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    input.Image = dbPath;
                }
            }
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
            var result = await _service.GetById(id);
        
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
            await _service.Delete(id);
            return new ApiResponse();
        }


    }
}
