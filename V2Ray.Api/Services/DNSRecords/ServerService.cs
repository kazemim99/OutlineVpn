using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using V2Ray.Api.Database;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.SSHKeyServices;
using V2Ray.Api.Services.DNSRecords.Dto;

namespace V2Ray.Api.Services.DNSRecords
{
    public class DNSRecordService : BaseService<DNSRecord,
        int,
        UpdateDNSRecordInput,
        CreateDNSRecordInput,
        GetDNSRecordOutput,
        GetDNSRecordListOutput,
        DNSRecordFilterInput>,
        IDNSRecordService
    {
        private readonly DB _db;

        private readonly IMapper _mapper;
        private readonly ISSHKeyService _sshService;
        public DNSRecordService(DB db, IMapper mapper, ISSHKeyService sshService) : base(mapper, db)
        {
            _mapper = mapper;
            _db = db;
            _sshService = sshService;
        }

    }
}