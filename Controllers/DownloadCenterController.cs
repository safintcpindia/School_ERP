using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers
{
    public class DownloadCenterController : Controller
    {
        private readonly IDownloadCenterService _service;
        private readonly ICompanyService _companyService;
        private readonly IClassService _classService;
        private readonly ISectionService _sectionService;
        private readonly ISessionService _sessionService;
        private readonly IWebHostEnvironment _env;

        public DownloadCenterController(
            IDownloadCenterService service, 
            ICompanyService companyService,
            IClassService classService,
            ISectionService sectionService,
            ISessionService sessionService,
            IWebHostEnvironment env)
        {
            _service = service;
            _companyService = companyService;
            _classService = classService;
            _sectionService = sectionService;
            _sessionService = sessionService;
            _env = env;
        }

        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value ?? "0");
        private int GetCompanyId() => _companyService.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionService.GetUserCurrentSession(GetUserId()) ?? 0;

        #region Content Type
        public IActionResult ContentType()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetContentTypeList(string? search)
        {
            var data = _service.GetContentTypeList(GetCompanyId(), search);
            return Json(new { success = true, data });
        }

        [HttpGet]
        public IActionResult GetContentType(int id)
        {
            var data = _service.GetContentTypeById(id);
            return Json(new { success = data != null, data });
        }

        [HttpPost]
        public IActionResult UpsertContentType([FromBody] ContentTypeUpsertRequest req)
        {
            var companyId = GetCompanyId();
            if (companyId <= 0) return Json(new { success = false, message = "Valid Company ID required." });

            var res = _service.UpsertContentType(req, companyId, GetUserId());
            return Json(new { success = res.Success, message = res.Message });
        }

        [HttpPost]
        public IActionResult DeleteContentType(int id)
        {
            var res = _service.DeleteContentType(id, GetUserId());
            return Json(new { success = res.Success, message = res.Message });
        }

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var res = _service.ToggleContentTypeStatus(id, GetUserId());
            return Json(new { success = res.Success, message = res.Message });
        }
        #endregion

        #region Video Tutorial
        public IActionResult VideoTutorial()
        {
            var companyId = GetCompanyId();
            var sessionId = GetSessionId();
            ViewBag.Classes = _classService.GetAllClasses(companyId, sessionId);
            return View();
        }

        [HttpGet]
        public IActionResult GetVideoTutorialList(int? classId, int? sectionId, string? search)
        {
            var data = _service.GetVideoTutorialList(GetCompanyId(), classId, sectionId, search);
            return Json(new { success = true, data });
        }

        [HttpGet]
        public IActionResult GetVideoTutorial(int id)
        {
            var data = _service.GetVideoTutorialById(id);
            return Json(new { success = data != null, data });
        }

        [HttpPost]
        public IActionResult UpsertVideoTutorial([FromBody] VideoTutorialUpsertRequest req)
        {
            var companyId = GetCompanyId();
            if (companyId <= 0) return Json(new { success = false, message = "Valid Company ID required." });

            var res = _service.UpsertVideoTutorial(req, companyId, GetUserId());
            return Json(new { success = res.Success, message = res.Message });
        }

        [HttpPost]
        public IActionResult DeleteVideoTutorial(int id)
        {
            var res = _service.DeleteVideoTutorial(id, GetUserId());
            return Json(new { success = res.Success, message = res.Message });
        }

        [HttpPost]
        public IActionResult ToggleVideoStatus(int id)
        {
            var res = _service.ToggleVideoTutorialStatus(id, GetUserId());
            return Json(new { success = res.Success, message = res.Message });
        }
        #endregion

        #region Upload Content
        public IActionResult UploadContent()
        {
            var companyId = GetCompanyId();
            ViewBag.ContentTypes = _service.GetContentTypeList(companyId, null);
            return View();
        }

        [HttpGet]
        public IActionResult GetContentList(string? search)
        {
            var data = _service.GetContentList(GetCompanyId(), search);
            return Json(new { success = true, data });
        }

        [HttpPost]
        public async Task<IActionResult> SaveUploadContent(string title, int contentTypeId, string? videoLink, IFormFile? file)
        {
            var companyId = GetCompanyId();
            var userId = GetUserId();

            string fileType = "Link";
            string fileName = "";
            string filePath = videoLink ?? "";
            string fileSize = "N/A";

            if (file != null)
            {
                fileType = "File";
                fileName = file.FileName;
                fileSize = (file.Length / 1024.0 / 1024.0).ToString("0.##") + " MB";

                var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "download_center");
                if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var path = Path.Combine(uploadDir, uniqueFileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                filePath = "/uploads/download_center/" + uniqueFileName;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return Json(new { success = false, message = "Please upload a file or provide a video link." });
            }

            var res = ((DownloadCenterService)_service).SaveUploadContent(title, contentTypeId, fileType, fileName, filePath, fileSize, companyId, userId);
            return Json(new { success = res.Success, message = res.Message });
        }

        [HttpPost]
        public IActionResult DeleteContent(int id)
        {
            var res = _service.DeleteContent(id, GetUserId());
            return Json(new { success = res.Success, message = res.Message });
        }

        [HttpPost]
        public IActionResult GenerateSharedLink([FromBody] SharedLinkUpsertRequest req)
        {
            var res = _service.GenerateSharedLink(req, GetCompanyId(), GetUserId());
            return Json(new { success = res.Success, message = res.Message, token = res.Token });
        }

        

        [HttpGet]
        public IActionResult GetSharedLinkList()
        {
            var data = _service.GetSharedLinkList(GetCompanyId());
            return Json(new { success = true, data });
        }

        [HttpPost]
        public IActionResult DeleteSharedLink(int id)
        {
            var res = _service.DeleteSharedLink(id);
            return Json(new { success = res.Success, message = res.Message });
        }
        #endregion

        [HttpGet]
        public IActionResult GetSections(int classId)
        {
            var sections = _sectionService.GetSectionsByClass(classId);
            return Json(sections);
        }

        public IActionResult ContentShareList()
        {
            return View();
        }
    }
}
