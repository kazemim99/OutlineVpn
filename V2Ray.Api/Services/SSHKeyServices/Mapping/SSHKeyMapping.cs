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
                .ForMember(a => a.Email, c => c.MapFrom(b => b.User.Email))
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToPeString("yyyy/MM/dd")))
                .ForMember(a => a.CreatedAt, c => c.MapFrom(b => b.User.CreatedAt.ToPeString("yyyy/MM/dd")));

            CreateMap<SSHKey, GetSSHKeyOutput>()
                .ForMember(a => a.Email, c => c.MapFrom(b => b.User.Email))
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToPeString("yyyy/MM/dd")));

            CreateMap<CreateSSHKeyInput, SSHKey>()
                .ForMember(a => a.Enable, c => c.MapFrom(b => true))
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.Value.ToGeo()));

            CreateMap<UpdateSSHKeyInput, SSHKey>()
                .ForMember(a => a.Enable, c => c.MapFrom(b => true))
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.Value.ToGeo()));

            CreateMap<UpdateSSHKeyInput, CreateSSHKeyInput>()
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.Value.ToGeo()));
        }

    }
}