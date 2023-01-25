using AutoMapper;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.Orders.Dto;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;
using V2Ray.Api.Shared;

namespace V2Ray.Api.Services.Orders.Mapping
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<Order, GetOrderListOutput>()
                .ForMember(a=>a.Statuses , c=>c.MapFrom(b=> GetOptions()))
                .ForMember(a=>a.Email , c=>c.MapFrom(b=> b.User.Email))
                .ForMember(a=>a.StateId , c=>c.MapFrom(b=> b.Status));

            CreateMap<Order, GetOrderOutput>();

            CreateMap<CreateOrderInput, Order>();

            CreateMap<UpdateOrderInput, Order>();
        }
        private IEnumerable<OptionItem> GetOptions()
        {
         return   Enum.GetValues(typeof(OrderStateEnum))
                .Cast<OrderStateEnum>()
                .Select(t => new OptionItem { Id = ((int)t), Text = t.GetDescription() });
        }

    }
}