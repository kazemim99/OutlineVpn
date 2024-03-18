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
    }
}