using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outline.Api.Database;
using Outline.Api.Services.sms;
using Outline.Api.Services.sms.Rahyab;
using Outline.Api.Services.ApiUrlServices;
using OutlineVpn;
using Outline.Api.Services.UserServices.Dto;
using Outline.Api.Services.ApiUrlServices;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Net.Http.Headers;

namespace Outline.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiUrlController : CustomBaseController
    {
        private readonly IApiUrlService _service;
        private readonly IOutlineApi _outline;

        public ApiUrlController(IApiUrlService service, IOutlineApi outline)
        {
            _service = service;
            _outline = outline;
        }

        [HttpGet("ApiUrls")]
        [Authorize]
        public async Task<ApiResponse> Filter([FromQuery] ApiUrlFilterInput filter)
        {
            //filter.ApiUrlId = ApiUrlId;
            //filter.IsAdmin = IsAdmin;
            var result = await _service.GetAllAsync(filter);
            return new ApiResponse(result);
        }

        /// <summary>
        /// ویرایش یک کاربر 
        /// </summary>
        ///
        [HttpPut("{id:int}")]
        [Authorize]

        public async Task<ApiResponse> Update([FromRoute] int id, [FromBody] UpdateApiUrlInput input)
        {
            await _service.UpdateAsync(id, input);

            return new ApiResponse();
        }


        [HttpGet("setApiUrl/{id}")]
        public async Task<ApiResponse> SetApiUrl([FromRoute] int id)
        {
            await _outline.SetUrl(id);
            return new ApiResponse();
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> Create([FromBody] CreateApiUrlInput input)
        {
            await _service.InsertAsync(input);
            return new ApiResponse();
        }



        /// <summary>
        /// دریافت اطلاعات یک کاربر
        /// </summary>
        ///
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ApiResponse> Get([FromRoute] int id)
        {
            var result = await _service.GetById(id);

            return new ApiResponse(result);
        }


        /// <summary>
        /// ویرایش پروفایل یک کاربر
        /// </summary>
        ///
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ApiResponse> Delete([FromRoute] int id)
        {
            await _service.SoftDelete(id);

            return new ApiResponse();
        }
        [HttpPost("Create_Server")]
        public async Task<ApiResponse> Createkey(CreateServerInput input)
        {

            var httpClient = GetCookie();
            Settins settings;
            Obj request;
            NewMethod(out settings, out request,input);

            var settins = JsonConvert.SerializeObject(request, Formatting.Indented);

            var content = new StringContent(settins, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = await httpClient.PostAsync("https://gre.iranoutline.tk:54321/xui/inbound/add", content);
            var tt = await result.Content.ReadAsStringAsync();
            var serverResponse = JsonConvert.DeserializeObject<ServerResponse>(tt);
            if (!serverResponse.success)
                throw new ApiException(serverResponse.msg);

            result.EnsureSuccessStatusCode();
            var key = $"{input.Url}://{settings.clients.First().id}@{input.Url}:{request.port}";
            if (input.Url == "vless")
                key += $"?type=tcp&security=xtls&flow=xtls-rprx-direct#{request.remark}";
            else
                key += $"#{request.remark}";


            var re = await httpClient.PostAsJsonAsync("https://gre.iranoutline.tk:54321/xui/inbound/list", new { });
            var ttt = await re.Content.ReadAsStringAsync();
            var root = JsonConvert.DeserializeObject<Root>(ttt);

            //var cookieContainer = new CookieContainer();
            //using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            //using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            //{

            //    cookieContainer.Add(baseAddress, new Cookie("session", "MTY3MDM4MjgzNnxEdi1CQkFFQ180SUFBUkFCRUFBQVpmLUNBQUVHYzNSeWFXNW5EQXdBQ2t4UFIwbE9YMVZUUlZJWWVDMTFhUzlrWVhSaFltRnpaUzl0YjJSbGJDNVZjMlZ5XzRNREFRRUVWWE5sY2dIX2hBQUJBd0VDU1dRQkJBQUJDRlZ6WlhKdVlXMWxBUXdBQVFoUVlYTnpkMjl5WkFFTUFBQUFIUC1FR1FFQ0FRcHJZWHBsYldrdWJYTjBBUWd4Y1RKM00yVTBjZ0E9fM6Kkb8broM-OWu2auQouDb805UWZ7Mvr7x_H7cqXSZG"));
            //    var content = new StringContent(json, Encoding.UTF8, "application/json");

            //    var result = await client.PostAsync("/lyYa/xui/inbound/add", content);
            //    result.EnsureSuccessStatusCode();
            //}

            return new ApiResponse();

        }

        private void NewMethod(out Settins settings, out Obj request, CreateServerInput input)
        {
            settings = new Settins()
            {
                clients = new List<Client>
                {
                    new Client
                    {
                        id = Guid.NewGuid().ToString(),
                        flow = "xtls-rprx-direct"
                    }
                },
                decryption = "none",
                fallbacks = new List<object>()
            };
            var sniffing = new Sniffing
            {
                network = "tcp",
                security = "xtls",
                tcpSettings = new TcpSettings
                {
                    header = new Header
                    {
                        type = "none"
                    }
                },
                xtlsSettings = new XtlsSettings
                {
                    serverName = "gre.iranoutline.tk",
                    certificates = new List<Certificate>()
                    {
                        new Certificate
                    {
                    certificateFile = "/root/cert.crt",
                    keyFile = "/root/private.key"
                    }
                    }
                },
            };
            var streamSettings = new streamSettings()
            {
                destOverride = new List<string>
                {
                    "http",
                    "tls"
                },
                enabled = true
            };
            request = new Obj()
            {
                down = 0,
                enable = true,
                expiryTime = ConvertToTimestamp(DateTime.Now.AddDays(30)),
                listen = "",
                port = new Random().Next(10000, 60000),
                protocol = input.Url,
                remark = input.Remark,
                settings = JsonConvert.SerializeObject(settings, Formatting.Indented),
                sniffing = JsonConvert.SerializeObject(sniffing, Formatting.Indented),
                streamSettings = JsonConvert.SerializeObject(streamSettings, Formatting.Indented),
                tag = "inbound-12058",
                total = GigaByteToBytes(50),
                up = 0
            };
        }

        private HttpClient GetCookie()
        {
            var uri = new Uri("gre.iranoutline.tk:54321");

            var loging = new
            {
                username = "kazemi.mst",
                password = "1q2w3e4r"
            };

            var formContent = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("username", "admin"),
    new KeyValuePair<string, string>("password", "admin")
});
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;

            var loginModel = JsonConvert.SerializeObject(loging, Formatting.Indented);

            HttpClient client = new HttpClient(handler);
            var response = client.PostAsync("https://gre.iranoutline.tk:54321/login", formContent).Result;

            var stringContent = response.Content.ReadAsStringAsync().Result;

            cookies.Add(uri, new Cookie("domain", "gre.iranoutline.tk"));

            var responseCookies = cookies.GetCookies(uri).Cast<Cookie>().ToList();

            return client;


        }
        private  long GigaByteToBytes(long gigateBytes)
        {
           return gigateBytes * Convert.ToInt64(Math.Pow(1024, 3));
        }
        private long ConvertToTimestamp(DateTime value)
        {
            long epoch = (value.Ticks - 621355968000000000) / 10000000;
            return epoch;
        }
    }

    public class ServerResponse
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public object obj { get; set; }
    }
    public class streamSettings
    {
        public bool enabled { get; set; }
        public List<string> destOverride { get; set; }
    }
    public class Certificate
    {
        public string certificateFile { get; set; }
        public string keyFile { get; set; }
    }

    public class Header
    {
        public string type { get; set; }
    }

    public class Sniffing
    {
        public string network { get; set; }
        public string security { get; set; }
        public XtlsSettings xtlsSettings { get; set; }
        public TcpSettings tcpSettings { get; set; }
    }

    public class TcpSettings
    {
        public Header header { get; set; }
    }

    public class XtlsSettings
    {
        public string serverName { get; set; }
        public List<Certificate> certificates { get; set; }
    }


    public class Client
    {
        public string id { get; set; }
        public string flow { get; set; }
    }

    public class Settins
    {
        public List<Client> clients { get; set; }
        public string decryption { get; set; }
        public List<object> fallbacks { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Obj
    {
        public int id { get; set; }
        public long up { get; set; }
        public long down { get; set; }
        public long total { get; set; }
        public string remark { get; set; }
        public bool enable { get; set; }
        public long expiryTime { get; set; }
        public string listen { get; set; }
        public int port { get; set; }
        public string protocol { get; set; }
        public string settings { get; set; }
        public string streamSettings { get; set; }
        public string tag { get; set; }
        public string sniffing { get; set; }
    }

    public class Root
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public List<Obj> obj { get; set; }
    }



}