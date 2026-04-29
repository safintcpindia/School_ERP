using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;
using System.Collections.Generic;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FrontOfficeApiController : ControllerBase
    {
        private readonly IFrontOfficeService _svc;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;

        public FrontOfficeApiController(IFrontOfficeService svc, ICompanyService companySvc, ISessionService sessionSvc)
        {
            _svc = svc;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
        }

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
        private int CompanyId => _companySvc.GetUserCurrentCompany(UserId) ?? 0;
        private int SessionId => _sessionSvc.GetUserCurrentSession(UserId) ?? 0;

        // ─── PURPOSE ────────────────────────────────────────────
        [HttpGet("GetAllPurposes")]
        public IActionResult GetAllPurposes(bool includeDeleted = false)
        {
            var data = _svc.GetAllPurposes(CompanyId, SessionId, includeDeleted);
            return Ok(ApiResponse<List<MstFOPurposeViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetPurposeByID/{id}")]
        public IActionResult GetPurposeByID(int id)
        {
            var data = _svc.GetPurposeByID(id);
            if (data == null) return NotFound(ApiResponse<MstFOPurposeViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<MstFOPurposeViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertPurpose")]
        public IActionResult UpsertPurpose([FromBody] MstFOPurposeUpsertRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, message) = _svc.UpsertPurpose(req, CompanyId, SessionId, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("DeletePurpose/{id}")]
        public IActionResult DeletePurpose(int id)
        {
            var (success, message) = _svc.DeletePurpose(id, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("TogglePurposeStatus")]
        public IActionResult TogglePurposeStatus(int id, bool isActive)
        {
            var (success, message) = _svc.TogglePurposeStatus(id, isActive, UserId);
            return Ok(new { success, message });
        }

        // ─── COMPLAINT TYPE ─────────────────────────────────────
        [HttpGet("GetAllComplaintTypes")]
        public IActionResult GetAllComplaintTypes(bool includeDeleted = false)
        {
            var data = _svc.GetAllComplaintTypes(CompanyId, SessionId, includeDeleted);
            return Ok(ApiResponse<List<MstFOComplaintTypeViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetComplaintTypeByID/{id}")]
        public IActionResult GetComplaintTypeByID(int id)
        {
            var data = _svc.GetComplaintTypeByID(id);
            if (data == null) return NotFound(ApiResponse<MstFOComplaintTypeViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<MstFOComplaintTypeViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertComplaintType")]
        public IActionResult UpsertComplaintType([FromBody] MstFOComplaintTypeUpsertRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, message) = _svc.UpsertComplaintType(req, CompanyId, SessionId, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("DeleteComplaintType/{id}")]
        public IActionResult DeleteComplaintType(int id)
        {
            var (success, message) = _svc.DeleteComplaintType(id, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("ToggleComplaintTypeStatus")]
        public IActionResult ToggleComplaintTypeStatus(int id, bool isActive)
        {
            var (success, message) = _svc.ToggleComplaintTypeStatus(id, isActive, UserId);
            return Ok(new { success, message });
        }

        // ─── SOURCE ─────────────────────────────────────────────
        [HttpGet("GetAllSources")]
        public IActionResult GetAllSources(bool includeDeleted = false)
        {
            var data = _svc.GetAllSources(CompanyId, SessionId, includeDeleted);
            return Ok(ApiResponse<List<MstFOSourceViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetSourceByID/{id}")]
        public IActionResult GetSourceByID(int id)
        {
            var data = _svc.GetSourceByID(id);
            if (data == null) return NotFound(ApiResponse<MstFOSourceViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<MstFOSourceViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertSource")]
        public IActionResult UpsertSource([FromBody] MstFOSourceUpsertRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, message) = _svc.UpsertSource(req, CompanyId, SessionId, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("DeleteSource/{id}")]
        public IActionResult DeleteSource(int id)
        {
            var (success, message) = _svc.DeleteSource(id, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("ToggleSourceStatus")]
        public IActionResult ToggleSourceStatus(int id, bool isActive)
        {
            var (success, message) = _svc.ToggleSourceStatus(id, isActive, UserId);
            return Ok(new { success, message });
        }

        // ─── REFERENCE ──────────────────────────────────────────
        [HttpGet("GetAllReferences")]
        public IActionResult GetAllReferences(bool includeDeleted = false)
        {
            var data = _svc.GetAllReferences(CompanyId, SessionId, includeDeleted);
            return Ok(ApiResponse<List<MstFOReferenceViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetReferenceByID/{id}")]
        public IActionResult GetReferenceByID(int id)
        {
            var data = _svc.GetReferenceByID(id);
            if (data == null) return NotFound(ApiResponse<MstFOReferenceViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<MstFOReferenceViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertReference")]
        public IActionResult UpsertReference([FromBody] MstFOReferenceUpsertRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, message) = _svc.UpsertReference(req, CompanyId, SessionId, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("DeleteReference/{id}")]
        public IActionResult DeleteReference(int id)
        {
            var (success, message) = _svc.DeleteReference(id, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("ToggleReferenceStatus")]
        public IActionResult ToggleReferenceStatus(int id, bool isActive)
        {
            var (success, message) = _svc.ToggleReferenceStatus(id, isActive, UserId);
            return Ok(new { success, message });
        }
        
        // ─── COMPLAINT ──────────────────────────────────────────
        [HttpGet("GetAllComplaints")]
        public IActionResult GetAllComplaints(bool includeDeleted = false)
        {
            var data = _svc.GetAllComplaints(CompanyId, SessionId, includeDeleted);
            return Ok(ApiResponse<List<FOComplaintViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetComplaintByID/{id}")]
        public IActionResult GetComplaintByID(int id)
        {
            var data = _svc.GetComplaintByID(id);
            if (data == null) return NotFound(ApiResponse<FOComplaintViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<FOComplaintViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertComplaint")]
        public IActionResult UpsertComplaint([FromBody] FOComplaintUpsertRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, message) = _svc.UpsertComplaint(req, CompanyId, SessionId, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("DeleteComplaint/{id}")]
        public IActionResult DeleteComplaint(int id)
        {
            var (success, message) = _svc.DeleteComplaint(id, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("ToggleComplaintStatus")]
        public IActionResult ToggleComplaintStatus(int id, bool isActive)
        {
            var (success, message) = _svc.ToggleComplaintStatus(id, isActive, UserId);
            return Ok(new { success, message });
        }

        // ─── POSTAL RECEIVE ─────────────────────────────────────
        [HttpGet("GetAllPostalReceives")]
        public IActionResult GetAllPostalReceives(bool includeDeleted = false)
        {
            var data = _svc.GetAllPostalReceives(CompanyId, SessionId, includeDeleted);
            return Ok(ApiResponse<List<FOPostalReceiveViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetPostalReceiveByID/{id}")]
        public IActionResult GetPostalReceiveByID(int id)
        {
            var data = _svc.GetPostalReceiveByID(id);
            if (data == null) return NotFound(ApiResponse<FOPostalReceiveViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<FOPostalReceiveViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertPostalReceive")]
        public IActionResult UpsertPostalReceive([FromBody] FOPostalReceiveUpsertRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, message) = _svc.UpsertPostalReceive(req, CompanyId, SessionId, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("DeletePostalReceive/{id}")]
        public IActionResult DeletePostalReceive(int id)
        {
            var (success, message) = _svc.DeletePostalReceive(id, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("TogglePostalReceiveStatus")]
        public IActionResult TogglePostalReceiveStatus(int id, bool isActive)
        {
            var (success, message) = _svc.TogglePostalReceiveStatus(id, isActive, UserId);
            return Ok(new { success, message });
        }

        // ─── POSTAL DISPATCH ─────────────────────────────────────
        [HttpGet("GetAllPostalDispatches")]
        public IActionResult GetAllPostalDispatches(bool includeDeleted = false)
        {
            var data = _svc.GetAllPostalDispatches(CompanyId, SessionId, includeDeleted);
            return Ok(ApiResponse<List<FOPostalDispatchViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetPostalDispatchByID/{id}")]
        public IActionResult GetPostalDispatchByID(int id)
        {
            var data = _svc.GetPostalDispatchByID(id);
            if (data == null) return NotFound(ApiResponse<FOPostalDispatchViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<FOPostalDispatchViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertPostalDispatch")]
        public IActionResult UpsertPostalDispatch([FromBody] FOPostalDispatchUpsertRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, message) = _svc.UpsertPostalDispatch(req, CompanyId, SessionId, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("DeletePostalDispatch/{id}")]
        public IActionResult DeletePostalDispatch(int id)
        {
            var (success, message) = _svc.DeletePostalDispatch(id, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("TogglePostalDispatchStatus")]
        public IActionResult TogglePostalDispatchStatus(int id, bool isActive)
        {
            var (success, message) = _svc.TogglePostalDispatchStatus(id, isActive, UserId);
            return Ok(new { success, message });
        }

        // ─── PHONE CALL LOG ─────────────────────────────────────
        [HttpGet("GetAllPhoneCallLogs")]
        public IActionResult GetAllPhoneCallLogs(bool includeDeleted = false)
        {
            var data = _svc.GetAllPhoneCallLogs(CompanyId, SessionId, includeDeleted);
            return Ok(ApiResponse<List<FOPhoneCallLogViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetPhoneCallLogByID/{id}")]
        public IActionResult GetPhoneCallLogByID(int id)
        {
            var data = _svc.GetPhoneCallLogByID(id);
            if (data == null) return NotFound(ApiResponse<FOPhoneCallLogViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<FOPhoneCallLogViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertPhoneCallLog")]
        public IActionResult UpsertPhoneCallLog([FromBody] FOPhoneCallLogUpsertRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, message) = _svc.UpsertPhoneCallLog(req, CompanyId, SessionId, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("DeletePhoneCallLog/{id}")]
        public IActionResult DeletePhoneCallLog(int id)
        {
            var (success, message) = _svc.DeletePhoneCallLog(id, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("TogglePhoneCallLogStatus")]
        public IActionResult TogglePhoneCallLogStatus(int id, bool isActive)
        {
            var (success, message) = _svc.TogglePhoneCallLogStatus(id, isActive, UserId);
            return Ok(new { success, message });
        }
    }
}
