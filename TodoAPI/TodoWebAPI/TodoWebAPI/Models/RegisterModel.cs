using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoWebAPI.Models
{
    public class RegisterModel
    {
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}
