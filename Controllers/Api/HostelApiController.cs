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
    public class HostelApiController : ControllerBase
    {
        private readonly IHostelService _hostelService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;

        public HostelApiController(IHostelService hostelService, ICompanyService companySvc, ISessionService sessionSvc)
        {
            _hostelService = hostelService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
        }

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
        private int CompanyId => _companySvc.GetUserCurrentCompany(UserId) ?? 0;
        private int SessionId => _sessionSvc.GetUserCurrentSession(UserId) ?? 0;

        [HttpGet("GetAllRoomTypes")]
        public IActionResult GetAllRoomTypes(bool includeDeleted = false)
        {
            try
            {
                var data = _hostelService.GetAllRoomTypes(CompanyId, SessionId, includeDeleted);
                return Ok(ApiResponse<List<RoomTypeViewModel>>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<List<RoomTypeViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetRoomTypeByID/{id}")]
        public IActionResult GetRoomTypeByID(int id)
        {
            try
            {
                var data = _hostelService.GetRoomTypeByID(id);
                if (data == null) return NotFound(ApiResponse<RoomTypeViewModel>.ErrorResponse("Not found"));
                return Ok(ApiResponse<RoomTypeViewModel>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<RoomTypeViewModel>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("UpsertRoomType")]
        public IActionResult UpsertRoomType([FromBody] RoomTypeUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = _hostelService.UpsertRoomType(req, CompanyId, SessionId, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("DeleteRoomType/{id}")]
        public IActionResult DeleteRoomType(int id)
        {
            try
            {
                var result = _hostelService.DeleteRoomType(id, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("ToggleRoomTypeStatus")]
        public IActionResult ToggleRoomTypeStatus(int id, bool isActive)
        {
            try
            {
                var result = _hostelService.ToggleRoomTypeStatus(id, isActive, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // ─── HOSTEL ─────────────────────────────────────────────
        [HttpGet("GetAllHostels")]
        public IActionResult GetAllHostels(bool includeDeleted = false)
        {
            try
            {
                var data = _hostelService.GetAllHostels(CompanyId, SessionId, includeDeleted);
                return Ok(ApiResponse<List<HostelViewModel>>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<List<HostelViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetHostelByID/{id}")]
        public IActionResult GetHostelByID(int id)
        {
            try
            {
                var data = _hostelService.GetHostelByID(id);
                if (data == null) return NotFound(ApiResponse<HostelViewModel>.ErrorResponse("Not found"));
                return Ok(ApiResponse<HostelViewModel>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<HostelViewModel>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("UpsertHostel")]
        public IActionResult UpsertHostel([FromBody] HostelUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = _hostelService.UpsertHostel(req, CompanyId, SessionId, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("DeleteHostel/{id}")]
        public IActionResult DeleteHostel(int id)
        {
            try
            {
                var result = _hostelService.DeleteHostel(id, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("ToggleHostelStatus")]
        public IActionResult ToggleHostelStatus(int id, bool isActive)
        {
            try
            {
                var result = _hostelService.ToggleHostelStatus(id, isActive, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // ─── HOSTEL ROOM ────────────────────────────────────────
        [HttpGet("GetAllHostelRooms")]
        public IActionResult GetAllHostelRooms(bool includeDeleted = false)
        {
            try
            {
                var data = _hostelService.GetAllHostelRooms(CompanyId, SessionId, includeDeleted);
                return Ok(ApiResponse<List<HostelRoomViewModel>>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<List<HostelRoomViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetHostelRoomByID/{id}")]
        public IActionResult GetHostelRoomByID(int id)
        {
            try
            {
                var data = _hostelService.GetHostelRoomByID(id);
                if (data == null) return NotFound(ApiResponse<HostelRoomViewModel>.ErrorResponse("Not found"));
                return Ok(ApiResponse<HostelRoomViewModel>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<HostelRoomViewModel>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("UpsertHostelRoom")]
        public IActionResult UpsertHostelRoom([FromBody] HostelRoomUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = _hostelService.UpsertHostelRoom(req, CompanyId, SessionId, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("DeleteHostelRoom/{id}")]
        public IActionResult DeleteHostelRoom(int id)
        {
            try
            {
                var result = _hostelService.DeleteHostelRoom(id, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("ToggleHostelRoomStatus")]
        public IActionResult ToggleHostelRoomStatus(int id, bool isActive)
        {
            try
            {
                var result = _hostelService.ToggleHostelRoomStatus(id, isActive, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
