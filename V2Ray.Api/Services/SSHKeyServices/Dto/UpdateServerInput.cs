using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace V2Ray.Api.Services.SSHKeyServices.Dto
{

    public class AddAccessKeyOutOut
    {
        public string Code { get; set; }
        public string Pass { get; set; }
    }
    public class UpdateSSHKeyInput : CreateSSHKeyInput
    {
        public bool Charge { get; set; }
       
        [JsonIgnore]
        public bool Enable { get;  set; }
        [JsonIgnore]
        public int UsedTraffic { get;  set; }
    }
}