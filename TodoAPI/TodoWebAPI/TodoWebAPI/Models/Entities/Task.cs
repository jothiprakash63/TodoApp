using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoWebAPI.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDesc { get; set; }
        public DateTime CreatedOn { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CompletedOn { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}
