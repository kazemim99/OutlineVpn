using System.Collections.Generic;
using V2Ray.Api.Entity;

namespace V2Ray.Api.Services.DNSRecords.Dto
{
    public class GetDNSRecordOutput : EntityDto<int>
    {

        public string IPv4 { get; set; }
        public string IPv6 { get; set; }

    }
}