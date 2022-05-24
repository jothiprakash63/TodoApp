using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public List<Task> Tasks { get; set; }
    }
}
