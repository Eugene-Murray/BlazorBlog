using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EM.CMS.Auth.SqlServer.API.Data;
using EM.CMS.Auth.SqlServer.API.Models;
using EM.CMS.Auth.SqlServer.API.Models.UserManagement;

namespace EM.CMS.Auth.SqlServer.API.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    #region User Operations

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(bool includeDeleted = false)
    {
        var query = _context.Users.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        var users = await query.ToListAsync();

        return users.Select(u => new UserDto(
            u.Id,
            u.UserName,
            u.Email,
            u.EmailConfirmed,
            u.PhoneNumber,
            u.PhoneNumberConfirmed,
            u.TwoFactorEnabled,
            u.LockoutEnd,
            u.LockoutEnabled,
            u.AccessFailedCount,
            u.IsDeleted
        ));
    }

    public async Task<UserDetailDto?> GetUserByIdAsync(string userId)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        var logins = await _userManager.GetLoginsAsync(user);

        return new UserDetailDto(
            user.Id,
            user.UserName,
            user.Email,
            user.EmailConfirmed,
            user.PhoneNumber,
            user.PhoneNumberConfirmed,
            user.TwoFactorEnabled,
            user.LockoutEnd,
            user.LockoutEnabled,
            user.AccessFailedCount,
            user.IsDeleted,
            roles,
            claims.Select(c => new ClaimDto(c.Type, c.Value)),
            logins.Select(l => new UserLoginDto(l.LoginProvider, l.ProviderDisplayName ?? l.LoginProvider))
        );
    }

    public async Task<IdentityResult> CreateUserAsync(CreateUserDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            IsDeleted = false
        };

        return await _userManager.CreateAsync(user, dto.Password);
    }

    public async Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto dto)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        if (dto.UserName is not null) user.UserName = dto.UserName;
        if (dto.Email is not null) user.Email = dto.Email;
        if (dto.PhoneNumber is not null) user.PhoneNumber = dto.PhoneNumber;
        if (dto.LockoutEnabled.HasValue) user.LockoutEnabled = dto.LockoutEnabled.Value;
        if (dto.TwoFactorEnabled.HasValue) user.TwoFactorEnabled = dto.TwoFactorEnabled.Value;

        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> SoftDeleteUserAsync(string userId)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;

        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> RestoreUserAsync(string userId)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        user.IsDeleted = false;
        user.DeletedAt = null;
        user.DeletedBy = null;

        return await _userManager.UpdateAsync(user);
    }

    #endregion

    #region Role Operations

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return roles.Select(r => new RoleDto(r.Id, r.Name, r.NormalizedName));
    }

    public async Task<IdentityResult> CreateRoleAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' already exists." });
        }

        return await _roleManager.CreateAsync(new IdentityRole(roleName));
    }

    public async Task<IdentityResult> DeleteRoleAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role is null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
        }

        return await _roleManager.DeleteAsync(role);
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return [];

        return await _userManager.GetRolesAsync(user);
    }

    public async Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' does not exist." });
        }

        return await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        return await _userManager.RemoveFromRoleAsync(user, roleName);
    }

    #endregion

    #region Claims Operations

    public async Task<IEnumerable<ClaimDto>> GetUserClaimsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return [];

        var claims = await _userManager.GetClaimsAsync(user);
        return claims.Select(c => new ClaimDto(c.Type, c.Value));
    }

    public async Task<IdentityResult> AddUserClaimAsync(string userId, AddClaimDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        var claim = new Claim(dto.Type, dto.Value);
        return await _userManager.AddClaimAsync(user, claim);
    }

    public async Task<IdentityResult> RemoveUserClaimAsync(string userId, string claimType, string claimValue)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        var claim = new Claim(claimType, claimValue);
        return await _userManager.RemoveClaimAsync(user, claim);
    }

    public async Task<IdentityResult> UpdateUserClaimAsync(string userId, UpdateClaimDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        var oldClaim = new Claim(dto.OldType, dto.OldValue);
        var newClaim = new Claim(dto.NewType, dto.NewValue);

        return await _userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
    }

    #endregion

    #region Login Operations

    public async Task<IEnumerable<UserLoginDto>> GetUserLoginsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return [];

        var logins = await _userManager.GetLoginsAsync(user);
        return logins.Select(l => new UserLoginDto(l.LoginProvider, l.ProviderDisplayName ?? l.LoginProvider));
    }

    public async Task<IdentityResult> RemoveUserLoginAsync(string userId, string loginProvider, string providerKey)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        return await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
    }

    #endregion
}