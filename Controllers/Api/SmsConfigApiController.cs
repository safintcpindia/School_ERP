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
    //[Authorize]
    /// <summary>
    /// This class handles HTTP routing and API requests for SmsConfigApiController.
    /// </summary>
    public class SmsConfigApiController : ControllerBase
    {
        private readonly ISmsConfigService _smsConfigService;

        public SmsConfigApiController(ISmsConfigService smsConfigService)
        {
            _smsConfigService = smsConfigService;
        }

        /// <summary>
        /// Extracts active API routes and integration keys for SMS transmission modules.
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
        /// Applies new parameters for an SMS service provider integration.
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
