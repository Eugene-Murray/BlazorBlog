using Microsoft.EntityFrameworkCore;
using BlazorBlog.Data;

namespace BlazorBlog.Services
{
    public class BlogService
    {
        private readonly ApplicationDbContext _context;

        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BlogPost>> GetPublishedBlogPostsAsync()
        {
            return await _context.BlogPosts
                .Include(bp => bp.Author)
                .Where(bp => bp.IsPublished)
                .OrderByDescending(bp => bp.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetAllBlogPostsAsync()
        {
            return await _context.BlogPosts
                .Include(bp => bp.Author)
                .OrderByDescending(bp => bp.CreatedAt)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetBlogPostByIdAsync(int id)
        {
            return await _context.BlogPosts
                .Include(bp => bp.Author)
                .FirstOrDefaultAsync(bp => bp.Id == id);
        }

        public async Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost)
        {
            blogPost.CreatedAt = DateTime.UtcNow;
            blogPost.UpdatedAt = DateTime.UtcNow;
            
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost> UpdateBlogPostAsync(BlogPost blogPost)
        {
            blogPost.UpdatedAt = DateTime.UtcNow;
            
            _context.BlogPosts.Update(blogPost);
            await _context.SaveChangesAsync();
            return blogPost;
        }

        public async Task DeleteBlogPostAsync(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
                await _context.SaveChangesAsync();
            }
        }
    }
}