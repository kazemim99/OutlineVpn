using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using System.Text;
using V2Ray.Api.Database;
using V2Ray.Api.Shared;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Controllers;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using V2Ray.Api.Services.Orders.Dto;

namespace V2Ray.Api.Services.Orders
{
    public class OrderService : BaseService<Order,
        int,
        UpdateOrderInput,
        CreateOrderInput,
        GetOrderOutput,
        GetOrderListOutput,
        OrderFilterInput>,
        IOrderservice
    {
     
        public OrderService(IMapper mapper,DB db) :base(mapper,db)
        {}

        public override async Task InsertAsync(CreateOrderInput input)
        {
            if (_db.Orders.Any(c => c.UserId == input.UserId && c.Status == sms.Kavenegar.Models.Enums.OrderStateEnum.Waiting))
                throw new ApiException("شما یک درخواست  در انتظار تایید دارید");

          await  base.InsertAsync(input);
        }
    }
}
