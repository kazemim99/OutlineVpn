using AutoMapper;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.V2Keys.Dto;
using V2Ray.Api.Extensions;
namespace V2Ray.Api.Services.V2Keys.Mapping
{
    public class V2V2KeyMapping : Profile
    {
        public V2V2KeyMapping()
        {
            CreateMap<V2Key, GetV2KeyListOutput>().ForMember(a => a.User, c => c.MapFrom(b => $"{b.User.FirstName} {b.User.LastName}"));

            CreateMap<V2Key, GetV2KeyOutput>().ForMember(a => a.ExpireDate, c => c.MapFrom(a => a.ExpireDate.TimeStampToDateTime()))
                .ForMember(a => a.Capacity, c => c.MapFrom(a => a.Capacity.GigaByteToBytes()));
                

            CreateMap<CreateV2KeyInput, V2Key>().ForMember(a=>a.ExpireDate,c=>c.MapFrom(b=>b.ExpireDate.ToTimeStamp()));

            CreateMap<UpdateV2KeyInput, V2Key>();
        }

    }
}