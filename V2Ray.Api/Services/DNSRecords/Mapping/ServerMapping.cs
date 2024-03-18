using AutoMapper;
using V2Ray.Api.Services.DNSRecords.Dto;

namespace V2Ray.Api.Services.DNSRecords.Mapping
{
    public class DNSRecordsMapping : Profile
    {
        public DNSRecordsMapping()
        {
            CreateMap<Entity.DNSRecord, GetDNSRecordListOutput>();
            CreateMap<Entity.DNSRecord, GetDNSRecordListOutput>();
            CreateMap<CreateDNSRecordInput, Entity.DNSRecord>();
            CreateMap<UpdateDNSRecordInput, Entity.DNSRecord>();
        }

    }
}