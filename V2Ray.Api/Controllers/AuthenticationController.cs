using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using V2Ray.Api.Services.UserServices.Dto;
using System.Text.RegularExpressions;
using V2Ray.Api.Services.JWT;
using V2Ray.Api.Services.Settings;
using V2Ray.Api.Services.UserServices;
using V2Ray.Api.Services.UserServices.Dto;
using V2Ray.Api.ViewModels;

namespace V2Ray.Api.Controllers
{

    /// <summary>
    /// احراز هویت 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : CustomBaseController
    {
        private readonly IUserService _service;

        private readonly ILogger<AuthenticationController> _logger;

        private readonly IJwtAuthManager _jwtAuthManager;

        private readonly IOptions<JwtConfig> _jwtTokenConfig;

        public AuthenticationController(IUserService service, IJwtAuthManager jwtAuthManager, IOptions<JwtConfig> jwtTokenConfig, ILogger<AuthenticationController> logger)
        {
            _service = service;
            _jwtAuthManager = jwtAuthManager;
            _jwtTokenConfig = jwtTokenConfig;
            _logger = logger;
        }


        /// <summary>
        /// فرم ورود و دریافت کد تایید 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ApiResponse> Login([FromBody] LoginDto login)
        {
            var result = await _service.Login(login);
            return new ApiResponse(result);
        }


        /// <summary>
        /// دریافت اطلاعات کاربری که لاگین کرده 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("me")]
        public async Task<ApiResponse> GetCurrentUser()
        {
            var result = new GetUserOutput()
            {
                FirstName = "Mostafa",
                LastName = "Kazemi"
            };
            return new ApiResponse(result);
        }
        /// <summary>
        /// دریافت کد تایید از طریق پیامک
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("get-code/{mobile}")]
        public async Task<ApiResponse> GetCode([FromRoute] string mobile)
        {
            try
            {


                if (!Regex.IsMatch(mobile, @"^09[0-9]{9}$"))
                    throw new ApiException("شماره وارد شده صحیح نیست");

                var result = await _service.GetUserByMobile(mobile);
                if (result == null)
                    throw new ApiException("چنین کاربری یافت نشد");
                await _service.SendCode(mobile);
                _service.SendMail(result.Email);

                return new ApiResponse();
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        /// <summary>
        /// ارسال کد تایید و دریافت توکن دسترسی
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        [HttpPost("verify-code")]
        public ApiResponse VerifyCode([FromBody] VerifyCodeViewModel model)
        {
            model.Mobile = model.Mobile.TrimStart(new[] { '0' });

            if (!Regex.IsMatch(model.Mobile, @"^9[0-9]{9}$"))
                throw new ApiException("شماره وارد شده صحیح نیست");

            _service.VerifyCode(model.Code, model.Mobile);

            return new ApiResponse();
        }


        /// <summary>
        /// تغییر رمز عبور کاربر
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ///
        /// 
        [HttpPut("change-password/{mobile}")]
        public async Task<ApiResponse> ChangePassword([FromRoute] string mobile, [FromBody] ChangePasswordViewModel input)
        {
            await _service.ChangePasswordAsync(mobile, input.Password);
            return new ApiResponse();
        }

        /// <summary>
        /// خروج کاربر و حذف توکن
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ///
        /// 

        [HttpPost("logout")]
        [Authorize]
        public ApiResponse Logout()
        {
            // optionally "revoke" JWT token on the server side --> add the current token to a
            // block-list https://github.com/auth0/node-jsonwebtoken/issues/375
            try
            {
                var userName = User.Identity.Name;
                _jwtAuthManager.RemoveRefreshTokenByUserName(userName);
                _logger.LogInformation($"User [{userName}] logged out the ");
            }
            catch (Exception e)
            {
                throw new ApiException(e.Message);
            }
            return new ApiResponse();
        }

        /// <summary>
        /// دریافت رفرش توکن
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ///
        /// 

        [HttpPost("refresh")]
        public async Task<ApiResponse> RefreshToken()
        {
            var refreshToken = _jwtAuthManager.Refresh();
            return new ApiResponse(refreshToken);
        }

        /// <summary>
        /// ارسال رفرش توکن و دریافت توکن جدید
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ///
        /// 
        [HttpPost("refresh-token")]
        public async Task<ApiResponse> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return new ApiResponse(Unauthorized());
                }

                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                var jwtResult = _jwtAuthManager.Refresh(request.RefreshToken, accessToken);

                if (jwtResult == null)
                    return new ApiResponse(Unauthorized());

                var result = new LoginResultDto
                {
                    JwtToken = new JwtToken
                    {
                        Token = jwtResult.AccessToken,
                        Expire = DateTime.Now.AddMinutes(_jwtTokenConfig.Value.AccessTokenExpiration)
                    },
                    RefreshToken = new RefreshToken
                    {
                        TokenString = jwtResult.RefreshToken.TokenString,
                        ExpireAt = DateTime.Now.AddMinutes(_jwtTokenConfig.Value.RefreshTokenExpiration)
                    }
                };
                return new ApiResponse(result);
            }
            catch (SecurityTokenException e)
            {
                throw new ApiException(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}