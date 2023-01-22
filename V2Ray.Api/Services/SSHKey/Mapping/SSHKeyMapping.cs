using AutoMapper;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.SSHKeys.Dto;

namespace V2Ray.Api.Services.SSHKeys.Mapping
{
    public class SSHKeyMapping : Profile
    {
        public SSHKeyMapping()
        {
            CreateMap<SSHKey, GetSSHKeyListOutput>()
                .ForMember(a => a.Email, c => c.MapFrom(b => b.User.Email))
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToDateTime().ToPeString("yyyy/MM/dd")))
                .ForMember(a => a.CreatedAt, c => c.MapFrom(b => b.User.CreatedAt.ToPeString("yyyy/MM/dd")));

            CreateMap<SSHKey, GetSSHKeyOutput>()
                .ForMember(a => a.Email, c => c.MapFrom(b => b.User.Email))
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToDateTime().ToPeString("yyyy/MM/dd")))
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToDateTime().ToPeString("yyyy/MM/dd")));


            CreateMap<CreateSSHKeyInput, SSHKey>()
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToGeo().ToTimeStamp()));

            CreateMap<UpdateSSHKeyInput, SSHKey>()
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToGeo().ToTimeStamp()));

            CreateMap<UpdateSSHKeyInput, CreateSSHKeyInput>()
                .ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToGeo().ToTimeStamp()));
        }

    }
}