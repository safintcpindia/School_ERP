using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Services.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    public class FrontOfficeController : Controller
    {
        private readonly IFrontOfficeClientService _client;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/FrontOffice/Setup";
        private const string ComplaintMenuPath = "/FrontOffice/Complaint";

        public FrontOfficeController(IFrontOfficeClientService client, IUserMenuPermissionService menuPerm)
        {
            _client = client;
            _menuPerm = menuPerm;
        }

        public async Task<IActionResult> Setup()
        {
            var purposes        = await _client.GetAllPurposesAsync();
            var complaintTypes  = await _client.GetAllComplaintTypesAsync();
            var sources         = await _client.GetAllSourcesAsync();
            var references      = await _client.GetAllReferencesAsync();

            var model = new FrontOfficeSetupPageViewModel
            {
                Purposes       = purposes.Success       ? purposes.Data       : new List<MstFOPurposeViewModel>(),
                ComplaintTypes = complaintTypes.Success  ? complaintTypes.Data  : new List<MstFOComplaintTypeViewModel>(),
                Sources        = sources.Success         ? sources.Data         : new List<MstFOSourceViewModel>(),
                References     = references.Success      ? references.Data      : new List<MstFOReferenceViewModel>()
            };

            return View(model);
        }

        // ─── PURPOSE ────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetPurpose(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.GetPurposeByIDAsync(id);
            if (!r.Success) return Json(new { success = false, message = r.Message });
            return Json(new { success = true, data = r.Data });
        }

        [HttpPost]
        public async Task<IActionResult> SavePurpose([FromBody] MstFOPurposeUpsertRequest req)
        {
            bool isCreate = req.PurposeID <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Json(new { success = false, message = "Permission denied." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.UpsertPurposeAsync(req);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePurpose(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Delete"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.DeletePurposeAsync(id);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> TogglePurposeStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.TogglePurposeStatusAsync(id, isActive);
            return Json(new { success = r.Success, message = r.Message });
        }

        // ─── COMPLAINT TYPE ─────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetComplaintType(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.GetComplaintTypeByIDAsync(id);
            if (!r.Success) return Json(new { success = false, message = r.Message });
            return Json(new { success = true, data = r.Data });
        }

        [HttpPost]
        public async Task<IActionResult> SaveComplaintType([FromBody] MstFOComplaintTypeUpsertRequest req)
        {
            bool isCreate = req.ComplaintTypeID <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Json(new { success = false, message = "Permission denied." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.UpsertComplaintTypeAsync(req);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComplaintType(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Delete"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.DeleteComplaintTypeAsync(id);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleComplaintTypeStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.ToggleComplaintTypeStatusAsync(id, isActive);
            return Json(new { success = r.Success, message = r.Message });
        }

        // ─── SOURCE ─────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetSource(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.GetSourceByIDAsync(id);
            if (!r.Success) return Json(new { success = false, message = r.Message });
            return Json(new { success = true, data = r.Data });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSource([FromBody] MstFOSourceUpsertRequest req)
        {
            bool isCreate = req.SourceID <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Json(new { success = false, message = "Permission denied." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.UpsertSourceAsync(req);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSource(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Delete"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.DeleteSourceAsync(id);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleSourceStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.ToggleSourceStatusAsync(id, isActive);
            return Json(new { success = r.Success, message = r.Message });
        }

        // ─── REFERENCE ──────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetReference(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.GetReferenceByIDAsync(id);
            if (!r.Success) return Json(new { success = false, message = r.Message });
            return Json(new { success = true, data = r.Data });
        }

        [HttpPost]
        public async Task<IActionResult> SaveReference([FromBody] MstFOReferenceUpsertRequest req)
        {
            bool isCreate = req.ReferenceID <= 0;
            if (isCreate && !_menuPerm.Has(User, MenuPath, "Add"))
                return Json(new { success = false, message = "Permission denied." });
            if (!isCreate && !_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.UpsertReferenceAsync(req);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReference(int id)
        {
            if (!_menuPerm.Has(User, MenuPath, "Delete"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.DeleteReferenceAsync(id);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleReferenceStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, MenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.ToggleReferenceStatusAsync(id, isActive);
            return Json(new { success = r.Success, message = r.Message });
        }

        public async Task<IActionResult> Complaint()
        {
            var complaints      = await _client.GetAllComplaintsAsync();
            var complaintTypes  = await _client.GetAllComplaintTypesAsync();
            var sources         = await _client.GetAllSourcesAsync();

            var model = new FOComplaintPageViewModel
            {
                Complaints     = complaints.Success      ? complaints.Data      : new List<FOComplaintViewModel>(),
                ComplaintTypes = complaintTypes.Success  ? complaintTypes.Data  : new List<MstFOComplaintTypeViewModel>(),
                Sources        = sources.Success         ? sources.Data         : new List<MstFOSourceViewModel>()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetComplaint(int id)
        {
            if (!_menuPerm.Has(User, ComplaintMenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.GetComplaintByIDAsync(id);
            return Json(new { success = r.Success, message = r.Message, data = r.Data });
        }

        [HttpPost]
        public async Task<IActionResult> SaveComplaint([FromBody] FOComplaintUpsertRequest req)
        {
            bool isCreate = req.ComplaintID <= 0;
            if (isCreate && !_menuPerm.Has(User, ComplaintMenuPath, "Add"))
                return Json(new { success = false, message = "Permission denied." });
            if (!isCreate && !_menuPerm.Has(User, ComplaintMenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.UpsertComplaintAsync(req);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComplaint(int id)
        {
            if (!_menuPerm.Has(User, ComplaintMenuPath, "Delete"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.DeleteComplaintAsync(id);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleComplaintStatus(int id, bool isActive)
        {
            if (!_menuPerm.Has(User, ComplaintMenuPath, "Edit"))
                return Json(new { success = false, message = "Permission denied." });
            var r = await _client.ToggleComplaintStatusAsync(id, isActive);
            return Json(new { success = r.Success, message = r.Message });
        }

        public async Task<IActionResult> PostalReceive()
        {
            var res = await _client.GetAllPostalReceivesAsync();
            var model = new FOPostalReceivePageViewModel
            {
                Items = res.Success ? res.Data : new List<FOPostalReceiveViewModel>()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetPostalReceive(int id)
        {
            var r = await _client.GetPostalReceiveByIDAsync(id);
            return Json(new { success = r.Success, message = r.Message, data = r.Data });
        }

        [HttpPost]
        public async Task<IActionResult> SavePostalReceive([FromForm] FOPostalReceiveUpsertRequest req, IFormFile? attachmentFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return Json(new { success = false, message = "Validation Error: " + errors });
                }

                if (attachmentFile != null && attachmentFile.Length > 0)
                {
                    req.FileName = attachmentFile.FileName;
                    req.FileType = attachmentFile.ContentType;
                    using (var ms = new MemoryStream())
                    {
                        await attachmentFile.CopyToAsync(ms);
                        req.Attachment = ms.ToArray();
                    }
                }
                var r = await _client.UpsertPostalReceiveAsync(req);
                return Json(new { success = r.Success, message = r.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePostalReceive(int id)
        {
            var r = await _client.DeletePostalReceiveAsync(id);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostalReceiveStatus(int id, bool isActive)
        {
            var r = await _client.TogglePostalReceiveStatusAsync(id, isActive);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(int id)
        {
            var r = await _client.GetPostalReceiveByIDAsync(id);
            if (!r.Success || r.Data?.Attachment == null) return NotFound();
            
            string contentType = r.Data.FileType ?? "application/octet-stream";
            string fileName = r.Data.FileName ?? $"attachment_{id}";
            
            return File(r.Data.Attachment, contentType, fileName);
        }

        public async Task<IActionResult> PostalDispatch()
        {
            var res = await _client.GetAllPostalDispatchesAsync();
            var model = new FOPostalDispatchPageViewModel
            {
                Items = res.Success ? res.Data : new List<FOPostalDispatchViewModel>()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetPostalDispatch(int id)
        {
            var r = await _client.GetPostalDispatchByIDAsync(id);
            return Json(new { success = r.Success, message = r.Message, data = r.Data });
        }

        [HttpPost]
        public async Task<IActionResult> SavePostalDispatch([FromForm] FOPostalDispatchUpsertRequest req, IFormFile? attachmentFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return Json(new { success = false, message = "Validation Error: " + errors });
                }

                if (attachmentFile != null && attachmentFile.Length > 0)
                {
                    req.FileName = attachmentFile.FileName;
                    req.FileType = attachmentFile.ContentType;
                    using (var ms = new MemoryStream())
                    {
                        await attachmentFile.CopyToAsync(ms);
                        req.Attachment = ms.ToArray();
                    }
                }
                var r = await _client.UpsertPostalDispatchAsync(req);
                return Json(new { success = r.Success, message = r.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePostalDispatch(int id)
        {
            var r = await _client.DeletePostalDispatchAsync(id);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostalDispatchStatus(int id, bool isActive)
        {
            var r = await _client.TogglePostalDispatchStatusAsync(id, isActive);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDispatchAttachment(int id)
        {
            var r = await _client.GetPostalDispatchByIDAsync(id);
            if (!r.Success || r.Data?.Attachment == null) return NotFound();

            string contentType = r.Data.FileType ?? "application/octet-stream";
            string fileName = r.Data.FileName ?? $"dispatch_attachment_{id}";

            return File(r.Data.Attachment, contentType, fileName);
        }

        public async Task<IActionResult> PhoneCallLog()
        {
            var res = await _client.GetAllPhoneCallLogsAsync();
            var model = new FOPhoneCallLogPageViewModel
            {
                Items = res.Success ? res.Data : new List<FOPhoneCallLogViewModel>()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetPhoneCallLog(int id)
        {
            var r = await _client.GetPhoneCallLogByIDAsync(id);
            return Json(new { success = r.Success, message = r.Message, data = r.Data });
        }

        [HttpPost]
        public async Task<IActionResult> SavePhoneCallLog([FromBody] FOPhoneCallLogUpsertRequest req)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = "Validation Error: " + errors });
            }
            var r = await _client.UpsertPhoneCallLogAsync(req);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoneCallLog(int id)
        {
            var r = await _client.DeletePhoneCallLogAsync(id);
            return Json(new { success = r.Success, message = r.Message });
        }

        [HttpPost]
        public async Task<IActionResult> TogglePhoneCallLogStatus(int id, bool isActive)
        {
            var r = await _client.TogglePhoneCallLogStatusAsync(id, isActive);
            return Json(new { success = r.Success, message = r.Message });
        }
    }
}
