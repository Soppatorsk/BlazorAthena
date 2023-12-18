using Microsoft.AspNetCore.Identity;

namespace BlazorAthenaFrontend.Data.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; }
        
    }
}
