using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using SchoolERP.Net.Services;
using System.Collections.Generic;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    /// <summary>
    /// This class handles HTTP routing and API requests for SettingsApiController.
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
        /// Pulls the complete localized dictionary for a specified ISO language code.
        /// </summary>
        /// <param name="language">e.g., 'en_US' or 'fr_FR'</param>
        [HttpGet("translations/{language}")]
        public IActionResult GetTranslations(string language)
        {
            var translations = _localizationService.GetTranslations(language);
            return Ok(ApiResponse<Dictionary<string, string>>.SuccessResponse(translations));
        }

        /// <summary>
        /// Mutates a single translated string definition in the backend JSON catalog.
        /// </summary>
        /// <param name="model">Contains the Language locale, Dictionary Key, and new Text Value.</param>
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
