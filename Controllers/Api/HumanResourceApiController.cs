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
        private readonly IUserMenuPermissionService _menuPerm;

        private const string DesignationMenuPath = "/HumanResource/Designation";
        private const string DepartmentMenuPath = "/HumanResource/Department";
        private const string LeaveTypeMenuPath = "/HumanResource/LeaveType";
        private const string StaffMenuPath = "/HumanResource/Staffs";

        public HumanResourceApiController(IHumanResourceService hrService, ICompanyService companySvc, ISessionService sessionSvc, IUserMenuPermissionService menuPerm)
        {
            _hrService = hrService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
            _menuPerm = menuPerm;
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
            var isCreate = req.HRDesignationID <= 0;
            if (isCreate && !_menuPerm.Has(User, DesignationMenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add designations." });
            if (!isCreate && !_menuPerm.Has(User, DesignationMenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit designations." });

            var res = _hrService.UpsertDesignation(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteDesignation/{id}")]
        public IActionResult DeleteDesignation(int id)
        {
            if (!_menuPerm.Has(User, DesignationMenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete designations." });

            var res = _hrService.DeleteDesignation(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("ToggleDesignationStatus")]
        public IActionResult ToggleDesignationStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, DesignationMenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to change designation status." });

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
            var isCreate = req.DepartmentID <= 0;
            if (isCreate && !_menuPerm.Has(User, DepartmentMenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add departments." });
            if (!isCreate && !_menuPerm.Has(User, DepartmentMenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit departments." });

            var res = _hrService.UpsertDepartment(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteDepartment/{id}")]
        public IActionResult DeleteDepartment(int id)
        {
            if (!_menuPerm.Has(User, DepartmentMenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete departments." });

            var res = _hrService.DeleteDepartment(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("ToggleDepartmentStatus")]
        public IActionResult ToggleDepartmentStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, DepartmentMenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to change department status." });

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
            var isCreate = req.LeaveTypeID <= 0;
            if (isCreate && !_menuPerm.Has(User, LeaveTypeMenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add leave types." });
            if (!isCreate && !_menuPerm.Has(User, LeaveTypeMenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit leave types." });

            var res = _hrService.UpsertLeaveType(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteLeaveType/{id}")]
        public IActionResult DeleteLeaveType(int id)
        {
            if (!_menuPerm.Has(User, LeaveTypeMenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete leave types." });

            var res = _hrService.DeleteLeaveType(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("ToggleLeaveTypeStatus")]
        public IActionResult ToggleLeaveTypeStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, LeaveTypeMenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to change leave type status." });

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
            var isCreate = req.StaffID <= 0;
            if (isCreate && !_menuPerm.Has(User, StaffMenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add staff." });
            if (!isCreate && !_menuPerm.Has(User, StaffMenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit staff." });

            var res = _hrService.UpsertStaff(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteStaff/{id}")]
        public IActionResult DeleteStaff(int id)
        {
            if (!_menuPerm.Has(User, StaffMenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete staff." });

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
