using System;
using System.Linq;
using System.Security.Claims;

namespace SchoolERP.Net.Services
{
    public sealed class UserMenuPermissionService : IUserMenuPermissionService
    {
        private readonly IUserManagementService _userManagementService;

        public UserMenuPermissionService(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public int GetCurrentUserId(ClaimsPrincipal user)
        {
            var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier) ?? user?.FindFirst("UserId");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var id))
                return 0;
            return id;
        }

        public bool Has(ClaimsPrincipal user, string menuUrlPrefix, string permissionName)
        {
            var userId = GetCurrentUserId(user);
            if (userId <= 0) return false;
            if (IsSuperAdmin(user)) return true;
            if (string.IsNullOrWhiteSpace(menuUrlPrefix) || string.IsNullOrWhiteSpace(permissionName))
                return false;

            var key = menuUrlPrefix.Trim().TrimEnd('/');
            if (string.IsNullOrEmpty(key)) return false;

            var perms = _userManagementService.GetUserPermissions(userId);
            return perms.Any(p =>
                !string.IsNullOrEmpty(p.MenuURL) &&
                p.PermissionName.Equals(permissionName, StringComparison.OrdinalIgnoreCase) &&
                (p.MenuURL.Equals(key, StringComparison.OrdinalIgnoreCase) ||
                 p.MenuURL.StartsWith(key + "/", StringComparison.OrdinalIgnoreCase) ||
                 p.MenuURL.Contains(key, StringComparison.OrdinalIgnoreCase)));
        }

        private static bool IsSuperAdmin(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true) return false;

            return user.Claims.Any(c =>
                (string.Equals(c.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "Role", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "UserTypeName", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase)) &&
                IsSuperAdminValue(c.Value));
        }

        private static bool IsSuperAdminValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var normalized = value.Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty)
                .Trim();
            return string.Equals(normalized, "SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
