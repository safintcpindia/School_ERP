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
        private readonly IFieldService _fieldService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;

        public SettingsApiController(
            ILocalizationService localizationService, 
            IUserManagementService userMgmtService, 
            IFieldService fieldService,
            ICompanyService companyService,
            ISessionService sessionService)
        {
            _localizationService = localizationService;
            _userMgmtService = userMgmtService;
            _fieldService = fieldService;
            _companyService = companyService;
            _sessionService = sessionService;
        }

        private int UserId
        {
            get
            {
                var idClaim = User.FindFirst("userId") ?? User.FindFirst("UserId") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                return int.TryParse(idClaim?.Value, out int id) ? id : 1;
            }
        }
        private int CompanyId => _companyService.GetUserCurrentCompany(UserId) ?? 0;
        private int SessionId => _sessionService.GetUserCurrentSession(UserId) ?? 0;

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

        [HttpGet("fields")]
        public IActionResult GetAllFields(bool? isSystemField = null, string belongsTo = null)
        {
            var fields = _fieldService.GetAllFields(CompanyId, SessionId, isSystemField, belongsTo);
            return Ok(ApiResponse<List<FieldModel>>.SuccessResponse(fields));
        }

        [HttpGet("fields/{id}")]
        public IActionResult GetFieldByID(int id)
        {
            var field = _fieldService.GetFieldByID(id);
            return Ok(ApiResponse<FieldModel>.SuccessResponse(field));
        }

        [HttpPost("fields/upsert")]
        public IActionResult UpsertField([FromBody] FieldViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid model state"));

            model.CompanyID = CompanyId;
            model.SessionID = SessionId;
            var (success, message) = _fieldService.UpsertField(model, UserId);
            return success ? Ok(ApiResponse<bool>.SuccessResponse(true, message)) : BadRequest(ApiResponse<bool>.ErrorResponse(message));
        }

        [HttpDelete("fields/delete/{id}")]
        public IActionResult DeleteField(int id)
        {
            var (success, message) = _fieldService.DeleteField(id, UserId);
            return success ? Ok(ApiResponse<bool>.SuccessResponse(true, message)) : BadRequest(ApiResponse<bool>.ErrorResponse(message));
        }

        [HttpPost("fields/toggle-status")]
        public IActionResult ToggleFieldStatus([FromBody] dynamic data)
        {
            int id = (int)data.id;
            bool isActive = (bool)data.isActive;
            var (success, message) = _fieldService.ToggleFieldStatus(id, isActive, UserId);
            return success ? Ok(ApiResponse<bool>.SuccessResponse(true, message)) : BadRequest(ApiResponse<bool>.ErrorResponse(message));
        }

        [HttpGet("id-autogen/settings")]
        public IActionResult GetIDAutoGenSettings()
        {
            var settings = _fieldService.GetIDAutoGenSettings(CompanyId, SessionId);
            return Ok(ApiResponse<List<IDAutoGenSettings>>.SuccessResponse(settings));
        }

        [HttpPost("id-autogen/save")]
        public IActionResult SaveIDAutoGenSettings([FromBody] IDAutoGenRequest request)
        {
            request.CompanyID = CompanyId;
            request.SessionID = SessionId;
            var (success, message) = _fieldService.SaveIDAutoGenSettings(request, UserId);
            return success ? Ok(ApiResponse<bool>.SuccessResponse(true, message)) : BadRequest(ApiResponse<bool>.ErrorResponse(message));
        }
    }
}
