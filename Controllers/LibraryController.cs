using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers
{
    public class LibraryController : Controller
    {
        private readonly ILibraryService _service;
        private readonly ICompanyService _companyService;
        private readonly IClassService _classService;
        private readonly ISectionService _sectionService;
        private readonly ISessionService _sessionService;
        private readonly IHumanResourceService _hrService;

        public LibraryController(
            ILibraryService service, 
            ICompanyService companyService,
            IClassService classService,
            ISectionService sectionService,
            ISessionService sessionService,
            IHumanResourceService hrService)
        {
            _service = service;
            _companyService = companyService;
            _classService = classService;
            _sectionService = sectionService;
            _sessionService = sessionService;
            _hrService = hrService;
        }

        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value ?? "0");
        private int GetCompanyId() => _companyService.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionService.GetUserCurrentSession(GetUserId()) ?? 0;

        #region Books
        public IActionResult Books()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetBookList(string? search)
        {
            var data = _service.GetBookList(GetCompanyId(), search);
            return Json(new { success = true, data });
        }

        [HttpGet]
        public IActionResult GetBook(int id)
        {
            var data = _service.GetBookById(id);
            return Json(new { success = data != null, data });
        }

        [HttpPost]
        public IActionResult UpsertBook([FromBody] BookUpsertRequest req)
        {
            var companyId = GetCompanyId();
            if (companyId <= 0) return Json(new { success = false, message = "Valid Company ID required." });

            var res = _service.UpsertBook(req, companyId, GetUserId());
            return Json(new { success = res.Success, message = res.Message });
        }

        [HttpPost]
        public IActionResult DeleteBook(int id)
        {
            var res = _service.DeleteBook(id, GetUserId());
            return Json(new { success = res.Success, message = res.Message });
        }

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var res = _service.ToggleBookStatus(id, GetUserId());
            return Json(new { success = res.Success, message = res.Message });
        }
        #endregion

        #region Membership
        public IActionResult AddStudents()
        {
            var companyId = GetCompanyId();
            var sessionId = GetSessionId();
            ViewBag.Classes = _classService.GetAllClasses(companyId, sessionId);
            return View();
        }

        public IActionResult AddStaff()
        {
            var companyId = GetCompanyId();
            var sessionId = GetSessionId();
            ViewBag.Departments = _hrService.GetAllDepartments(companyId, sessionId);
            return View();
        }

        [HttpGet]
        public IActionResult GetMemberList(string type, int? classId, int? sectionId, int? deptId, string? search)
        {
            var data = _service.GetMemberList(type, GetCompanyId(), classId, sectionId, deptId, search);
            return Json(new { success = true, data });
        }

        [HttpGet]
        public IActionResult SearchForMembership(string type, int? classId, int? sectionId, int? deptId, string? search)
        {
            var data = _service.SearchForMembership(type, GetCompanyId(), classId, sectionId, deptId, search);
            return Json(new { success = true, data });
        }

        [HttpPost]
        public IActionResult AddMember([FromBody] LibraryMemberUpsertRequest req)
        {
            var res = _service.AddMember(req, GetCompanyId(), GetUserId());
            return Json(new { success = res.Success, message = res.Message });
        }

        [HttpPost]
        public IActionResult DeleteMember(int id, int? studentId, int? staffId)
        {
            if (studentId.HasValue || staffId.HasValue)
            {
                // Custom delete via SP update
                var companyId = GetCompanyId();
                var userId = GetUserId();
                var res = ((LibraryService)_service).DeleteMemberEx(id, studentId, staffId, userId);
                return Json(new { success = res.Success, message = res.Message });
            }
            else
            {
                var res = _service.DeleteMember(id, GetUserId());
                return Json(new { success = res.Success, message = res.Message });
            }
        }
        #endregion

        [HttpGet]
        public IActionResult GetSections(int classId)
        {
            var sections = _sectionService.GetSectionsByClass(classId);
            return Json(sections);
        }
    }
}
