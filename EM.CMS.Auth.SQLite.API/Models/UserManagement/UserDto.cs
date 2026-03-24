namespace EM.CMS.Auth.SQLite.API.Models.UserManagement;

public record UserDto(
    string Id,
    string? UserName,
    string? Email,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool PhoneNumberConfirmed,
    bool TwoFactorEnabled,
    DateTimeOffset? LockoutEnd,
    bool LockoutEnabled,
    int AccessFailedCount,
    bool IsDeleted
);

public record UserDetailDto(
    string Id,
    string? UserName,
    string? Email,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool PhoneNumberConfirmed,
    bool TwoFactorEnabled,
    DateTimeOffset? LockoutEnd,
    bool LockoutEnabled,
    int AccessFailedCount,
    bool IsDeleted,
    IEnumerable<string> Roles,
    IEnumerable<ClaimDto> Claims,
    IEnumerable<UserLoginDto> Logins
);

public record CreateUserDto(
    string UserName,
    string Email,
    string Password,
    string? PhoneNumber = null
);

public record UpdateUserDto(
    string? UserName,
    string? Email,
    string? PhoneNumber,
    bool? LockoutEnabled,
    bool? TwoFactorEnabled
);

public record ClaimDto(string Type, string Value);

public record UserLoginDto(string LoginProvider, string ProviderDisplayName);

public record AddClaimDto(string Type, string Value);

public record UpdateClaimDto(string OldType, string OldValue, string NewType, string NewValue);

public record AssignRoleDto(string RoleName);

public record CreateRoleDto(string RoleName);

public record RoleDto(string Id, string? Name, string? NormalizedName);