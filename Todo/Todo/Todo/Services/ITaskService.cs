using AspNetIdentityDemo.Shared;
using Refit;
using System.Threading.Tasks;
using Todo.Shared;

namespace Todo.Services
{
    public interface ITaskService
    {
        [Get("/tasks")]
        [Headers("Authorization: Bearer")]
        Task<TaskResponse> GetTasksOfUser();
    }
}
