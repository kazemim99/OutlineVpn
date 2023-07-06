using AutoMapper;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.OrderServices.Dto;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;
using V2Ray.Api.Shared;

namespace V2Ray.Api.Services.OrderServices.Mapping
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<Order, GetOrderListOutput>()
                .ForMember(c => c.KeyUserName, a => a.MapFrom(b => b.SSHKey.UserName))
                .ForMember(c => c.Creator, a => a.MapFrom(d => $"{d.User.FirstName} {d.User.LastName}"))
                .ForMember(c => c.CreatedAt, a => a.MapFrom(b => b.CreatedAt.ToPeString("yyyy/MM/dd")))
                .ForMember(c => c.Status, a => a.MapFrom(b => b.Status.GetDescription()));

            CreateMap<Order, GetOrderOutput>()
                .ForMember(c => c.KeyUserName, a => a.MapFrom(b => b.User.SSHKeyInfos.First().UserName))
                .ForMember(c => c.CreateAt, a => a.MapFrom(b => b.CreatedAt.ToPeString("yyyy/MM/dd")))
                .ForMember(c => c.Status, a => a.MapFrom(b => b.Status.GetDescription()));



            CreateMap<CreateOrderInput, Order>()
                .ForMember(c => c.Status, a => a.MapFrom(b => OrderStateEnum.Waiting));


            CreateMap<UpdateOrderInput, Order>();
        }

        private IEnumerable<OptionItem> GetStatus()
        {
            var result = Enum.GetValues(typeof(OrderStateEnum))
               .Cast<OrderStateEnum>()
               .Select(t => new OptionItem { Id = ((int)t), Text = t.GetDescription() });
            return result;
        }
    }
}