using Microsoft.AspNetCore.Identity;

namespace BlazorBlog.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public bool IsAdmin { get; set; }

        // Navigation property for blog posts
        public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }

}
