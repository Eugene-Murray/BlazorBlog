using System.ComponentModel.DataAnnotations;

namespace BlazorBlog.Data
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Excerpt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsPublished { get; set; }

        // Navigation properties
        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser Author { get; set; } = null!;
    }
}