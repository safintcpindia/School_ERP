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
    public class AccountHeadApiController : ControllerBase
    {
        private readonly IAccountHeadService _accountHeadService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;

        public AccountHeadApiController(IAccountHeadService accountHeadService, ICompanyService companySvc, ISessionService sessionSvc)
        {
            _accountHeadService = accountHeadService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
        }

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
        private int CompanyId => _companySvc.GetUserCurrentCompany(UserId) ?? 0;
        private int SessionId => _sessionSvc.GetUserCurrentSession(UserId) ?? 0;

        [HttpGet("GetAllAccountHeads")]
        public IActionResult GetAllAccountHeads(string headType, bool includeDeleted = false)
        {
            try
            {
                var data = _accountHeadService.GetAllAccountHeads(CompanyId, SessionId, headType, includeDeleted);
                return Ok(ApiResponse<List<AccountHeadViewModel>>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<List<AccountHeadViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetAccountHeadByID/{id}")]
        public IActionResult GetAccountHeadByID(int id)
        {
            try
            {
                var data = _accountHeadService.GetAccountHeadByID(id);
                if (data == null) return NotFound(ApiResponse<AccountHeadViewModel>.ErrorResponse("Not found"));
                return Ok(ApiResponse<AccountHeadViewModel>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<AccountHeadViewModel>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("UpsertAccountHead")]
        public IActionResult UpsertAccountHead([FromBody] AccountHeadUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = _accountHeadService.UpsertAccountHead(req, CompanyId, SessionId, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("DeleteAccountHead/{id}")]
        public IActionResult DeleteAccountHead(int id)
        {
            try
            {
                var result = _accountHeadService.DeleteAccountHead(id, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("ToggleAccountHeadStatus")]
        public IActionResult ToggleAccountHeadStatus(int id, bool isActive)
        {
            try
            {
                var result = _accountHeadService.ToggleAccountHeadStatus(id, isActive, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
