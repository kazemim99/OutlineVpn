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
            CreateMap<V2Key, GetV2KeyListOutput>().ForMember(a => a.Url, c => c.MapFrom(b => b.V2Server.Url))
                .ForMember(a => a.Key, c => c.MapFrom(b => b.Key))
                .ForMember(a => a.V2ServerId, c => c.MapFrom(b => b.V2ServerId))
                .ForMember(a => a.CreateDate, c => c.MapFrom(b => b.CreatedAt.ToPeString("yyyy/MM/dd")));

            CreateMap<V2Key, GetV2KeyOutput>()
                .ForMember(a => a.Capacity, c => c.MapFrom(a => a.Capacity.GigaByteToBytes()));


            CreateMap<CreateV2KeyInput, V2Key>().
                ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToTimeStamp())).
                ForMember(a => a.V2ServerId, c => c.MapFrom(b => b.ServerId));

            CreateMap<UpdateV2KeyInput, V2Key>().
                ForMember(a => a.ExpireDate, c => c.MapFrom(b => b.ExpireDate.ToTimeStamp())).
                ForMember(a => a.V2ServerId, c => c.MapFrom(b => b.ServerId));
        }

    }
}