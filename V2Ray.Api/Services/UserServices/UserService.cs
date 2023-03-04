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
using V2Ray.Api.Services.UserServices.Dto;
using V2Ray.Api.Services.OTP;
using V2Ray.Api.Services.sms;
using V2Ray.Api.Shared;
using V2Ray.Api.Database;
using V2Ray.Api.Entity;
using V2Ray.Api.Extensions;
using V2Ray.Api.Common;
using V2Ray.Api.Services.sms.Rahyab;
using V2Ray.Api.Services.V2Keys;
using Newtonsoft.Json;

namespace V2Ray.Api.Services.UserServices
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
           //await RecaptchaResult(input.LoginToken);

            var user = await _db.Users.Include(new[] { "Roles.Role" })
                 .FirstOrDefaultAsync(a => a.Email == input.Email);

            if (user == null)
                throw new ApiException(AppErrors.UserNotFound, 400);

            if (!user.Enable)
                throw new ApiException(AppErrors.UserDeactive, 400);

            if (!BCrypt.Net.BCrypt.Verify(input.Password, user.Password))
                throw new ApiException(AppErrors.WrongPassword);

            if (user.NeedConfirm)
            {
                SendMail(user.Email);
                return new LoginResultDto
                {
                    NeedConfirm = true
                };
            }

            var response = new LoginResultDto
            {
                JwtToken = new JwtToken()
                {
                    Token = GenerateJwtToken(user),
                },
                NeedConfirm = user.NeedConfirm,
                IsAdmin = user.IsAdmin,
                FreeAccount = user.UsedFreeAccount,
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
                include = new[] { "Roles" };
                var user = await _db.Users.Include(include).FirstOrDefaultAsync(a => a.Id == id);
                if (user == null)
                    throw new ApiException(AppErrors.UserNotFound);

                var map = _mapper.Map<User>(input);
                map.Id = id;
                //if (!string.IsNullOrEmpty(input.Password) && input.Password != "null")
                //    map.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
                //else
                //    map.Password = user.Password;

                //if (!string.IsNullOrEmpty(map.Avatar))
                //    map.Avatar = input.Avatar;
                //else
                //    map.Avatar = user.Avatar;


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
           await  RecaptchaResult(input.LoginToken);
            var user = await _db.Users.AnyAsync(a => a.Email == input.Email);
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
            map.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
            SendMail(input.Email);
            map.NeedConfirm = true;
            await _db.AddAsync(map);
            await _db.SaveChangesAsync();
        }

        private async Task RecaptchaResult(string loginToken)
        {
            var dictionary = new Dictionary<string, string>


                    {


                        { "secret", "6LfYMdwjAAAAAOCmbjG8IWUbhltnlgthqtRytW0O" },


                        { "response", loginToken }


                    };




            var postContent = new FormUrlEncodedContent(dictionary);




            HttpResponseMessage recaptchaResponse = null;


            string stringContent = "";




            // Call recaptcha api and validate the token


            using (var http = new HttpClient())


            {


                recaptchaResponse = await http.PostAsync("https://www.google.com/recaptcha/api/siteverify", postContent);


                stringContent = await recaptchaResponse.Content.ReadAsStringAsync();


            }
            if (string.IsNullOrEmpty(stringContent))
            {
                throw new ApiException("خطا در احراز هویت کپچا");
            }
            var googleReCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(stringContent);

            if (!recaptchaResponse.IsSuccessStatusCode  || !googleReCaptchaResponse.Success)

            {
                throw new ApiException("خطا در احراز هویت کپچا");
            }
        }

        public async Task ChangeState(int id)
        {
            var user = _db.Users.FirstOrDefault(a => a.Id == id);
            

            var stateString = user.Enable ? "فعال" : "غیر فعال";
            user.Enable = !user.Enable;
            _db.Update(user);
            await _db.SaveChangesAsync();
        }

        public override IQueryable<User> Filter(UserFilterInput filter)
        {
            var query = _db.Users.AsQueryable();

            if (!filter.Email.IsNullOrEmpty())
                query = query.Where(a => a.Email.Contains(filter.Email));

            if (filter.Enable != null)
                query = query.Where(a => a.Enable == filter.Enable);


            return query;
        }

        public async Task<GetUserOutput> GetUserByMobile(string mobile)
        {
            var user = await _db.Users.FirstOrDefaultAsync(a => a.Email == mobile);
            return _mapper.Map<GetUserOutput>(user);
        }

        public void SendMail(string mail)
        {
            var otpCode = _otpService.GetCode(mail);
            _smsServcie.SendEmail(otpCode, mail);
        }
        public async Task SendCode(string mobile)
        {
            if (_otpService.Sandbox) return;

            var otpCode = _otpService.GetCode(mobile);
            //await _smsServcie.SendAsync(new RahyabSendSmsReques { message = otpCode, destinationAddress = mobile });
        }

        public async Task<LoginResultDto> VerifyCode(string code, string email)
        {
            _otpService.VerifyCode(email, code);
            var user = await _db.Users.Include(new[] { "Roles.Role" }).FirstOrDefaultAsync(a => a.Email == email);
            var response = new LoginResultDto
            {
                JwtToken = new JwtToken()
                {
                    Token = GenerateJwtToken(user),
                },
                IsAdmin = user.IsAdmin,
                NeedConfirm = user.NeedConfirm,
                FreeAccount = user.UsedFreeAccount,
                FirstName = $"{user.FirstName} ",
                LastName = $"{user.LastName} ",
                Id = user.Id,
            };
            user.NeedConfirm = false;
            _db.Users.Update(user);
            _db.SaveChanges();
            return response;
        }

        public async Task ChangePasswordAsync(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(a => a.Email == email);

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




        public async Task IsDelete(int id)
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
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Email),
                new Claim("fullName", $"{userInfo.FirstName} {userInfo.LastName}"),
                new Claim("UserId", userInfo.Id.ToString()),
                new Claim("FreeAccount", userInfo.UsedFreeAccount.ToString()),
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
            expires: DateTime.Now.AddDays(70),
            signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}