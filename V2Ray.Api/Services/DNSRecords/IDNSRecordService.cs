using V2Ray.Api.Controllers;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.DNSRecords.Dto;

namespace V2Ray.Api.Services.DNSRecords
{
    public interface IDNSRecordService : IBaseService<int,
        UpdateDNSRecordInput,
        CreateDNSRecordInput,
        GetDNSRecordOutput,
        GetDNSRecordListOutput,
        DNSRecordFilterInput>
    {

    }
}