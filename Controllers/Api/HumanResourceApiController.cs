using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using System;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HumanResourceApiController : ControllerBase
    {
        private readonly IHumanResourceService _hrService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;

        public HumanResourceApiController(IHumanResourceService hrService, ICompanyService companySvc, ISessionService sessionSvc)
        {
            _hrService = hrService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
        }

        private int GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId"), out var id) ? id : 0;
        private int GetCompanyId() => _companySvc.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionSvc.GetUserCurrentSession(GetUserId()) ?? 0;

        [HttpGet("GetAllDesignations")]
        public IActionResult GetAllDesignations()
        {
            var data = _hrService.GetAllDesignations(GetCompanyId(), GetSessionId());
            return Ok(new { success = true, data });
        }

        [HttpGet("GetDesignationByID/{id}")]
        public IActionResult GetDesignationByID(int id)
        {
            var data = _hrService.GetDesignationByID(id);
            if (data == null) return Ok(new { success = false, message = "Record not found" });
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertDesignation")]
        public IActionResult UpsertDesignation([FromBody] HRDesignationUpsertRequest req)
        {
            var res = _hrService.UpsertDesignation(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteDesignation/{id}")]
        public IActionResult DeleteDesignation(int id)
        {
            var res = _hrService.DeleteDesignation(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("ToggleDesignationStatus")]
        public IActionResult ToggleDesignationStatus(int id, bool isActive)
        {
            var res = _hrService.ToggleDesignationStatus(id, isActive, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpGet("GetAllDepartments")]
        public IActionResult GetAllDepartments()
        {
            var data = _hrService.GetAllDepartments(GetCompanyId(), GetSessionId());
            return Ok(new { success = true, data });
        }

        [HttpGet("GetDepartmentByID/{id}")]
        public IActionResult GetDepartmentByID(int id)
        {
            var data = _hrService.GetDepartmentByID(id);
            if (data == null) return Ok(new { success = false, message = "Record not found" });
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertDepartment")]
        public IActionResult UpsertDepartment([FromBody] HRDepartmentUpsertRequest req)
        {
            var res = _hrService.UpsertDepartment(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteDepartment/{id}")]
        public IActionResult DeleteDepartment(int id)
        {
            var res = _hrService.DeleteDepartment(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("ToggleDepartmentStatus")]
        public IActionResult ToggleDepartmentStatus(int id, bool isActive)
        {
            var res = _hrService.ToggleDepartmentStatus(id, isActive, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpGet("GetAllLeaveTypes")]
        public IActionResult GetAllLeaveTypes()
        {
            var data = _hrService.GetAllLeaveTypes(GetCompanyId(), GetSessionId());
            return Ok(new { success = true, data });
        }

        [HttpGet("GetLeaveTypeByID/{id}")]
        public IActionResult GetLeaveTypeByID(int id)
        {
            var data = _hrService.GetLeaveTypeByID(id);
            if (data == null) return Ok(new { success = false, message = "Record not found" });
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertLeaveType")]
        public IActionResult UpsertLeaveType([FromBody] HRLeaveTypeUpsertRequest req)
        {
            var res = _hrService.UpsertLeaveType(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteLeaveType/{id}")]
        public IActionResult DeleteLeaveType(int id)
        {
            var res = _hrService.DeleteLeaveType(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("ToggleLeaveTypeStatus")]
        public IActionResult ToggleLeaveTypeStatus(int id, bool isActive)
        {
            var res = _hrService.ToggleLeaveTypeStatus(id, isActive, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpGet("GetAllStaff")]
        public IActionResult GetAllStaff()
        {
            var data = _hrService.GetAllStaff(GetCompanyId(), GetSessionId());
            return Ok(new { success = true, data });
        }

        [HttpGet("GetStaffByID/{id}")]
        public IActionResult GetStaffByID(int id)
        {
            var data = _hrService.GetStaffByID(id);
            if (data == null) return Ok(new { success = false, message = "Record not found" });
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertStaff")]
        public IActionResult UpsertStaff([FromBody] HRStaffUpsertRequest req)
        {
            var res = _hrService.UpsertStaff(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteStaff/{id}")]
        public IActionResult DeleteStaff(int id)
        {
            var res = _hrService.DeleteStaff(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpGet("GetNewStaffCode")]
        public IActionResult GetNewStaffCode()
        {
            var data = _hrService.GetNewStaffCode(GetCompanyId(), GetSessionId());
            return Ok(new { success = true, data });
        }
    }
}
