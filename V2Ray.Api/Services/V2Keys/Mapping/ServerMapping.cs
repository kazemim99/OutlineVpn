using AutoMapper;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.V2Keys.Dto;

namespace V2Ray.Api.Services.V2Keys.Mapping
{
    public class V2V2KeyMapping : Profile
    {
        public V2V2KeyMapping()
        {
            CreateMap<V2Key, GetV2KeyListOutput>().ForMember(a => a.User, c => c.MapFrom(b => $"{b.User.FirstName} {b.User.LastName}"));

            CreateMap<V2Key, GetV2KeyOutput>();

            CreateMap<CreateV2KeyInput, V2Key>();

            CreateMap<UpdateV2KeyInput, V2Key>();
        }

    }
}