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
                .ForMember(a => a.ServerName, c => c.MapFrom(b => b.V2Server.Url))
                .ForMember(a => a.ChargeDate, c => c.MapFrom(b => b.ExpireDate.ToPeString("yyyy/MM/dd HH:mm")))
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToPeString("yyyy/MM/dd")));

            CreateMap<SSHKey, GetSSHKeyOutput>()
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToPeString("yyyy/MM/dd")))
                .ForMember(a => a.Amount, c => c.MapFrom(b =>
                b.Orders.OrderByDescending(c=>c.CreatedAt)
                .Select(c=> new { c.SSHKey.UserName,c.Amount}).FirstOrDefault(c=>c.UserName == b.UserName).Amount));

            CreateMap<CreateSSHKeyInput, SSHKey>()
                .ForMember(a => a.Enable, c => c.MapFrom(b => true))
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.Value.ToGeo().Date));

            CreateMap<UpdateSSHKeyInput, SSHKey>()
                .ForMember(a => a.Enable, c => c.MapFrom(b => true))
                .ForMember(a => a.UpdateAt, c => c.MapFrom(b => DateTime.UtcNow.ToGeo().Date))
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.Value.ToGeo().Date));

            CreateMap<UpdateSSHKeyInput, CreateSSHKeyInput>()
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.Value.ToGeo().Date));
        }

    }
}