using Outline.Api.Entity;
using Outline.Api.Services.UserServices.Dto;
using Outline.Api.Shared;

namespace Outline.Api.Services.ApiUrlServices
{
    public interface IApiUrlService : IBaseService<int,
        UpdateApiUrlInput,
        CreateApiUrlInput,
        GetApiUrlOutput,
        GetApiUrlListOutput,
        ApiUrlFilterInput>
    {

        Task IsDelete(int id, string fullName);

        //Task AddComplexToApiUrl(AddComplexToApiUrl input);

        Task ChangeState(int id, string fullName);
    }
}