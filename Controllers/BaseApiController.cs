using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RedBerryApi.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        public static class AppRoles
        {
            public const string SuperAdmin = "SuperAdmin";
            public const string Admin = "Admin";
            public const string Agent = "Agent";
            public const string Viewer = "Viewer";
        }

        protected bool IsValidRole(string role)
        {
            var allowedRoles = new[]
            {
                AppRoles.SuperAdmin,
                AppRoles.Admin,
                AppRoles.Agent,
                AppRoles.Viewer
            };

            return allowedRoles.Contains(role);
        }

        //protected string GetCurrentUserRole()
        //{
        //    return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";
        //}
        protected string GetCurrentUserRole()
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            return roleClaim?.Value;
        }
        //protected int? GetCurrentEmpId()
        //{
        //    var empIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EmpId")?.Value;
        //    if (int.TryParse(empIdClaim, out int empId))
        //        return empId;

        //    return null;
        //}
        protected int? GetCurrentEmpId()
        {
            var empIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EmpId");
            if (int.TryParse(empIdClaim?.Value, out int empId))
                return empId;
            return null;
        }

        protected int? GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;

            return null;
        }

        protected bool IsSuperAdmin()
        {
            return GetCurrentUserRole() == AppRoles.SuperAdmin;
        }

        protected bool IsAdmin()
        {
            return GetCurrentUserRole() == AppRoles.Admin;
        }

        protected bool IsAgent()
        {
            return GetCurrentUserRole() == AppRoles.Agent;
        }

        protected bool IsViewer()
        {
            return GetCurrentUserRole() == AppRoles.Viewer;
        }

        protected bool IsSuperAdminOrAdmin()
        {
            var role = GetCurrentUserRole();
            return role == AppRoles.SuperAdmin || role == AppRoles.Admin;
        }

        protected bool CanManageProperties()
        {
            var role = GetCurrentUserRole();
            return role == AppRoles.SuperAdmin ||
                   role == AppRoles.Admin ||
                   role == AppRoles.Agent;
        }
        protected IActionResult? CheckRoleAccess(params string[] allowedRoles)
        {
            var currentRole = GetCurrentUserRole();

            if (string.IsNullOrWhiteSpace(currentRole))
            {
                return Unauthorized(new
                {
                    message = "Unable to detect current user role. Please login again."
                });
            }

            if (!allowedRoles.Contains(currentRole))
            {
                return StatusCode(403, new
                {
                    message = $"Access denied. '{currentRole}' is not allowed to access this API."
                });
            }

            return null;
        }


        protected IActionResult? RequireSuperAdmin(string moduleName = "this resource")
        {
            var role = GetCurrentUserRole();

            if (string.IsNullOrWhiteSpace(role))
            {
                return Unauthorized(new
                {
                    message = "Invalid token or role not found."
                });
            }

            if (role != AppRoles.SuperAdmin)
            {
                return StatusCode(403, new
                {
                    message = $"Only Super Admin can manage {moduleName}."
                });
            }

            return null;
        }
        protected string GetUserRole()
        {
            return User.FindFirst("role")?.Value
                ?? User.FindFirst(ClaimTypes.Role)?.Value;
        }
        protected int GetCurrentUserIdOrThrow()
        {
            var userId = GetCurrentUserId();

            if (!userId.HasValue)
            {
                throw new UnauthorizedAccessException("User not authenticated or token invalid.");
            }

            return userId.Value;
        }
        protected Guid? GetCurrentUserGuid()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
                return userId;

            return null;
        }
        protected Guid GetCurrentUserGuidOrThrow()
        {
            var userId = GetCurrentUserGuid();

            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User not authenticated or token invalid.");

            return userId.Value;
        }
    }
}