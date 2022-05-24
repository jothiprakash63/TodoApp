using AspNetIdentityDemo.Shared;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Services
{
    public interface IAuthService
    {
        [Post("/auth/login")]
        Task<UserManagerResponse> LoginAsync(LoginRequest user);

        [Post("/auth/register")]
        Task<UserManagerResponse> RegisterAsync(RegisterRequest user);

        [Post("/auth/{scheme}")]
        Task<UserManagerResponse> SignInOauthAsync(string scheme);
    }
}
