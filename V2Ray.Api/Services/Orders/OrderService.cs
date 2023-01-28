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
using V2Ray.Api.Services.sms;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;
using V2Ray.Api.Services.SSHKeyServices;

namespace V2Ray.Api.Services.Orders
{
    public class OrderService : BaseService<Order,
        int,
        UpdateOrderInput,
        CreateOrderInput,
        GetOrderOutput,
        GetOrderListOutput,
        OrderFilterInput>,
        IOrderService
    {
        private readonly IRahyabSmsSender _sms;
        private readonly ISSHKeyService _service;
        public OrderService(IMapper mapper, DB db, IRahyabSmsSender sms, ISSHKeyService service) : base(mapper, db)
        {
            _sms = sms;
            _service = service;
        }

        public async Task ChangeStatus(int id,string email, OrderStateEnum stateId)
        {

            var order = await _db.Orders.FindAsync(id);
            if (order == null)
                throw new ApiException("تراکنش یافت نشد");
            if(stateId == OrderStateEnum.Confirmed)
            {
                await _service.ChargeOneMonth(email);
            }

            order.Status = stateId;
            _db.Orders.Update(order);
            _db.SaveChanges();
        }

        public override async Task InsertAsync(CreateOrderInput input)
        {

            if (_db.Orders.Any(c => c.UserId == input.UserId && c.Status == sms.Kavenegar.Models.Enums.OrderStateEnum.Waiting))
                throw new ApiException("شما یک درخواست  در انتظار تایید دارید");

            await _sms.SendAsync(new sms.Rahyab.RahyabSendSmsReques
            {
                message = $"{input.CardNumber}_{input.TranactionNumber}",
                destinationAddress = "09123135143"
            });

            await base.InsertAsync(input);
        }

        public override IQueryable<Order> Filter(OrderFilterInput filter)
        {
            var query = _db.Orders.AsQueryable();

            if (filter.UserId != null)
                query = query.Where(a => a.UserId == filter.UserId);

            return query;
        }
    }
}
