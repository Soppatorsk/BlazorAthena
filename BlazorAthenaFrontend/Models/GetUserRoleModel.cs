using System.Security.Permissions;

namespace BlazorAthenaFrontend.Models
{
    public class GetUserRoleModel
    {
        public string email { get; set; }
        public string role { get; set; }
        public string userId { get; set; }
    }
}
