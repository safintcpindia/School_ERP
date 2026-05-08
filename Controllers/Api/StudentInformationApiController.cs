using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentInformationApiController : ControllerBase
    {
        private readonly IStudentInformationService _studentService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;

        public StudentInformationApiController(IStudentInformationService studentService, ICompanyService companyService, ISessionService sessionService)
        {
            _studentService = studentService;
            _companyService = companyService;
            _sessionService = sessionService;
        }

        private int GetUserId() => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value, out var id) ? id : 0;
        private int GetCompanyId() => _companyService.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionService.GetUserCurrentSession(GetUserId()) ?? 0;

        [HttpGet("GetAllDisableReasons")]
        public IActionResult GetAllDisableReasons()
        {
            var data = _studentService.GetAllDisableReasons(GetCompanyId(), GetSessionId());
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertDisableReason")]
        public IActionResult UpsertDisableReason([FromBody] StudentDisableReasonUpsertRequest req)
        {
            var res = _studentService.UpsertDisableReason(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteDisableReason/{id}")]
        public IActionResult DeleteDisableReason(int id)
        {
            var res = _studentService.DeleteDisableReason(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpGet("GetAllStudentHouses")]
        public IActionResult GetAllStudentHouses()
        {
            var data = _studentService.GetAllStudentHouses(GetCompanyId(), GetSessionId());
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertStudentHouse")]
        public IActionResult UpsertStudentHouse([FromBody] StudentHouseUpsertRequest req)
        {
            var res = _studentService.UpsertStudentHouse(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteStudentHouse/{id}")]
        public IActionResult DeleteStudentHouse(int id)
        {
            var res = _studentService.DeleteStudentHouse(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpGet("GetAllStudentCategories")]
        public IActionResult GetAllStudentCategories()
        {
            var data = _studentService.GetAllStudentCategories(GetCompanyId(), GetSessionId());
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertStudentCategory")]
        public IActionResult UpsertStudentCategory([FromBody] StudentCategoryUpsertRequest req)
        {
            var res = _studentService.UpsertStudentCategory(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteStudentCategory/{id}")]
        public IActionResult DeleteStudentCategory(int id)
        {
            var res = _studentService.DeleteStudentCategory(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpGet("GetStudentTimeline/{studentId}")]
        public IActionResult GetStudentTimeline(int studentId)
        {
            try
            {
                var data = _studentService.GetStudentTimeline(studentId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Database Error: " + ex.Message });
            }
        }

        [HttpPost("UpsertTimeline")]
        public IActionResult UpsertTimeline([FromBody] StudentTimelineUpsertRequest req)
        {
            try
            {
                var result = _studentService.UpsertStudentTimeline(req, GetCompanyId(), GetSessionId(), GetUserId());
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Database Error: " + ex.Message });
            }
        }

        [HttpPost("DeleteTimeline/{id}")]
        public IActionResult DeleteTimeline(int id)
        {
            try
            {
                var result = _studentService.DeleteStudentTimeline(id, GetUserId());
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Database Error: " + ex.Message });
            }
        }

        [HttpGet("DownloadTimelineDoc/{id}")]
        public IActionResult DownloadTimelineDoc(int id)
        {
            var (bytes, fileName, contentType) = _studentService.GetStudentTimelineDocument(id);
            if (bytes == null) return NotFound();
            return File(bytes, contentType, fileName);
        }
        [HttpPost("ToggleStatus")]
        public IActionResult ToggleStatus([FromBody] StudentStatusToggleRequest req)
        {
            var res = _studentService.ToggleStudentStatus(req, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpGet("GetMultiClassStudents")]
        public IActionResult GetMultiClassStudents(int? classId, int? sectionId, string? searchTerm)
        {
            var data = _studentService.GetMultiClassStudents(GetCompanyId(), GetSessionId(), classId, sectionId, searchTerm);
            return Ok(new { success = true, data });
        }

        [HttpGet("GetStudentList")]
        public IActionResult GetStudentList(int? classId, int? sectionId, string? searchTerm)
        {
            var data = _studentService.GetStudentList(GetCompanyId(), GetSessionId(), classId, sectionId, searchTerm);
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertMultiClass")]
        public IActionResult UpsertMultiClass([FromBody] StudentMultiClassUpsertRequest req)
        {
            var res = _studentService.UpsertStudentMultiClass(req, GetCompanyId(), GetSessionId(), GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteMultiClass/{id}")]
        public IActionResult DeleteMultiClass(int id)
        {
            var res = _studentService.DeleteStudentMultiClass(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }
    }
}
