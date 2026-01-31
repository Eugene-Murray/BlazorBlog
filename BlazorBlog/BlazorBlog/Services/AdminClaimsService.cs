using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using BlazorBlog.Data;

namespace BlazorBlog.Services
{
    public class AdminClaimsService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminClaimsService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> SetUserAsAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.IsAdmin = true;
            var updateResult = await _userManager.UpdateAsync(user);
            
            if (updateResult.Succeeded)
            {
                // Add admin claim
                var claimResult = await _userManager.AddClaimAsync(user, new Claim("IsAdmin", "True"));
                return claimResult.Succeeded;
            }

            return false;
        }

        public async Task<bool> RemoveUserAsAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.IsAdmin = false;
            var updateResult = await _userManager.UpdateAsync(user);
            
            if (updateResult.Succeeded)
            {
                // Remove admin claim
                var claimResult = await _userManager.RemoveClaimAsync(user, new Claim("IsAdmin", "True"));
                return claimResult.Succeeded;
            }

            return false;
        }

        public async Task SyncUserClaimsAsync(ApplicationUser user)
        {
            var existingClaims = await _userManager.GetClaimsAsync(user);
            var hasAdminClaim = existingClaims.Any(c => c.Type == "IsAdmin" && c.Value == "True");

            if (user.IsAdmin && !hasAdminClaim)
            {
                await _userManager.AddClaimAsync(user, new Claim("IsAdmin", "True"));
            }
            else if (!user.IsAdmin && hasAdminClaim)
            {
                await _userManager.RemoveClaimAsync(user, new Claim("IsAdmin", "True"));
            }
        }
    }
}