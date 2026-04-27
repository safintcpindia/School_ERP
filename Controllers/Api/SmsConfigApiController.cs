using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for configuring the SMS gateway settings through the API.
    /// </summary>
    public class SmsConfigApiController : ControllerBase
    {
        private readonly ISmsConfigService _smsConfigService;

        public SmsConfigApiController(ISmsConfigService smsConfigService)
        {
            _smsConfigService = smsConfigService;
        }

        /// <summary>
        /// Gets the current SMS gateway settings (like the API URL and keys) from the system.
        /// </summary>
        [HttpGet("Get")]
        public IActionResult GetConfig()
        {
            try
            {
                var config = _smsConfigService.GetSmsConfig();
                return Ok(new { success = true, data = config });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Saves or updates the SMS gateway settings with the information you provided.
        /// </summary>
        [HttpPost("Upsert")]
        public IActionResult UpsertConfig([FromBody] MstSmsConfigUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data format or missing required payload parameters" });
                }

                // Identify the requester for audit traceability mappings
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userId = string.IsNullOrEmpty(userIdStr) ? 0 : int.Parse(userIdStr);

                var (success, message) = _smsConfigService.UpsertSmsConfig(request, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
