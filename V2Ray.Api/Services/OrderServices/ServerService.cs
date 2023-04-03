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

    }
}
