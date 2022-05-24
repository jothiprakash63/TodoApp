using AspNetIdentityDemo.Api.Services;
using AspNetIdentityDemo.Shared;
using AuthDemoWeb.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodoWebAPI.Models;
using TodoWebAPI.Models.Entities;
using Task = System.Threading.Tasks.Task;

namespace TodoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private IMailService _mailService;
        private IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly FacebookAuthService _facebookAuthService;
        public AuthController(IUserService userService, IMailService mailService, IConfiguration configuration, SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager)
        {
            _userService = userService;
            _mailService = mailService;
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _facebookAuthService = new FacebookAuthService();

        }





        // /api/auth/register
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAsync(model);

                if (result.IsSuccess)
                    return Ok(result); // Status Code: 200 

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid"); // Status code: 400
        }

        // /api/auth/login
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);

                if (result.IsSuccess)
                {
                    //await _mailService.SendEmailAsync(model.Email, "New login", "<h1>Hey!, new login to your account noticed</h1><p>New login to your account at " + DateTime.Now + "</p>");
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }


        // /api/auth/confirmemail?userid&token
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _userService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
            {
                return Redirect($"{_configuration["AppUrl"]}/ConfirmEmail.html");
            }

            return BadRequest(result);
        }

        // api/auth/forgetpassword
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var result = await _userService.ForgetPasswordAsync(email);

            if (result.IsSuccess)
                return Ok(result); // 200

            return BadRequest(result); // 400
        }



        [HttpGet("{scheme}")]
        public async Task MobileAuth([FromRoute] string scheme)
        {
            //NOTE: see https://docs.microsoft.com/en-us/xamarin/essentials/web-authenticator?tabs=android
            var auth = await Request.HttpContext.AuthenticateAsync(scheme);

            if (!auth.Succeeded
                || auth?.Principal == null
                || !auth.Principal.Identities.Any(id => id.IsAuthenticated)
                || string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token")))
            {
                // Not authenticated, challenge
                await Request.HttpContext.ChallengeAsync(scheme);
            }
            else
            {
                var claims = auth.Principal.Identities.FirstOrDefault()?.Claims;

                var email = string.Empty;
                email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
                var givenName = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.GivenName)?.Value;
                var surName = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Surname)?.Value;
                var nameIdentifier = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                string picture = string.Empty;

                if (scheme == "Facebook")
                {
                    picture = await _facebookAuthService.GetFacebookProfilePicURL(auth.Properties.GetTokenValue("access_token"));
                }
                else
                    picture = claims?.FirstOrDefault(c => c.Type == "picture")?.Value;

                var appUser = new AppUser
                {
                    Email = email,
                    FirstName = givenName,
                    SecondName = surName,
                    PictureURL = picture
                };

               var res = await _userService.SignInOauthAsync(appUser);

             
                // Get parameters to send back to the callback
                var qs = new Dictionary<string, string>
                {
                    { "access_token", res.Message },
                    { "refresh_token",  string.Empty },
                    { "jwt_token_expires", res.ExpireDate.ToString() },
                    { "email", email },
                    { "firstName", givenName },
                    { "picture", picture },
                    { "secondName", surName },
                    { "id", res.UserId },
                };

                // Build the result url
                var url = "xamarinapp" + "://#" + string.Join(
                    "&",
                    qs.Where(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value != "-1")
                    .Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

                // Redirect to final url
                Request.HttpContext.Response.Redirect(url);
            }
        }

        // api/auth/resetpassword
        //[HttpPost("ResetPassword")]
        //public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _userService.ResetPasswordAsync(model);

        //        if (result.IsSuccess)
        //            return Ok(result);

        //        return BadRequest(result);
        //    }

        //    return BadRequest("Some properties are not valid");
        //}



    }
}