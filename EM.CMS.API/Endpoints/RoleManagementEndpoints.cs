using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using EM.CMS.API.Models.UserManagement;
using EM.CMS.API.Services;

namespace EM.CMS.API.Endpoints;

public static class RoleManagementEndpoints
{
    public static void MapRoleManagementEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/roles")
            .WithTags("Role Management")
            .RequireAuthorization();

        group.MapGet("/", GetAllRoles).WithName("GetAllRoles");
        group.MapPost("/", CreateRole).WithName("CreateRole");
        group.MapDelete("/{roleId}", DeleteRole).WithName("DeleteRole");
    }

    private static async Task<Ok<IEnumerable<RoleDto>>> GetAllRoles(IUserManagementService service)
    {
        var roles = await service.GetAllRolesAsync();
        return TypedResults.Ok(roles);
    }

    private static async Task<Results<Created<string>, BadRequest<IEnumerable<IdentityError>>>> CreateRole(
        CreateRoleDto dto,
        IUserManagementService service)
    {
        var result = await service.CreateRoleAsync(dto.RoleName);
        return result.Succeeded
            ? TypedResults.Created($"/api/roles", dto.RoleName)
            : TypedResults.BadRequest(result.Errors);
    }

    private static async Task<Results<Ok, BadRequest<IEnumerable<IdentityError>>>> DeleteRole(
        string roleId,
        IUserManagementService service)
    {
        var result = await service.DeleteRoleAsync(roleId);
        return result.Succeeded
            ? TypedResults.Ok()
            : TypedResults.BadRequest(result.Errors);
    }
}