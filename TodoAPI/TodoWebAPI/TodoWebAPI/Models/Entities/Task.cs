using System;

namespace TodoWebAPI.Models
{
    public class Task
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
