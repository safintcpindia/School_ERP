using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;

namespace SchoolERP.Net.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _service;
        private readonly IClassService _classService;
        private readonly ISectionService _sectionService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IStudentLeaveService _leaveService;
        private readonly IStudentInformationService _studentService;

        public AttendanceController(
            IAttendanceService service,
            IClassService classService,
            ISectionService sectionService,
            ICompanyService companyService,
            ISessionService sessionService,
            IStudentLeaveService leaveService,
            IStudentInformationService studentService)
        {
            _service = service;
            _classService = classService;
            _sectionService = sectionService;
            _companyService = companyService;
            _sessionService = sessionService;
            _leaveService = leaveService;
            _studentService = studentService;
        }

        private int GetUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value ?? "0");
        private int GetCompanyId() => _companyService.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionService.GetUserCurrentSession(GetUserId()) ?? 0;

        public IActionResult Attendance()
        {
            ViewBag.ClassList = _classService.GetAllClasses(GetCompanyId(), GetSessionId());
            return View();
        }

        [HttpGet]
        public IActionResult GetAttendanceList(int classId, int sectionId, DateTime date)
        {
            var data = _service.GetStudentAttendanceList(classId, sectionId, date, GetCompanyId());
            return Json(new { success = true, data });
        }

        [HttpPost]
        public IActionResult SaveAttendance([FromBody] AttendanceUpsertRequest req)
        {
            var result = _service.SaveBulkAttendance(req, GetCompanyId(), GetUserId());
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public IActionResult GetSectionsByClass(int classId)
        {
            var data = _sectionService.GetSectionsByClass(classId);
            return Json(new { success = true, data });
        }

        public IActionResult Report()
        {
            ViewBag.ClassList = _classService.GetAllClasses(GetCompanyId(), GetSessionId());
            return View();
        }

        public IActionResult ApproveLeave()
        {
            ViewBag.ClassList = _classService.GetAllClasses(GetCompanyId(), GetSessionId());
            return View();
        }

        [HttpGet]
        public IActionResult GetLeaveList(int? classId, int? sectionId, int? status)
        {
            var data = _leaveService.GetLeaveApplications(classId, sectionId, status, GetCompanyId());
            return Json(new { success = true, data });
        }

        [HttpPost]
        public IActionResult UpdateLeaveStatus([FromBody] LeaveStatusUpdateRequest req)
        {
            var result = _leaveService.UpdateLeaveStatus(req.LeaveAppID, req.Status, GetCompanyId(), GetUserId());
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public IActionResult GetStudentsForLeave(int classId, int sectionId)
        {
            var data = _studentService.GetStudentList(GetCompanyId(), GetSessionId(), classId, sectionId, null);
            return Json(new { success = true, data });
        }

        [HttpPost]
        public IActionResult UpsertLeave([FromForm] int LeaveAppID, [FromForm] int StudentID, [FromForm] DateTime FromDate, 
                                        [FromForm] DateTime ToDate, [FromForm] DateTime ApplyDate, [FromForm] string? Reason, 
                                        [FromForm] int Status, IFormFile? Attachment)
        {
            var req = new StudentLeaveUpsertRequest
            {
                LeaveAppID = LeaveAppID,
                StudentID = StudentID,
                FromDate = FromDate,
                ToDate = ToDate,
                ApplyDate = ApplyDate,
                Reason = Reason,
                Status = Status
            };

            if (Attachment != null)
            {
                using (var ms = new MemoryStream())
                {
                    Attachment.CopyTo(ms);
                    req.Attachment = ms.ToArray();
                    req.AttachmentName = Attachment.FileName;
                    req.AttachmentType = Attachment.ContentType;
                }
            }

            var result = _leaveService.UpsertLeaveApplication(req, GetCompanyId(), GetUserId());
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public IActionResult DownloadAttachment(int id)
        {
            var (bytes, fileName, contentType) = _leaveService.GetLeaveAttachment(id, GetCompanyId());
            if (bytes == null) return NotFound();
            return File(bytes, contentType ?? "application/octet-stream", fileName ?? "attachment");
        }
    }
}
