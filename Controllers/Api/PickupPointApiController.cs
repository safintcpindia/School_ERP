using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PickupPointApiController : ControllerBase
    {
        private readonly IPickupPointService _pickupPointService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Transport/PickupPoint";

        public PickupPointApiController(IPickupPointService pickupPointService, ICompanyService companySvc, ISessionService sessionSvc, IUserMenuPermissionService menuPerm)
        {
            _pickupPointService = pickupPointService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
            _menuPerm = menuPerm;
        }

        private int GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId"), out var id) ? id : 0;
        private int GetCompanyId() => _companySvc.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionSvc.GetUserCurrentSession(GetUserId()) ?? 0;

        [HttpGet("GetAllPickupPoints")]
        public IActionResult GetAllPickupPoints()
        {
            var data = _pickupPointService.GetAllPickupPoints(GetCompanyId(), GetSessionId());
            return Ok(new { success = true, data });
        }

        [HttpGet("GetPickupPointByID/{id}")]
        public IActionResult GetPickupPointByID(int id)
        {
            var data = _pickupPointService.GetPickupPointByID(id);
            if (data == null) return Ok(new { success = false, message = "Record not found" });
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertPickupPoint")]
        public IActionResult UpsertPickupPoint([FromBody] PickupPointUpsertRequest req)
        {
            var isCreate = req.PickupPointID <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add pickup points." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit pickup points." });

            var res = _pickupPointService.UpsertPickupPoint(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeletePickupPoint/{id}")]
        public IActionResult DeletePickupPoint(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete pickup points." });

            var res = _pickupPointService.DeletePickupPoint(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("TogglePickupPointStatus")]
        public IActionResult TogglePickupPointStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to change pickup point status." });

            var res = _pickupPointService.TogglePickupPointStatus(id, isActive, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }
    }
}
