using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleExperimentsApp.Experiments
{
    #region Models

    public class Blog
    {
        [Key]
        public int BlogId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        // Navigation property
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }

    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;

        public bool IsPublished { get; set; }

        public int ViewCount { get; set; }

        // Foreign Key
        public int BlogId { get; set; }

        // Navigation property
        [ForeignKey(nameof(BlogId))]
        public Blog Blog { get; set; } = null!;

        // Navigation property
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }

    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int PostId { get; set; }

        // Navigation property
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; } = null!;
    }

    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }

    // Many-to-Many relationship
    public class PostTag
    {
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }

    #endregion

    #region DbContext

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<PostTag> PostTags { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "blogging.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            // Enable sensitive data logging for development
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship
            modelBuilder.Entity<PostTag>()
                .HasKey(pt => new { pt.PostId, pt.TagId });

            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(pt => pt.PostId);

            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags)
                .HasForeignKey(pt => pt.TagId);

            // Configure cascade delete
            modelBuilder.Entity<Blog>()
                .HasMany(b => b.Posts)
                .WithOne(p => p.Blog)
                .HasForeignKey(p => p.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            modelBuilder.Entity<Blog>()
                .HasIndex(b => b.Name)
                .IsUnique();

            modelBuilder.Entity<Post>()
                .HasIndex(p => p.PublishedDate);

            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // Seed data
            modelBuilder.Entity<Blog>().HasData(
                new Blog { BlogId = 1, Name = "Tech Blog", Description = "All about technology", CreatedDate = DateTime.UtcNow },
                new Blog { BlogId = 2, Name = "Travel Blog", Description = "Travel stories and tips", CreatedDate = DateTime.UtcNow }
            );

            modelBuilder.Entity<Tag>().HasData(
                new Tag { TagId = 1, Name = "CSharp" },
                new Tag { TagId = 2, Name = "EntityFramework" },
                new Tag { TagId = 3, Name = "SQLite" },
                new Tag { TagId = 4, Name = "Tutorial" }
            );
        }
    }

    #endregion

    public static class EntityFrameworkExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Entity Framework SQLite Experiments ===\n");
            Console.ResetColor();

            try
            {
                // Initialize database
                await InitializeDatabase();

                // Run experiments
                await CreateOperations();
                Console.WriteLine();

                await ReadOperations();
                Console.WriteLine();

                await UpdateOperations();
                Console.WriteLine();

                await QueryOperations();
                Console.WriteLine();

                await RelationshipOperations();
                Console.WriteLine();

                await TrackingOperations();
                Console.WriteLine();

                await TransactionOperations();
                Console.WriteLine();

                await RawSqlOperations();
                Console.WriteLine();

                // Cleanup option
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Do you want to delete the database? (y/n): ");
                Console.ResetColor();
                var response = Console.ReadLine();
                if (response?.ToLower() == "y")
                {
                    await CleanupDatabase();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        private static async Task InitializeDatabase()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1. Initializing Database");
            Console.ResetColor();

            await using var context = new BloggingContext();

            // Delete existing database
            await context.Database.EnsureDeletedAsync();

            // Create database and apply schema
            await context.Database.EnsureCreatedAsync();

            Console.WriteLine("   ✓ Database created successfully");
            Console.WriteLine($"   ✓ Database location: {Path.Combine(AppContext.BaseDirectory, "blogging.db")}");
        }

        private static async Task CreateOperations()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("2. CREATE Operations");
            Console.ResetColor();

            await using var context = new BloggingContext();

            // Create a new post for existing blog
            var post1 = new Post
            {
                Title = "Getting Started with Entity Framework Core",
                Content = "Entity Framework Core is a modern object-database mapper for .NET.",
                IsPublished = true,
                BlogId = 1,
                PublishedDate = DateTime.UtcNow
            };

            var post2 = new Post
            {
                Title = "SQLite with EF Core",
                Content = "Learn how to use SQLite with Entity Framework Core.",
                IsPublished = true,
                BlogId = 1,
                PublishedDate = DateTime.UtcNow
            };

            context.Posts.AddRange(post1, post2);
            await context.SaveChangesAsync();

            Console.WriteLine($"   ✓ Created {context.Posts.Local.Count} posts");
            Console.WriteLine($"   ✓ Post IDs: {string.Join(", ", context.Posts.Local.Select(p => p.PostId))}");
        }

        private static async Task ReadOperations()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("3. READ Operations");
            Console.ResetColor();

            await using var context = new BloggingContext();

            // Read all blogs
            var blogs = await context.Blogs.ToListAsync();
            Console.WriteLine($"   ✓ Total Blogs: {blogs.Count}");

            // Read with Include (eager loading)
            var blogsWithPosts = await context.Blogs
                .Include(b => b.Posts)
                .ToListAsync();

            Console.WriteLine("   ✓ Blogs with Posts:");
            foreach (var blog in blogsWithPosts)
            {
                Console.WriteLine($"      - {blog.Name}: {blog.Posts.Count} posts");
            }

            // Find by ID
            var blog1 = await context.Blogs.FindAsync(1);
            Console.WriteLine($"   ✓ Found blog by ID: {blog1?.Name}");

            // Single or default
            var techBlog = await context.Blogs
                .SingleOrDefaultAsync(b => b.Name == "Tech Blog");
            Console.WriteLine($"   ✓ Found blog by name: {techBlog?.Name}");
        }

        private static async Task UpdateOperations()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("4. UPDATE Operations");
            Console.ResetColor();

            await using var context = new BloggingContext();

            // Update tracked entity
            var post = await context.Posts.FirstAsync();
            var oldTitle = post.Title;
            post.Title = post.Title + " (Updated)";
            post.ViewCount += 10;

            await context.SaveChangesAsync();
            Console.WriteLine($"   ✓ Updated post title: '{oldTitle}' → '{post.Title}'");
            Console.WriteLine($"   ✓ Updated view count: {post.ViewCount}");

            // Update without tracking (ExecuteUpdate - EF Core 7+)
            var rowsAffected = await context.Posts
                .Where(p => p.IsPublished)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.ViewCount, p => p.ViewCount + 1));

            Console.WriteLine($"   ✓ Incremented view count for {rowsAffected} published posts");
        }

        private static async Task QueryOperations()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("5. Advanced QUERY Operations");
            Console.ResetColor();

            await using var context = new BloggingContext();

            // Where clause
            var publishedPosts = await context.Posts
                .Where(p => p.IsPublished)
                .ToListAsync();
            Console.WriteLine($"   ✓ Published posts: {publishedPosts.Count}");

            // OrderBy
            var recentPosts = await context.Posts
                .OrderByDescending(p => p.PublishedDate)
                .Take(5)
                .ToListAsync();
            Console.WriteLine($"   ✓ Recent posts: {recentPosts.Count}");

            // Projection (Select)
            var postTitles = await context.Posts
                .Select(p => new { p.PostId, p.Title, p.ViewCount })
                .ToListAsync();
            Console.WriteLine($"   ✓ Post projections:");
            foreach (var item in postTitles)
            {
                Console.WriteLine($"      - [{item.PostId}] {item.Title} (Views: {item.ViewCount})");
            }

            // GroupBy
            var postsByBlog = await context.Posts
                .GroupBy(p => p.BlogId)
                .Select(g => new { BlogId = g.Key, Count = g.Count() })
                .ToListAsync();
            Console.WriteLine($"   ✓ Posts grouped by blog:");
            foreach (var group in postsByBlog)
            {
                Console.WriteLine($"      - Blog {group.BlogId}: {group.Count} posts");
            }

            // Any
            var hasPublishedPosts = await context.Posts.AnyAsync(p => p.IsPublished);
            Console.WriteLine($"   ✓ Has published posts: {hasPublishedPosts}");

            // Count
            var totalPosts = await context.Posts.CountAsync();
            Console.WriteLine($"   ✓ Total posts count: {totalPosts}");

            // Average
            var avgViews = await context.Posts.AverageAsync(p => p.ViewCount);
            Console.WriteLine($"   ✓ Average views per post: {avgViews:F2}");
        }

        private static async Task RelationshipOperations()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("6. RELATIONSHIP Operations");
            Console.ResetColor();

            await using var context = new BloggingContext();

            // Add comments to a post
            var post = await context.Posts.FirstAsync();
            var comments = new List<Comment>
            {
                new Comment { Author = "Alice", Content = "Great article!", PostId = post.PostId },
                new Comment { Author = "Bob", Content = "Very informative, thanks!", PostId = post.PostId },
                new Comment { Author = "Charlie", Content = "Looking forward to more posts.", PostId = post.PostId }
            };

            context.Comments.AddRange(comments);
            await context.SaveChangesAsync();
            Console.WriteLine($"   ✓ Added {comments.Count} comments to post: '{post.Title}'");

            // Create many-to-many relationships (Posts and Tags)
            var posts = await context.Posts.Take(2).ToListAsync();
            var tags = await context.Tags.Take(3).ToListAsync();

            var postTags = new List<PostTag>
            {
                new PostTag { PostId = posts[0].PostId, TagId = tags[0].TagId },
                new PostTag { PostId = posts[0].PostId, TagId = tags[1].TagId },
                new PostTag { PostId = posts[0].PostId, TagId = tags[2].TagId },
                new PostTag { PostId = posts[1].PostId, TagId = tags[1].TagId },
                new PostTag { PostId = posts[1].PostId, TagId = tags[2].TagId }
            };

            context.PostTags.AddRange(postTags);
            await context.SaveChangesAsync();
            Console.WriteLine($"   ✓ Created {postTags.Count} post-tag relationships");

            // Load related data (Explicit loading)
            var blogToLoad = await context.Blogs.FirstAsync();
            await context.Entry(blogToLoad).Collection(b => b.Posts).LoadAsync();
            Console.WriteLine($"   ✓ Explicitly loaded {blogToLoad.Posts.Count} posts for blog: '{blogToLoad.Name}'");

            // Navigation property query
            var postsWithTags = await context.Posts
                .Include(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .Include(p => p.Comments)
                .ToListAsync();

            Console.WriteLine($"   ✓ Posts with tags and comments:");
            foreach (var p in postsWithTags)
            {
                Console.WriteLine($"      - {p.Title}");
                Console.WriteLine($"        Tags: {string.Join(", ", p.PostTags.Select(pt => pt.Tag.Name))}");
                Console.WriteLine($"        Comments: {p.Comments.Count}");
            }
        }

        private static async Task TrackingOperations()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("7. TRACKING Operations");
            Console.ResetColor();

            // Tracked query (default)
            await using (var context = new BloggingContext())
            {
                var trackedPost = await context.Posts.FirstAsync();
                Console.WriteLine($"   ✓ Tracked entity state: {context.Entry(trackedPost).State}");

                trackedPost.ViewCount += 5;
                Console.WriteLine($"   ✓ After modification: {context.Entry(trackedPost).State}");

                await context.SaveChangesAsync();
                Console.WriteLine($"   ✓ After save: {context.Entry(trackedPost).State}");
            }

            // No-tracking query (read-only, better performance)
            await using (var context = new BloggingContext())
            {
                var untrackedPosts = await context.Posts
                    .AsNoTracking()
                    .ToListAsync();
                Console.WriteLine($"   ✓ Untracked query returned {untrackedPosts.Count} posts");
            }

            // Attach and update - using a new context to avoid tracking conflicts
            await using (var context = new BloggingContext())
            {
                var detachedPost = new Post { PostId = 1, Title = "Updated Title via Attach", ViewCount = 200 };
                context.Attach(detachedPost);
                context.Entry(detachedPost).Property(p => p.Title).IsModified = true;
                context.Entry(detachedPost).Property(p => p.ViewCount).IsModified = true;

                await context.SaveChangesAsync();
                Console.WriteLine($"   ✓ Updated detached entity using Attach: {detachedPost.Title}");
            }

            // Alternative: Clear change tracker before attaching
            await using (var context = new BloggingContext())
            {
                var post = await context.Posts.FirstAsync();
                Console.WriteLine($"   ✓ Post loaded and tracked: {post.Title}");

                // Clear all tracked entities
                context.ChangeTracker.Clear();
                Console.WriteLine($"   ✓ Change tracker cleared");

                // Now we can attach an entity with the same key
                var updatedPost = new Post { PostId = post.PostId, Title = "Updated after clearing tracker", ViewCount = 300 };
                context.Attach(updatedPost);
                context.Entry(updatedPost).Property(p => p.Title).IsModified = true;
                context.Entry(updatedPost).Property(p => p.ViewCount).IsModified = true;

                await context.SaveChangesAsync();
                Console.WriteLine($"   ✓ Updated after clearing tracker: {updatedPost.Title}");
            }
        }

        private static async Task TransactionOperations()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("8. TRANSACTION Operations");
            Console.ResetColor();

            await using var context = new BloggingContext();

            // Manual transaction
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var newBlog = new Blog
                {
                    Name = "Transaction Test Blog",
                    Description = "Created in a transaction"
                };
                context.Blogs.Add(newBlog);
                await context.SaveChangesAsync();

                var newPost = new Post
                {
                    Title = "Transaction Test Post",
                    Content = "This post is part of a transaction",
                    IsPublished = true,
                    BlogId = newBlog.BlogId
                };
                context.Posts.Add(newPost);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();
                Console.WriteLine($"   ✓ Transaction committed successfully");
                Console.WriteLine($"   ✓ Created blog '{newBlog.Name}' and post '{newPost.Title}'");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"   ✗ Transaction rolled back");
                throw;
            }

            // Savepoint example
            await using var transaction2 = await context.Database.BeginTransactionAsync();
            try
            {
                var blog = new Blog { Name = "Savepoint Blog", Description = "Testing savepoints" };
                context.Blogs.Add(blog);
                await context.SaveChangesAsync();

                await transaction2.CreateSavepointAsync("AfterBlogCreation");

                try
                {
                    // This might fail
                    var post = new Post { Title = "Savepoint Post", Content = "Test", BlogId = blog.BlogId, IsPublished = true };
                    context.Posts.Add(post);
                    await context.SaveChangesAsync();
                }
                catch
                {
                    await transaction2.RollbackToSavepointAsync("AfterBlogCreation");
                    Console.WriteLine($"   ✓ Rolled back to savepoint");
                }

                await transaction2.CommitAsync();
                Console.WriteLine($"   ✓ Transaction with savepoint completed");
            }
            catch (Exception)
            {
                await transaction2.RollbackAsync();
                throw;
            }
        }

        private static async Task RawSqlOperations()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("9. RAW SQL Operations");
            Console.ResetColor();

            await using var context = new BloggingContext();

            // Raw SQL query
            var blogName = "Tech Blog";
            var blogs = await context.Blogs
                .FromSqlRaw("SELECT * FROM Blogs WHERE Name = {0}", blogName)
                .ToListAsync();
            Console.WriteLine($"   ✓ Raw SQL query returned {blogs.Count} blogs");

            // Raw SQL for non-entity query
            var postCount = await context.Database
                .SqlQueryRaw<int>("SELECT COUNT(*) as Value FROM Posts")
                .FirstAsync();
            Console.WriteLine($"   ✓ Raw SQL count: {postCount} posts");

            // Execute non-query SQL
            var rowsAffected = await context.Database
                .ExecuteSqlRawAsync("UPDATE Posts SET ViewCount = ViewCount + 1 WHERE IsPublished = 1");
            Console.WriteLine($"   ✓ Raw SQL update affected {rowsAffected} rows");

            // Stored procedure simulation (SQLite doesn't support stored procedures natively)
            var recentPosts = await context.Posts
                .FromSqlRaw(@"
                    SELECT * FROM Posts 
                    WHERE PublishedDate >= datetime('now', '-30 days')
                    ORDER BY PublishedDate DESC")
                .ToListAsync();
            Console.WriteLine($"   ✓ Recent posts from raw SQL: {recentPosts.Count}");
        }

        private static async Task CleanupDatabase()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n10. Cleaning up database...");
            Console.ResetColor();

            await using var context = new BloggingContext();
            await context.Database.EnsureDeletedAsync();

            var dbPath = Path.Combine(AppContext.BaseDirectory, "blogging.db");
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }

            Console.WriteLine("   ✓ Database deleted successfully");
        }
    }
}
