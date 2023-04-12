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
using V2Ray.Api.Services.V2Keys;
using V2Ray.Api.Services.OrderServices.Dto;
using V2Ray.Api.Services.SSHKeyServices;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;

namespace V2Ray.Api.Services.OrderServices
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
        private readonly DB _db;
        private readonly ISSHKeyService _sshKeyService;
        private readonly IMapper _mapper;
        public OrderService(DB db, IMapper mapper, ISSHKeyService sshKeyService) : base(mapper, db)
        {
            _mapper = mapper;
            _db = db;
            _sshKeyService = sshKeyService;
        }

        public async Task ChangeState(int id, OrderStateEnum stateId)
        {
            var order = await _db.Orders.FirstAsync(a => a.Id == id);
            var sshKey = await _db.SSHKeyInfos.Include(a => a.V2Server).FirstAsync(a => a.Id == order.SSHKeyId);
            if (stateId == OrderStateEnum.Confirmed)
            {
                await _sshKeyService.Charge(order.UserId);
            }
            if (stateId == OrderStateEnum.Invalid)
            {
                await _sshKeyService.DeleteFromVPS(sshKey.UserName, sshKey.V2Server);
                sshKey.Enable = false;
                _db.Update(sshKey);
            }
            order.Status = stateId;
            _db.Update(order);
            _db.SaveChanges();

        }

        public override async Task InsertAsync(CreateOrderInput input)
        {
            var sshKey = _db.SSHKeyInfos.FirstOrDefault(c => c.UserId == input.UserId);
            if(sshKey == null)
                throw new ApiException("شما هنوز هیچ اکانتی نساخته ایید");

            if (sshKey.Orders.Any(a => a.Status == OrderStateEnum.Waiting))
                throw new ApiException("شما یک تراکنش در انتظار تایید دارید");

            input.SSHKeyId = sshKey.Id;
            await base.InsertAsync(input);
        }

    }
}
