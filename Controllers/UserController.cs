using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This class handles HTTP routing and API requests for UserController.
    /// </summary>
    public class UserController : Controller
    {
        private readonly IUserClientService _userClient;
        private readonly ICompanyClientService _companyClient;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/User";

        public UserController(IUserClientService userClient, ICompanyClientService companyClient, IUserMenuPermissionService menuPerm)
        {
            _userClient = userClient;
            _companyClient = companyClient;
            _menuPerm = menuPerm;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "User Management";

            var usersResponse = await _userClient.GetAllUsersAsync();
            var rolesResponse = await _userClient.GetRolesDropdownAsync();
            var typesResponse = await _userClient.GetUserTypesDropdownAsync();
            var companiesResponse = await _companyClient.GetAllAsync();

            var model = new UsersPageViewModel
            {
                Users     = usersResponse.Success ? usersResponse.Data : new List<UserViewModel>(),
                Roles     = rolesResponse.Success ? rolesResponse.Data : new List<RoleViewModel>(),
                UserTypes = typesResponse.Success ? typesResponse.Data : new List<MstUserTypeViewModel>(),
                Companies = companiesResponse.Success ? companiesResponse.Data : new List<MstCompanyViewModel>()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(int userId)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit users." });

            try
            {
                var userResponse = await _userClient.GetUserByIdAsync(userId);
                if (!userResponse.Success)
                    return Json(new { success = false, message = userResponse.Message });

                var roleIdsResponse = await _userClient.GetUserRoleIdsAsync(userId);
                var user = userResponse.Data;

                return Json(new
                {
                    success = true,
                    user = new
                    {
                        user.UserID,
                        user.FullName,
                        user.Username,
                        user.Email,
                        user.PhoneNo,
                        user.UserTypeID,
                        user.DefaultRoleID,
                        user.DashboardID,
                        user.BackDaysAllow,
                        user.IsOTPEnabled,
                        user.OTPSecret,
                        OTPExpiry  = user.OTPExpiry?.ToString("yyyy-MM-dd"),
                        StartDate  = user.StartDate?.ToString("yyyy-MM-dd"),
                        EndDate    = user.EndDate?.ToString("yyyy-MM-dd"),
                        StartTime  = user.StartTime?.ToString(@"hh\:mm"),
                        EndTime    = user.EndTime?.ToString(@"hh\:mm"),
                        user.IsActive,
                        user.IsLocked,
                        RoleIDs    = roleIdsResponse.Success ? roleIdsResponse.Data : new List<int>()
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] UserUpsertRequest request)
        {
            var isCreate = request.UserID <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Json(new { success = false, message = "You do not have permission to add users." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to edit users." });

            try
            {
                if (string.IsNullOrWhiteSpace(request.FullName))
                    return Json(new { success = false, message = "Full name is required" });

                if (string.IsNullOrWhiteSpace(request.Username))
                    return Json(new { success = false, message = "Username is required" });

                if (request.UserTypeID <= 0)
                    return Json(new { success = false, message = "User type is required" });

                var response = await _userClient.SaveUserAsync(request);
                return Json(new { success = response.Success, message = response.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int userId, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to change user status." });

            try
            {
                var response = await _userClient.ToggleStatusAsync(userId, isActive);
                return Json(new { success = response.Success, message = response.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Unlock(int userId)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "You do not have permission to unlock users." });

            try
            {
                var response = await _userClient.UnlockUserAsync(userId);
                return Json(new { success = response.Success, message = response.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
