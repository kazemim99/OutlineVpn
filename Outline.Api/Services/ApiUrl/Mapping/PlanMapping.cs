using AutoMapper;
using Outline.Api.Entity;
using Outline.Api.Services.UserServices.Dto;

namespace Outline.Api.Mapping
{
    public class ApiUrlMapping : Profile
    {
        public ApiUrlMapping()
        {
            CreateMap<ApiUrl, GetApiUrlListOutput>();

            CreateMap<ApiUrl, GetApiUrlOutput>();

            CreateMap<CreateApiUrlInput, ApiUrl>();

            CreateMap<UpdateApiUrlInput, ApiUrl>();
        }

    }
}