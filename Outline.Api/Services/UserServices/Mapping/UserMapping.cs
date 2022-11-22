using AutoMapper;
using Outline.Api.Entity;
using Outline.Api.Services.UserServices.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Outline.Api.Services.UserServices.Mapping
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, GetUserListOutput>();

            CreateMap<User, GetUserOutput>();

            CreateMap<CreateUserInput, User>()
                      .AfterMap((input, user) =>
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
                });

            //CreateMap<AddComplexToUser, ComplexUser>();

            CreateMap<UpdateUserInput, User>()
                  .ForMember(a => a.Code,
                    c =>
                        c.Ignore())
                 .ForMember(a => a.Roles,
                    c =>
                        c.Ignore());
        }

        private string GenerateUserCode(CreateUserInput d)
        {
            if (d.Code != null)
                return d.Code;

            return new Random().Next(111111, 999999).ToString();
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