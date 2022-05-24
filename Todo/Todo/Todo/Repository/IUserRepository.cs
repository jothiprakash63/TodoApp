using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TodoWebAPI.Models;

namespace Todo.Repository
{
    public interface IUserRepository
    {
        [Post("/api/Users/Login")]
        Task<User> LoginAsync(User user);
    }
}
