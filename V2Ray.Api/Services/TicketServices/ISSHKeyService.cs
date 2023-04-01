using V2Ray.Api.Entity;
using V2Ray.Api.Services.MessageServices.Dto;
using V2Ray.Api.Services.TicketServices.Dto;
using V2Ray.Api.Services.V2Keys.Dto;

namespace V2Ray.Api.Services.MessageServices
{
    public interface IMessageService : IBaseService<int,
        UpdateMessageInput,
        CreateMessageInput,
        GetMessageOutput,
        GetMessageListOutput,
        MessageFilterInput>
    {
    }
}