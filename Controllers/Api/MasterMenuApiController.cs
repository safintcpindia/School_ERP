using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// Serves as the UI framework routing generator.
    /// Provides raw datasets to construct the frontend application's navigation sidebar.
    /// </summary>
    public class MasterMenuApiController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MasterMenuApiController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Reads the master list of all available navigation nodes globally.
        /// </summary>
        [HttpGet]
        public IActionResult GetAllMenus()
        {
            var menus = _menuService.GetAllMenus();
            return Ok(ApiResponse<List<MenuViewModel>>.SuccessResponse(menus));
        }

        /// <summary>
        /// Queries a single UI module's property set (icon, redirect paths, sort order).
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetMenuById(int id)
        {
            var menu = _menuService.GetMenuById(id);
            if (menu == null) return NotFound(ApiResponse<MenuViewModel>.ErrorResponse("Menu module block not found"));
            return Ok(ApiResponse<MenuViewModel>.SuccessResponse(menu));
        }

        /// <summary>
        /// Deploys a new routing leaf or updates an existing branch directly in the architecture DB.
        /// </summary>
        [HttpPost("save")]
        public IActionResult Save([FromBody] MenuUpsertRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.ErrorResponse("Menu validation block failed", errors));
            }

            int userId = GetCurrentUserId();
            if (userId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));
            int mainAccountId = GetClaimInt("MainAccountId", userId);
            int sessionId = GetClaimInt("SessionId", 0);
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var (result, message) = _menuService.UpsertMenu(request, userId, mainAccountId, sessionId, ipAddress);
            
            if (result > 0)
                return Ok(ApiResponse<bool>.SuccessResponse(true, message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(message));
        }

        /// <summary>
        /// Masks a menu option from rendering without severing DB relation graphs.
        /// </summary>
        [HttpPost("toggle-status")]
        public IActionResult ToggleStatus([FromQuery] int menuId, [FromQuery] bool isActive)
        {
            int userId = GetCurrentUserId();
            if (userId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));
            int mainAccountId = GetClaimInt("MainAccountId", userId);
            int sessionId = GetClaimInt("SessionId", 0);
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var (result, message) = _menuService.ToggleMenuStatus(menuId, isActive, userId, mainAccountId, sessionId, ipAddress);
            
            if (result > 0)
                return Ok(ApiResponse<bool>.SuccessResponse(true, message));

            return BadRequest(ApiResponse<bool>.ErrorResponse(message));
        }

        private int GetCurrentUserId()
        {
            return GetClaimInt(ClaimTypes.NameIdentifier, GetClaimInt("UserId", 0));
        }

        private int GetClaimInt(string claimType, int fallback = 0)
        {
            var claim = User.FindFirst(claimType);
            return (claim != null && int.TryParse(claim.Value, out int value)) ? value : fallback;
        }
    }
}
