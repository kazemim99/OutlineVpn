using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Services.DNSRecords.Dto
{
    public class DNSRecordFilterInput : PaginationModelInput
    {
        public string IPv4 { get; set; }
        public string IPv6 { get; set; }
    }

}