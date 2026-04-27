using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;
using System.Collections.Generic;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for managing system translations and localization settings through the API.
    /// </summary>
    public class SettingsApiController : ControllerBase
    {
        private readonly ILocalizationService _localizationService;
        private readonly IUserManagementService _userMgmtService;

        public SettingsApiController(ILocalizationService localizationService, IUserManagementService userMgmtService)
        {
            _localizationService = localizationService;
            _userMgmtService = userMgmtService;
        }

        /// <summary>
        /// Gets all the translated words and phrases for a specific language (like English or Hindi).
        /// </summary>
        [HttpGet("translations/{language}")]
        public IActionResult GetTranslations(string language)
        {
            var translations = _localizationService.GetTranslations(language);
            return Ok(ApiResponse<Dictionary<string, string>>.SuccessResponse(translations));
        }

        /// <summary>
        /// Changes the translation for one specific word or phrase in a chosen language.
        /// </summary>
        [HttpPost("translations/update")]
        public IActionResult UpdateTranslation([FromBody] TranslationUpdateModel model)
        {
            try
            {
                var translations = _localizationService.GetTranslations(model.Language);
                
                // Ensure key exists before modifying to prevent uncontrolled registry bloating
                if (translations.ContainsKey(model.Key))
                {
                    translations[model.Key] = model.Value;
                    _localizationService.SaveTranslations(model.Language, translations);
                    return Ok(ApiResponse<bool>.SuccessResponse(true));
                }
                return NotFound(ApiResponse<bool>.ErrorResponse("Key not found"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        // Add other settings endpoints as needed (General, SMS, Email, etc.)
        // For now, mirroring what's in SettingsController
    }
}
