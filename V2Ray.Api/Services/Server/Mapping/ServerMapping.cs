using AutoMapper;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Services.Server.Dto;

namespace V2Ray.Api.Services.Server.Mapping
{
    public class V2ServerMapping : Profile
    {
        public V2ServerMapping()
        {
            CreateMap<V2Server, GetServerListOutput>().ForMember(c=>c.KeyCount,a=>a.MapFrom(b=>b.SSHKeys.Count(a=>a.Enable)));

            CreateMap<V2Server, GetServerOutput>();

            CreateMap<CreateServerInput, V2Server>();

            CreateMap<UpdateServerInput, V2Server>();
        }

    }
}