using Microsoft.AspNetCore.Identity;

namespace TodoWebAPI.Models.Entities
{
    public class AppUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string PictureURL { get; set; }
    }
}
