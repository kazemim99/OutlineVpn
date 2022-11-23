using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Outline.Api.Services.UserServices.Dto;
using Outline.Api.Common;
using Outline.Api.Database;
using Outline.Api.Shared;
using Outline.Api.Services.sms;
using Outline.Api.Services.OTP;
using Outline.Api.Entity;
using Outline.Api.Extensions;
using Outline.Api.Services.sms.Rahyab;

namespace Outline.Api.Services.UserServices
{
    public class UserService : BaseService<User,
        int,
        UpdateUserInput,
        CreateUserInput,
        GetUserOutput,
        GetUserListOutput,
        UserFilterInput>,
        IUserService
    {
        private readonly DB _db;

        private readonly IMapper _mapper;

        private readonly IConfiguration _config;

        private readonly IOtpService _otpService;

        private readonly IRahyabSmsSender _smsServcie;

        public UserService(DB db, IMapper mapper, IConfiguration config, IOtpService otpService, IRahyabSmsSender smsServcie) : base(mapper, db)
        {
            _config = config;
            _mapper = mapper;
            _db = db;
            _otpService = otpService;
            _smsServcie = smsServcie;
        }

        public async Task<LoginResultDto> Login(LoginDto input)
        {
            var user = await _db.Users.Include(new[] { "Roles.Role" })
                 .FirstOrDefaultAsync(a => a.Mobile == input.UserName);

            if (user == null)
                throw new ApiException(AppErrors.UserNotFound, 400);

            if (!user.UserState)
                throw new ApiException(AppErrors.UserDeactive, 400);

            //if (!BCrypt.Net.BCrypt.Verify(input.Password, user.Password))
            //    throw new ApiException(AppErrors.WrongPassword);
            await SendCode(user.Mobile);
            //SendMail(user.Email);
            var response = new LoginResultDto
            {
                JwtToken = new JwtToken()
                {
                    Token = GenerateJwtToken(user),
                },
                IsAdmin = user.IsAdmin,
                UserName = user.Mobile,
                FirstName = $"{user.FirstName} ",
                LastName = $"{user.LastName} ",
                Id = user.Id,
            };
            return response;
        }

        //public async Task AddComplexToUser(AddComplexToUser input)
        //{
        //    var entity = _mapper.Map<ComplexUser>(input);
        //    _db.ComplexUsers.Add(entity);
        //    await _db.SaveChangesAsync();
        //}

        public override async Task UpdateAsync(int id, UpdateUserInput input, params string[] include)
        {
            try
            {
                include = new[] {"Roles" };
                var user = await _db.Users.Include(include).FirstOrDefaultAsync(a => a.Id == id);
                if (user == null)
                    throw new ApiException(AppErrors.UserNotFound);

                var map = _mapper.Map<User>(input);
                map.Id = id;

                //if (!string.IsNullOrEmpty(input.Password) && input.Password != "null")
                //    map.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
                //else
                //    map.Password = user.Password;

                if (!string.IsNullOrEmpty(map.Avatar))
                    map.Avatar = input.Avatar;
                else
                    map.Avatar = user.Avatar;

               
                _db.Users.Update(map);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task InsertAsync(CreateUserInput input)
        {
            var user = await _db.Users.AnyAsync(a => a.Mobile == input.Mobile);
            if (user)
                throw new ApiException(AppErrors.UserAlreadyExists);

            var map = _mapper.Map<CreateUserInput, User>(input);

            
            map.Roles = new List<UserRole>();
            if (input.IsAdmin)
            {
                map.Roles.Add(new UserRole
                {
                    RoleId = _db.Roles.First(a => a.Title == Policies.Admin).Id
                });
            }
            map.Roles.Add(new UserRole
            {
                RoleId = _db.Roles.First(a => a.Title == Policies.User).Id
            });
            //map.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
         
            await _db.AddAsync(map);
            await _db.SaveChangesAsync();
        }

        public async Task ChangeState(int id, string fullName)
        {
            var user = _db.Users.FirstOrDefault(a => a.Id == id);
            user.UserState = !user.UserState;
            _db.Update(user);

            var stateString = user.UserState ? "فعال" : "غیر فعال";
         
            await _db.SaveChangesAsync();
        }

        public override IQueryable<User> Filter(UserFilterInput filter)
        {
            var query = _db.Users.AsQueryable();
           
            if (!filter.FirstName.IsNullOrEmpty())
                query = query.Where(a => a.FirstName.Contains(filter.FirstName));

            if (!filter.LastName.IsNullOrEmpty())
                query = query.Where(a => a.LastName.Contains(filter.LastName));

            if (!filter.Mobile.IsNullOrEmpty())
                query = query.Where(a => a.Mobile.Contains(filter.Mobile));

            if (filter.UserState != null)
                query = query.Where(a => a.UserState == filter.UserState);


            return query;
        }

        public async Task<GetUserOutput> GetUserByMobile(string mobile)
        {
            var user = await _db.Users.FirstOrDefaultAsync(a => a.Mobile == mobile);
            return _mapper.Map<GetUserOutput>(user);
        }

        public void SendMail(string mail)
        {
            var otpCode = _otpService.GetCode(mail);
            //_smsServcie.SendEmail(otpCode, mail);
        }
        public async Task SendCode(string mobile)
        {
            if (_otpService.Sandbox) return;

            var otpKey = mobile.TrimStart(new[] { '0' });
            var otpCode = _otpService.GetCode(otpKey);
            await _smsServcie.SendAsync(new RahyabSendSmsReques { message = otpCode, destinationAddress = mobile });
        }

        public void VerifyCode(string code, string mobile)
        {
            try
            {
                _otpService.VerifyCode(mobile, code);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex);
            }
        }

        public async Task ChangePasswordAsync(string mobile, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(a => a.Mobile == mobile);

            //user.Password = BCrypt.Net.BCrypt.HashPassword(password);
            _db.Update(user);
            await _db.SaveChangesAsync();
        }

  
     

        public async Task<IEnumerable<OptionItem>> GetSelectList(string input)
        {
            var users = await _db.Users.Where(a => a.FirstName.Contains(input) || a.LastName.Contains(input)).Select(a => new OptionItem
            {
                Id = a.Id,
                Text = a.FirstName
            }).ToListAsync();

            return users;
        }


   

        public async Task IsDelete(int id, string fullName)
        {
            var user = await _db.Users.FirstAsync(a => a.Id == id);
            user.IsDeleted = true;
            _db.Update(user);
          
            await _db.SaveChangesAsync();
        }


        private string GenerateJwtToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"])); ;
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Mobile),
                new Claim("fullName", $"{userInfo.FirstName} {userInfo.LastName}"),
                new Claim("UserId", userInfo.Id.ToString()),
                new Claim("IsAdmin", userInfo.IsAdmin.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            if (userInfo.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, Policies.Admin));
            }
            claims.AddRange(userInfo.Roles.Select(role => new Claim(ClaimTypes.Role, role.Role.Title)));

            var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(365),
            signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task SetAccessKey(int id,string accessUrl)
        {
            var user =await _db.Users.FirstOrDefaultAsync(a => a.Id == id);
            user.AccessUrl = accessUrl;
            _db.Update(user);
            _db.SaveChanges();
        }

        public async Task UpdateConsumedTraffic(double cunsumedTraffic, int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(a => a.Id == userId);
            user.CunsumedTraffic = cunsumedTraffic;
            _db.Update(user);
            _db.SaveChanges();
        }
    }
}