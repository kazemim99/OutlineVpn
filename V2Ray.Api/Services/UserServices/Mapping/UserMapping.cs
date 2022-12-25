using AutoMapper;
using V2Ray.Api.Entity;
using V2Ray.Api.Services.UserServices.Dto;

namespace V2Ray.Api.Services.UserServices.Mapping
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, GetUserListOutput>();

            CreateMap<User, GetUserOutput>()
                   .ForMember(a => a.Avatar,
                    c => c.MapFrom(d => GetAvatar(d)));

            CreateMap<CreateUserInput, User>().ForMember(a => a.NeedConfirm, c => c.MapFrom(b => true))

                     .ForMember(a => a.Avatar,
                    c => c.Ignore())
                      .AfterMap((input, user) =>
                {
                    //user.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
                });

            //CreateMap<AddComplexToUser, ComplexUser>();

            CreateMap<UpdateUserInput, User>()
                 .ForMember(a => a.Roles,
                    c =>
                        c.Ignore());
        }



        private string GetAvatar(User a)
        {
            if (a.Avatar == null) return "";
            return $"api/publicData/get-file/{a.Avatar.Replace('\\', '*')}";
        }


        #region Private +

        //private static void AddPermissions(IEnumerable<int> input, User user)
        //{
        //    user.Permissions = input.Select(a => new Permission()
        //    {
        //        Id = a
        //    }).ToList();
        //}

        private static void AddRoles(IEnumerable<int> input, User user)
        {
            foreach (var item in input)
            {
                user.Roles.Clear();
                user.Roles.Add(new UserRole()
                {
                    UserId = user.Id,
                    RoleId = item
                });
            }
        }

        #endregion Private +
    }
}