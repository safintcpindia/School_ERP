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

        public PickupPointApiController(IPickupPointService pickupPointService, ICompanyService companySvc, ISessionService sessionSvc)
        {
            _pickupPointService = pickupPointService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
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
            var res = _pickupPointService.UpsertPickupPoint(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeletePickupPoint/{id}")]
        public IActionResult DeletePickupPoint(int id)
        {
            var res = _pickupPointService.DeletePickupPoint(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("TogglePickupPointStatus")]
        public IActionResult TogglePickupPointStatus(int id, bool isActive)
        {
            var res = _pickupPointService.TogglePickupPointStatus(id, isActive, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }
    }
}
