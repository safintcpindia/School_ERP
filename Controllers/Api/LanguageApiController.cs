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
    /// <summary>
    /// Feeds linguistic definitions mapped across global string dictionaries.
    /// </summary>
    public class LanguageApiController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguageApiController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        /// <summary>
        /// Reads installed localization branches available to users.
        /// </summary>
        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false)
        {
            var data = _languageService.GetAllLanguages(includeDeleted);
            return Ok(ApiResponse<List<MstLanguageViewModel>>.SuccessResponse(data));
        }

        /// <summary>
        /// Extrapolates detailed metadata for a single specific dialect block.
        /// </summary>
        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _languageService.GetLanguageByID(id);
            if (data == null) return NotFound(ApiResponse<MstLanguageViewModel>.ErrorResponse("Language identifier nullified"));
            return Ok(ApiResponse<MstLanguageViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Implements a new localized language into translation dependencies.
        /// </summary>
        [HttpPost("Upsert")]
        public IActionResult Upsert([FromBody] MstLanguageUpsertRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int userId = GetCurrentUserId();
            var (success, message) = _languageService.UpsertLanguage(request, userId);
            if (success) return Ok(ApiResponse<dynamic>.SuccessResponse(new { success, message }));
            return BadRequest(ApiResponse<dynamic>.ErrorResponse(message));
        }

        /// <summary>
        /// Completely deletes localized pointers from dialect graphs natively.
        /// </summary>
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _languageService.DeleteLanguage(id, userId);
            if (success) return Ok(ApiResponse<dynamic>.SuccessResponse(new { success, message }));
            return BadRequest(ApiResponse<dynamic>.ErrorResponse(message));
        }

        /// <summary>
        /// Masks locale choices from end-users dynamically.
        /// </summary>
        [HttpPost("ToggleStatus")]
        public IActionResult ToggleStatus(int id, bool isActive)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _languageService.ToggleLanguageStatus(id, isActive, userId);
            if (success) return Ok(ApiResponse<dynamic>.SuccessResponse(new { success, message }));
            return BadRequest(ApiResponse<dynamic>.ErrorResponse(message));
        }

        /// <summary>
        /// Extracts user identifier securely bypassing payload vulnerabilities.
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1; 
        }
    }
}
