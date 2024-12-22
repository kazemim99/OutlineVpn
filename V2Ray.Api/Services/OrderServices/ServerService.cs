using AutoMapper;
using AutoWrapper.Wrappers;
using V2Ray.Api.Database;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.OrderServices.Dto;
using V2Ray.Api.Services.SSHKeyServices;
using V2Ray.Api.Services.sms.Kavenegar.Models.Enums;
using V2Ray.Api.Extensions;

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
        private readonly IMapper _mapper;
        public OrderService(DB db, IMapper mapper) : base(mapper, db)
        {
            _mapper = mapper;
            _db = db;
        }



        public override IQueryable<Order> Filter(OrderFilterInput filter)
        {
            if (filter.UserId == null)
                return new List<Order>().AsQueryable();

            var query = _db.Orders.OrderByDescending(c=>c.CreatedAt).AsQueryable();
        
                //query = query.Where(a => !a.SSHKey.User.IsAdmin);
            if (filter.UserId != null)
                query = query.Where(a => a.UserId == filter.UserId);

            if (filter.FromGeo != null)
                query = query.Where(a => a.CreatedAt.Date >= filter.FromGeo.Value.Date);

            if (filter.ToGeo != null)
                query = query.Where(a => a.CreatedAt.Date <= filter.ToGeo.Value.Date);

            if (filter.DurationId != null)
                query = query.Where(a => a.SSHKey.DurationId == filter.DurationId);

            
            
            return query;

        }
        public override async Task InsertAsync(CreateOrderInput input)
        {
            var sshKey = _db.SSHKeyInfos.FirstOrDefault(c => c.UserId == input.UserId);
            if (sshKey == null)
                throw new ApiException("شما هنوز هیچ اکانتی نساخته ایید");

            if (sshKey.Orders.Any(a => a.Status == OrderStateEnum.Waiting))
                throw new ApiException("شما یک تراکنش در انتظار تایید دارید");

            input.SSHKeyId = sshKey.Id;
            await base.InsertAsync(input);
        }

        public OrdersCountOutput OrderCount(OrderFilterInput filter, string[] vs)
        {
            if (filter.UserId == null)
                return new OrdersCountOutput();

            var query = _db.Orders.OrderByDescending(c => c.Id).AsQueryable();

            
            if (filter.UserId != null)
                query = query.Where(a => a.UserId == filter.UserId);

            if (filter.FromGeo != null)
                query = query.Where(a => a.CreatedAt.Date >= filter.FromGeo.Value.Date);

            if (filter.ToGeo != null)
                query = query.Where(a => a.CreatedAt.Date <= filter.ToGeo.Value.Date);

            if (filter.DurationId != null)
                query = query.Where(a => a.DurationId == filter.DurationId);


         
            return new OrdersCountOutput
            {
                ThreeMonthCount = query.Count(c=>c.DurationId == 90),
                OneMonthCount = query.Count(c=>c.DurationId == 30),
                TwoMonthCount = query.Count(c=>c.DurationId == 60),
                UnknownCount = query.Count(c=>c.DurationId == 0),
            };

        }
    }
}
