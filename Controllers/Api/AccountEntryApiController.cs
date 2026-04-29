using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountEntryApiController : ControllerBase
    {
        private readonly IAccountEntryService _accountEntryService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;

        public AccountEntryApiController(IAccountEntryService accountEntryService, ICompanyService companySvc, ISessionService sessionSvc)
        {
            _accountEntryService = accountEntryService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
        }

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
        private int CompanyId => _companySvc.GetUserCurrentCompany(UserId) ?? 0;
        private int SessionId => _sessionSvc.GetUserCurrentSession(UserId) ?? 0;

        [HttpGet("GetAllAccountEntries")]
        public IActionResult GetAllAccountEntries(string entryType, bool includeDeleted = false)
        {
            try
            {
                var data = _accountEntryService.GetAllAccountEntries(CompanyId, SessionId, entryType, includeDeleted);
                return Ok(ApiResponse<List<AccountEntryViewModel>>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<List<AccountEntryViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("SearchAccountEntries")]
        public IActionResult SearchAccountEntries([FromBody] AccountEntrySearchRequest req)
        {
            try
            {
                var data = _accountEntryService.SearchAccountEntries(CompanyId, SessionId, req.EntryType, req.SearchType, req.DateFrom, req.DateTo);
                return Ok(ApiResponse<List<AccountEntryViewModel>>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<List<AccountEntryViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetAccountEntryByID/{id}")]
        public IActionResult GetAccountEntryByID(int id)
        {
            try
            {
                var data = _accountEntryService.GetAccountEntryByID(id);
                if (data == null) return NotFound(ApiResponse<AccountEntryViewModel>.ErrorResponse("Not found"));
                return Ok(ApiResponse<AccountEntryViewModel>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<AccountEntryViewModel>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("UpsertAccountEntry")]
        public IActionResult UpsertAccountEntry([FromBody] AccountEntryUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = _accountEntryService.UpsertAccountEntry(req, CompanyId, SessionId, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("DeleteAccountEntry/{id}")]
        public IActionResult DeleteAccountEntry(int id)
        {
            try
            {
                var result = _accountEntryService.DeleteAccountEntry(id, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("ToggleAccountEntryStatus")]
        public IActionResult ToggleAccountEntryStatus(int id, bool isActive)
        {
            try
            {
                var result = _accountEntryService.ToggleAccountEntryStatus(id, isActive, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
