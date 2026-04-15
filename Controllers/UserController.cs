using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
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

        public UserController(IUserClientService userClient)
        {
            _userClient = userClient;
        }

        /// <summary>
        /// Renders the master User Management list interface.
        /// Extracts both the list of registered users and the dropdown lookup sources (Roles, Types)
        /// in a single page load to avoid secondary AJAX delays on the view.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "User Management";

            var usersResponse = await _userClient.GetAllUsersAsync();
            var rolesResponse = await _userClient.GetRolesDropdownAsync();
            var typesResponse = await _userClient.GetUserTypesDropdownAsync();

            var model = new UsersPageViewModel
            {
                Users     = usersResponse.Success ? usersResponse.Data : new List<UserViewModel>(),
                Roles     = rolesResponse.Success ? rolesResponse.Data : new List<RoleViewModel>(),
                UserTypes = typesResponse.Success ? typesResponse.Data : new List<MstUserTypeViewModel>()
            };

            return View(model);
        }

        /// <summary>
        /// Fetches the details of a specific User ID, including their mapped multidimensional Role sets.
        /// Used primarily to populate the User Upsert Modal on the frontend.
        /// </summary>
        /// <param name="userId">The targeted User Primary Key.</param>
        [HttpGet]
        public async Task<IActionResult> GetUser(int userId)
        {
            try
            {
                // Fetch core user metadata mapping
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

        /// <summary>
        /// Validates and saves an Upsert (Insert/Update) User payload.
        /// Performs minimal server-side validation checks before dispatching the payload to the API client.
        /// </summary>
        /// <param name="request">The mapping properties from the User Create/Edit modal.</param>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] UserUpsertRequest request)
        {
            try
            {
                // Sanity validation: Core identities cannot be blank
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

        /// <summary>
        /// System toggle for allowing or dismissing a user's right to login (Soft Delete / Disable).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int userId, bool isActive)
        {
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

        /// <summary>
        /// Intervenes to remove the "Locked" block from an account that has exceeded invalid attempts.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Unlock(int userId)
        {
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
