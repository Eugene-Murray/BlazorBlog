using Microsoft.AspNetCore.Identity;

namespace EM.CMS.Auth.SqlServer.API.Models.UserManagement
{
    public class ApplicationUser : IdentityUser
    {
        // Add custom properties here if needed
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; } 
        public string? DeletedBy { get; set; }
    }
}