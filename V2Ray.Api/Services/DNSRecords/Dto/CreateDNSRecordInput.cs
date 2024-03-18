using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace V2Ray.Api.Services.DNSRecords.Dto
{
    public class CreateDNSRecordInput
    {
        public string IPv4 { get; set; }
        public string IPv6 { get; set; }
    }
}