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
    public class VehicleAssignApiController : ControllerBase
    {
        private readonly IVehicleAssignService _vehicleAssignService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;

        public VehicleAssignApiController(IVehicleAssignService vehicleAssignService, ICompanyService companySvc, ISessionService sessionSvc)
        {
            _vehicleAssignService = vehicleAssignService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
        }

        private int GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId"), out var id) ? id : 0;
        private int GetCompanyId() => _companySvc.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionSvc.GetUserCurrentSession(GetUserId()) ?? 0;

        [HttpGet("GetAllAssignments")]
        public IActionResult GetAllAssignments()
        {
            var data = _vehicleAssignService.GetAllAssignments(GetCompanyId(), GetSessionId());
            return Ok(new { success = true, data });
        }

        [HttpGet("GetAssignmentByID/{id}")]
        public IActionResult GetAssignmentByID(int id)
        {
            var data = _vehicleAssignService.GetAssignmentByID(id);
            if (data == null) return Ok(new { success = false, message = "Record not found" });
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertAssignments")]
        public IActionResult UpsertAssignments([FromBody] VehicleAssignUpsertRequest req)
        {
            var res = _vehicleAssignService.UpsertAssignments(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteAssignment/{id}")]
        public IActionResult DeleteAssignment(int id)
        {
            var res = _vehicleAssignService.DeleteAssignment(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("ToggleAssignmentStatus")]
        public IActionResult ToggleAssignmentStatus(int id, bool isActive)
        {
            var res = _vehicleAssignService.ToggleAssignmentStatus(id, isActive, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }
    }
}
