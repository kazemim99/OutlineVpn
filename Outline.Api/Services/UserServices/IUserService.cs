using Outline.Api.Entity;
using Outline.Api.Extensions;
using Outline.Api.Services.UserServices.Dto;
using Outline.Api.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Outline.Api.Services.UserServices
{
    public interface IUserService : IBaseService<int,
        UpdateUserInput,
        CreateUserInput,
        GetUserOutput,
        GetUserListOutput,
        UserFilterInput>
    {
        Task<LoginResultDto> Login(LoginDto login);

        Task IsDelete(int id, string fullName);

        //Task AddComplexToUser(AddComplexToUser input);

        Task ChangeState(int id, string fullName);

        Task<GetUserOutput> GetUserByMobile(string mobile);

        Task SendCode(string mobile);
        void SendMail(string mail);
        void VerifyCode(string code, string mobile);

        Task ChangePasswordAsync(string mobile, string password);

        Task<IEnumerable<OptionItem>> GetSelectList(string input);
        Task SetAccessKey(int id, string accessUrl);
        Task UpdateConsumedTraffic(double remainigCapacity, int userId);
        ApiUrl UserServer(int userId);
    }
}