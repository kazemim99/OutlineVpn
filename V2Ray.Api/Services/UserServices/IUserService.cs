using V2Ray.Api.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.UserServices.Dto;
using V2Ray.Api.Shared;

namespace V2Ray.Api.Services.UserServices
{
    public interface IUserService : IBaseService<int,
        UpdateUserInput,
        CreateUserInput,
        GetUserOutput,
        GetUserListOutput,
        UserFilterInput>
    {
        Task Login(LoginDto login);

        Task IsDelete(int id, string fullName);

        //Task AddComplexToUser(AddComplexToUser input);

        Task ChangeState(int id, string fullName);

        Task<GetUserOutput> GetUserByMobile(string mobile);

        Task SendCode(string mobile);
        void SendMail(string mail);
        Task<LoginResultDto> VerifyCode(string code, string mobile);
        Task ChangePasswordAsync(string mobile, string password);
        Task<IEnumerable<OptionItem>> GetSelectList(string input);
    }
}