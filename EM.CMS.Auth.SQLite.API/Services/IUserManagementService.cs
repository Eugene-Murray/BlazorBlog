using Microsoft.AspNetCore.Identity;
using EM.CMS.Auth.SQLite.API.Models.UserManagement;

namespace EM.CMS.Auth.SQLite.API.Services;

public interface IUserManagementService
{
    // User operations
    Task<IEnumerable<UserDto>> GetAllUsersAsync(bool includeDeleted = false);
    Task<UserDetailDto?> GetUserByIdAsync(string userId);
    Task<IdentityResult> CreateUserAsync(CreateUserDto dto);
    Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto dto);
    Task<IdentityResult> SoftDeleteUserAsync(string userId);
    Task<IdentityResult> RestoreUserAsync(string userId);

    // Role operations
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<IdentityResult> CreateRoleAsync(string roleName);
    Task<IdentityResult> DeleteRoleAsync(string roleId);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
    Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName);
    Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName);

    // Claims operations
    Task<IEnumerable<ClaimDto>> GetUserClaimsAsync(string userId);
    Task<IdentityResult> AddUserClaimAsync(string userId, AddClaimDto dto);
    Task<IdentityResult> RemoveUserClaimAsync(string userId, string claimType, string claimValue);
    Task<IdentityResult> UpdateUserClaimAsync(string userId, UpdateClaimDto dto);

    // Login operations
    Task<IEnumerable<UserLoginDto>> GetUserLoginsAsync(string userId);
    Task<IdentityResult> RemoveUserLoginAsync(string userId, string loginProvider, string providerKey);
}