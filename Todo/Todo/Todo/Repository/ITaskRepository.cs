using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Repository
{
    public interface ITaskRepository
    {
        [Post("/api/Users/Login")]
        Task<UserWithToken> LoginAsync(User user);
    }
}
