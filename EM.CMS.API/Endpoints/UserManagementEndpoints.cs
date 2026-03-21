using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using EM.CMS.API.Models.UserManagement;
using EM.CMS.API.Services;

namespace EM.CMS.API.Endpoints;

public static class UserManagementEndpoints
{
    public static IEndpointRouteBuilder MapUserManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
            .WithTags("User Management")
            .RequireAuthorization();

        // User endpoints
        group.MapGet("/", GetAllUsers).WithName("GetAllUsers");
        group.MapGet("/{userId}", GetUserById).WithName("GetUserById");
        group.MapPost("/", CreateUser).WithName("CreateUser");
        group.MapPut("/{userId}", UpdateUser).WithName("UpdateUser");
        group.MapDelete("/{userId}", SoftDeleteUser).WithName("SoftDeleteUser");
        group.MapPost("/{userId}/restore", RestoreUser).WithName("RestoreUser");

        // Role endpoints for users
        group.MapGet("/{userId}/roles", GetUserRoles).WithName("GetUserRoles");
        group.MapPost("/{userId}/roles", AddUserToRole).WithName("AddUserToRole");
        group.MapDelete("/{userId}/roles/{roleName}", RemoveUserFromRole).WithName("RemoveUserFromRole");

        // Claims endpoints
        group.MapGet("/{userId}/claims", GetUserClaims).WithName("GetUserClaims");
        group.MapPost("/{userId}/claims", AddUserClaim).WithName("AddUserClaim");
        group.MapPut("/{userId}/claims", UpdateUserClaim).WithName("UpdateUserClaim");
        group.MapDelete("/{userId}/claims", RemoveUserClaim).WithName("RemoveUserClaim");

        // Logins endpoints
        group.MapGet("/{userId}/logins", GetUserLogins).WithName("GetUserLogins");
        group.MapDelete("/{userId}/logins/{loginProvider}/{providerKey}", RemoveUserLogin).WithName("RemoveUserLogin");

        return app;
    }

    #region User Handlers

    private static async Task<Ok<IEnumerable<UserDto>>> GetAllUsers(
        IUserManagementService service,
        bool includeDeleted = false)
    {
        var users = await service.GetAllUsersAsync(includeDeleted);
        return TypedResults.Ok(users);
    }

    private static async Task<Results<Ok<UserDetailDto>, NotFound<string>>> GetUserById(
        string userId,
        IUserManagementService service)
    {
        var user = await service.GetUserByIdAsync(userId);
        return user is not null
            ? TypedResults.Ok(user)
            : TypedResults.NotFound($"User with ID '{userId}' not found.");
    }

    private static async Task<Results<Created<string>, BadRequest<IEnumerable<IdentityError>>>> CreateUser(
        CreateUserDto dto,
        IUserManagementService service)
    {
        var result = await service.CreateUserAsync(dto);
        return result.Succeeded
            ? TypedResults.Created($"/api/users", dto.UserName)
            : TypedResults.BadRequest(result.Errors);
    }

    private static async Task<Results<Ok, BadRequest<IEnumerable<IdentityError>>>> UpdateUser(
        string userId,
        UpdateUserDto dto,
        IUserManagementService service)
    {
        var result = await service.UpdateUserAsync(userId, dto);
        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.BadRequest(result.Errors);
    }

    private static async Task<Results<Ok, BadRequest<IEnumerable<IdentityError>>>> SoftDeleteUser(
        string userId,
        IUserManagementService service)
    {
        var result = await service.SoftDeleteUserAsync(userId);
        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.BadRequest(result.Errors);
    }

    private static async Task<Results<Ok, BadRequest<IEnumerable<IdentityError>>>> RestoreUser(
        string userId,
        IUserManagementService service)
    {
        var result = await service.RestoreUserAsync(userId);
        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.BadRequest(result.Errors);
    }

    #endregion

    #region Role Handlers

    private static async Task<Ok<IEnumerable<string>>> GetUserRoles(
        string userId,
        IUserManagementService service)
    {
        var roles = await service.GetUserRolesAsync(userId);
        return TypedResults.Ok(roles);
    }

    private static async Task<Results<Ok, BadRequest<IEnumerable<IdentityError>>>> AddUserToRole(
        string userId,
        AssignRoleDto dto,
        IUserManagementService service)
    {
        var result = await service.AddUserToRoleAsync(userId, dto.RoleName);
        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.BadRequest(result.Errors);
    }

    private static async Task<Results<Ok, BadRequest<IEnumerable<IdentityError>>>> RemoveUserFromRole(
        string userId,
        string roleName,
        IUserManagementService service)
    {
        var result = await service.RemoveUserFromRoleAsync(userId, roleName);
        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.BadRequest(result.Errors);
    }

    #endregion

    #region Claims Handlers

    private static async Task<Ok<IEnumerable<ClaimDto>>> GetUserClaims(
        string userId,
        IUserManagementService service)
    {
        var claims = await service.GetUserClaimsAsync(userId);
        return TypedResults.Ok(claims);
    }

    private static async Task<Results<Ok, BadRequest<IEnumerable<IdentityError>>>> AddUserClaim(
        string userId,
        AddClaimDto dto,
        IUserManagementService service)
    {
        var result = await service.AddUserClaimAsync(userId, dto);
        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.BadRequest(result.Errors);
    }

    private static async Task<Results<Ok, BadRequest<IEnumerable<IdentityError>>>> UpdateUserClaim(
        string userId,
        UpdateClaimDto dto,
        IUserManagementService service)
    {
        var result = await service.UpdateUserClaimAsync(userId, dto);
        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.BadRequest(result.Errors);
    }

    private static async Task<Results<Ok, BadRequest<IEnumerable<IdentityError>>>> RemoveUserClaim(
        string userId,
        string claimType,
        string claimValue,
        IUserManagementService service)
    {
        var result = await service.RemoveUserClaimAsync(userId, claimType, claimValue);
        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.BadRequest(result.Errors);
    }

    #endregion

    #region Login Handlers

    private static async Task<Ok<IEnumerable<UserLoginDto>>> GetUserLogins(
        string userId,
        IUserManagementService service)
    {
        var logins = await service.GetUserLoginsAsync(userId);
        return TypedResults.Ok(logins);
    }

    private static async Task<Results<Ok, BadRequest<IEnumerable<IdentityError>>>> RemoveUserLogin(
        string userId,
        string loginProvider,
        string providerKey,
        IUserManagementService service)
    {
        var result = await service.RemoveUserLoginAsync(userId, loginProvider, providerKey);
        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.BadRequest(result.Errors);
    }

    #endregion
}