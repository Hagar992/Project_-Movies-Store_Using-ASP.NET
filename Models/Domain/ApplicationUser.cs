using Microsoft.AspNetCore.Identity;

namespace full_Project.Models.Domain
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
    }
}
