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
using V2Ray.Api.Services.V2Keys.Dto;
using Renci.SshNet;
using V2Ray.Api.Services.MessageServices.Dto;
using System.Diagnostics;
using V2Ray.Api.Services.TicketServices.Dto;

namespace V2Ray.Api.Services.MessageServices
{
    public class MessageService : BaseService<Message,
        int,
        UpdateMessageInput,
        CreateMessageInput,
        GetMessageOutput,
        GetMessageListOutput,
        MessageFilterInput>,
        IMessageService
    {
        private readonly DB _db;



        private readonly IMapper _mapper;
        public MessageService(IMapper mapper, DB db) : base(mapper, db)
        {
            _db = db;
            _mapper = mapper;

        }

    }
}
