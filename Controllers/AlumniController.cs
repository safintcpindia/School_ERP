using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Services;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Controllers
{
    public class AlumniController : Controller
    {
        private readonly IAlumniEventService _eventService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IClassService _classService;

        public AlumniController(IAlumniEventService eventService, ICompanyService companyService, ISessionService sessionService, IClassService classService)
        {
            _eventService = eventService;
            _companyService = companyService;
            _sessionService = sessionService;
            _classService = classService;
        }

        private int GetUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value ?? "0");
        private int GetCompanyId() => _companyService.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionService.GetUserCurrentSession(GetUserId()) ?? 0;

        public IActionResult Events()
        {
            ViewBag.SessionList = _sessionService.GetAllSessions();
            ViewBag.ClassList = _classService.GetAllClasses(GetCompanyId(), GetSessionId());
            return View();
        }

        [HttpGet]
        public IActionResult GetEvents(string? searchText)
        {
            var data = _eventService.GetEvents(searchText, GetCompanyId());
            return Json(new { success = true, data });
        }

        [HttpPost]
        public IActionResult UpsertEvent([FromForm] int EventID, [FromForm] string EventTitle, [FromForm] string? EventDescription,
                                        [FromForm] DateTime FromDate, [FromForm] DateTime ToDate, [FromForm] string? Location,
                                        [FromForm] int EventFor, [FromForm] int? SessionID, [FromForm] int? ClassID, [FromForm] string? SectionIDs,
                                        IFormFile? EventPhoto)
        {
            var req = new AlumniEventUpsertRequest
            {
                EventID = EventID,
                EventTitle = EventTitle,
                EventDescription = EventDescription,
                FromDate = FromDate,
                ToDate = ToDate,
                Location = Location,
                EventFor = EventFor,
                SessionID = SessionID,
                ClassID = ClassID,
                SectionIDs = SectionIDs
            };

            if (EventPhoto != null)
            {
                using (var ms = new MemoryStream())
                {
                    EventPhoto.CopyTo(ms);
                    req.EventPhoto = ms.ToArray();
                    req.EventPhotoName = EventPhoto.FileName;
                    req.EventPhotoType = EventPhoto.ContentType;
                }
            }

            var result = _eventService.UpsertEvent(req, GetCompanyId(), GetUserId());
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public IActionResult DeleteEvent(int id)
        {
            var result = _eventService.DeleteEvent(id, GetCompanyId());
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public IActionResult ToggleStatus(int id, bool isActive)
        {
            var result = _eventService.ToggleEventStatus(id, isActive, GetCompanyId());
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpGet]
        public IActionResult DownloadPhoto(int id)
        {
            var (bytes, fileName, contentType) = _eventService.GetEventPhoto(id, GetCompanyId());
            if (bytes == null) return NotFound();
            return File(bytes, contentType ?? "image/jpeg", fileName ?? "event_photo");
        }
    }
}
