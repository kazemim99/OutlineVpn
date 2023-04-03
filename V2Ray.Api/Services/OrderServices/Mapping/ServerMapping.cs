using AutoMapper;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.OrderServices.Dto;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.OrderServices.Mapping
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<Order, GetOrderListOutput>()
                .ForMember(c => c.KeyUserName, a => a.MapFrom(b => b.User.SSHKeyInfos.First().UserName))
                .ForMember(c => c.Status, a => a.MapFrom(b => b.Status.GetDescription()));

            CreateMap<Order, GetOrderOutput>()
                .ForMember(c => c.KeyUserName, a => a.MapFrom(b => b.User.SSHKeyInfos.First().UserName))
                .ForMember(c => c.Status, a => a.MapFrom(b => b.Status.GetDescription()));



            CreateMap<CreateOrderInput, Order>()
                .ForMember(c => c.Status, a => a.MapFrom(b => OrderStateEnum.Waiting));


            CreateMap<UpdateOrderInput, Order>();
        }

    }
}