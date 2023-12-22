using System.ComponentModel.DataAnnotations;

namespace AthenaResturantWebAPI.Models
{
    
    public class GetUserRoleModels
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Roles are required.")]
        public List<string> Roles { get; set; }
    }
}
