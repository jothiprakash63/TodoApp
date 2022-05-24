using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Shared
{
    public  class TaskResponse
    {
        public IEnumerable<TaskModel> Tasks { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public DateTime? ExpireDate { get; set; }
    }

    public class TaskModel
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDesc { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CompletedOn { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}
