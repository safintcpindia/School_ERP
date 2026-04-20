using System.Security.Claims;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// Resolves the signed-in user id and checks menu URL–scoped permissions (same rules as <see cref="Helpers.PermissionHelper.HasPermissionByUrl"/>).
    /// </summary>
    public interface IUserMenuPermissionService
    {
        int GetCurrentUserId(ClaimsPrincipal user);

        /// <param name="menuUrlPrefix">Example: "/Role", "/Settings". Trailing slash is ignored.</param>
        /// <param name="permissionName">Example: View, Add, Edit, Delete.</param>
        bool Has(ClaimsPrincipal user, string menuUrlPrefix, string permissionName);
    }
}
