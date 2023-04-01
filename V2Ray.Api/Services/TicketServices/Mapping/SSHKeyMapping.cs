using AutoMapper;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.MessageServices.Dto;
using V2Ray.Api.Services.TicketServices.Dto;

namespace V2Ray.Api.Services.MessageServices.Mapping
{
    public class MessageMapping : Profile
    {
        public MessageMapping()
        {
            CreateMap<Message, GetMessageListOutput>()
                .ForMember(a => a.CreateAt, c => c.MapFrom(d => d.CreatedAt.ToPeString("dddd, dd MMMM,yyyy ,HH:mm")));


            CreateMap<Message, GetMessageOutput>()
                .ForMember(a => a.CreateAt, c => c.MapFrom(d => d.CreatedAt.ToPeString("dddd, dd MMMM,yyyy ,HH:mm")));
            CreateMap<CreateMessageInput, Message>();

            CreateMap<UpdateMessageInput, Message>();
        }

    }
}