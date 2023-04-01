using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using V2Ray.Api.Services.MessageServices;
using V2Ray.Api.Services.MessageServices.Dto;
using V2Ray.Api.Services.TicketServices.Dto;

namespace V2Ray.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TickeController : CustomBaseController
    {
        private readonly IMessageService _MessageService;

        public TickeController(IMessageService MessageService)
        {
            _MessageService = MessageService;
        }

        [HttpGet("user-Messages")]
        public async Task<ApiResponse> UserMessages()
        {
          await  _MessageService.GetAllAsync(new MessageFilterInput
            {
                ItemsPerPage = 100
            });
            return new ApiResponse();
        }


        [HttpGet("Messages")]
        public async Task<ApiResponse> Messages(MessageFilterInput input) 
        {
            var result =await _MessageService.GetAllAsync(input);
            return new ApiResponse(result);
        }

        [HttpGet("send-Message")]
        public async Task<ApiResponse> SendMessage([FromForm] CreateMessageInput input)
        {
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "Message");
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
                    input.Attachment = dbPath;
                    input.IsAdmin = IsAdmin;
                    if (!input.IsAdmin)
                    {
                        input.SenderId = UserId;
                    }
                }
            }
            await _MessageService.InsertAsync(input);
            return new ApiResponse();
        }
    }
}
