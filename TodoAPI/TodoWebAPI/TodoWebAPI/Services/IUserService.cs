using AspNetIdentityDemo.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodoWebAPI.Models.Entities;
using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace AspNetIdentityDemo.Api.Services
{
    public interface IUserService
    {

        Task<UserManagerResponse> RegisterUserAsync(RegisterRequest model);

        Task<UserManagerResponse> LoginUserAsync(LoginRequest model);
        Task<UserManagerResponse> LogOutUserAsync(LoginRequest model);

        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);

        Task<UserManagerResponse> ForgetPasswordAsync(string email);
        Task<UserManagerResponse> SignInOauthAsync(AppUser model);

       // Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model);
    }

    public class UserService : IUserService
    {

        private UserManager<AppUser> _userManger;
        private IConfiguration _configuration;
        private IMailService _mailService;
        private SignInManager<AppUser> _signInManager;
        public UserService(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager, IConfiguration configuration, IMailService mailService) 
        {
            _userManger = userManager;
            _configuration = configuration;
            _mailService = mailService;
            _signInManager = signInManager;
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterRequest model)
        {
            if (model == null)
                throw new NullReferenceException("Reigster Model is null");

            if (model.Password != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    Message = "Confirm password doesn't match the password",
                    IsSuccess = false,
                };

            var identityUser = new AppUser
            {
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _userManger.CreateAsync(identityUser, model.Password);

            if(result.Succeeded)
            {
                //var confirmEmailToken = await _userManger.GenerateEmailConfirmationTokenAsync(identityUser);

                //var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                //var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                //string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userid={identityUser.Id}&token={validEmailToken}";

                //await _mailService.SendEmailAsync(identityUser.Email, "Confirm your email", $"<h1>Welcome to Auth Demo</h1>" +
                //    $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");


                return new UserManagerResponse
                {
                    Message = "User created successfully!",
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };

        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginRequest model)
        {
            var user = await _userManger.FindByEmailAsync(model.Email);

            if(user == null)
            {
                return new UserManagerResponse
                {
                    Message = "There is no user with that Email address",
                    IsSuccess = false,
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password,true);

            if(!result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Invalid password",
                    IsSuccess = false,
                };

            
           

            var token = this.GenerateJwtToken(model.Email, user.Id);
            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return new UserManagerResponse
            {
                Message = tokenAsString,
                UserId = user.Id,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }


        public async Task<UserManagerResponse> LogOutUserAsync(LoginRequest model)
        {
           
            await _signInManager.SignOutAsync();

            return new UserManagerResponse()
            {
                 IsSuccess = true,
            };
        }

        public async Task<UserManagerResponse> SignOutAsync(AppUser appUser)
        {


            var user = await _userManger.FindByEmailAsync(appUser.Email);

            if (user == null)
            {
                //Create a username unique
                appUser.UserName = appUser.FirstName + appUser.SecondName;
                var result = await _userManger.CreateAsync(appUser);
                user = appUser;

            }
            

             await _signInManager.SignOutAsync();
          

            var token = this.GenerateJwtToken(user.Email, user.Id);
            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return new UserManagerResponse
            {
                Message = tokenAsString,
                UserId = user.Id,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };


        }

        public async Task<UserManagerResponse> SignInOauthAsync(AppUser appUser)
        {


            var user = await _userManger.FindByEmailAsync(appUser.Email);

            if (user == null)
            {
                //Create a username unique
                appUser.UserName = appUser.FirstName + appUser.SecondName;
                var result = await _userManger.CreateAsync(appUser);
                user = appUser;

            }

            await _signInManager.SignInAsync(user, true);


            var token = this.GenerateJwtToken(user.Email, user.Id);
            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return new UserManagerResponse
            {
                Message = tokenAsString,
                UserId = user.Id,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };


        }




        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManger.FindByIdAsync(userId);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManger.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Email confirmed successfully!",
                    IsSuccess = true,
                };

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Email did not confirm",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManger.FindByEmailAsync(email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email",
                };

            var token = await _userManger.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

            await _mailService.SendEmailAsync(email, "Reset Password", "<h1>Follow the instructions to reset your password</h1>" +
                $"<p>To reset your password <a href='{url}'>Click here</a></p>");

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Reset password URL has been sent to the email successfully!"
            };
        }


        public JwtSecurityToken GenerateJwtToken(string Email,string userId)
        {
            var claims = new[]
            {
                new Claim("Email", Email),
                new Claim(ClaimTypes.NameIdentifier, userId),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

          
           
            return token;
        }

       

        //public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        //{
        //    var user = await _userManger.FindByEmailAsync(model.Email);
        //    if (user == null)
        //        return new UserManagerResponse
        //        {
        //            IsSuccess = false,
        //            Message = "No user associated with email",
        //        };

        //    if(model.NewPassword != model.ConfirmPassword)
        //        return new UserManagerResponse
        //        {
        //            IsSuccess = false,
        //            Message = "Password doesn't match its confirmation",
        //        };

        //    var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
        //    string normalToken = Encoding.UTF8.GetString(decodedToken);

        //    var result = await _userManger.ResetPasswordAsync(user, normalToken, model.NewPassword);

        //    if (result.Succeeded)
        //        return new UserManagerResponse
        //        {
        //            Message = "Password has been reset successfully!",
        //            IsSuccess = true,
        //        };

        //    return new UserManagerResponse
        //    {
        //        Message = "Something went wrong",
        //        IsSuccess = false,
        //        Errors = result.Errors.Select(e => e.Description),
        //    };
        //}
    }
}
