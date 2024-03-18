using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using V2Ray.Api.Services.DNSRecords;

namespace V2Ray.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class DNSRecordController : CustomBaseController
    {
        private readonly IDNSRecordService _dnsRecordService;

        public DNSRecordController(IDNSRecordService dnsRecordService)
        {
            _dnsRecordService = dnsRecordService;
        }

        [HttpGet("dnsRecores")]
        public async Task<ApiResponse> DnsRecordes()
        {
          await  _dnsRecordService.GetAllAsync(new Services.DNSRecords.Dto.DNSRecordFilterInput
            {
                ItemsPerPage = 100
            });
            return new ApiResponse();
        }
  
    }
}
