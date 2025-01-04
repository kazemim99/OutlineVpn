using AutoMapper;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.SSHKeyServices.Dto;

namespace V2Ray.Api.Services.SSHKeyServices.Mapping
{
    public class SSHKeyMapping : Profile
    {
        public SSHKeyMapping()
        {
            CreateMap<SSHKey, GetSSHKeyListOutput>()
                .ForMember(a => a.Protocol, c => c.MapFrom(b => b.AccountType.GetDescription()))
                .ForMember(a => a.UsedTraffic, c => c.MapFrom(b => b.UsedTraffic))
                .ForMember(a => a.CodeFil, c => c.MapFrom(b => ""))
                .ForMember(a => a.TrafficExpired, c => c.MapFrom(b => b.TrefficExpired ? "اتمام ترافیک": "فعال"))
                .ForMember(a => a.TotalTraffic, c => c.MapFrom(b => b.TotalTraffic))
                .ForMember(a => a.ChargeDate, c => c.MapFrom(b => b.ChargeDate.ToPeString("yyyy/MM/dd")))
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToPeString("yyyy/MM/dd")));


            CreateMap<SSHKey, GetSSHKeyOutput>()
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToPeString("yyyy/MM/dd")));

            CreateMap<CreateSSHKeyInput, SSHKey>()
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToGeo()))
                .ForMember(a => a.Enable, c => c.MapFrom(b => true));

            CreateMap<SSHKey, UpdateSSHKeyInput>();

            CreateMap<UpdateSSHKeyInput, SSHKey>()
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToGeo()))
                .ForMember(a => a.UpdateAt, c => c.MapFrom(b => DateTime.UtcNow));


        }

        private string WireGuardQrCode(string privateKey)
        {
            return @$"
                        [Interface]
                        PrivateKey = {privateKey}
                        Address = 176.66.66.3/32
                        DNS = 8.8.8.8
                        
                        [Peer]
                        PublicKey = TYzkMeJzKvbvvZYCrFLKVT3FZQ6wwZRR3gYstZsHzXk=
                        Endpoint =46.245.64.66:55825
                        AllowedIPs = 0.0.0.0/0
                        PersistentKeepalive = 25";
        }
    }
}