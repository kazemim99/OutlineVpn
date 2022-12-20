using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using static V2Ray.Api.Services.Server.ServerService;

namespace V2Ray.Api.Services.V2Keys.Dto
{
    public class CreateV2KeyInput
    {
        public int UserId { get; set; }
        public int ServerId { get; set; }
        public int Capacity { get; set; }
        public int Port { get; set; }
        public int ExpireDate { get; set; }
        public bool State { get; set; }
        public Protocol Protocol { get; set; }
        public string Key { get;  set; }
    }
}