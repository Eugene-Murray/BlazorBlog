using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EM.CMS.Auth.SQLite.API.Models;
using EM.CMS.Auth.SQLite.API.Models.UserManagement;

namespace EM.CMS.Auth.SQLite.API.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Add index for soft delete queries
        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.IsDeleted);

        // Global query filter to exclude soft-deleted users by default
        builder.Entity<ApplicationUser>()
            .HasQueryFilter(u => !u.IsDeleted);
    }
}
