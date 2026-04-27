using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Models.Common;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for managing payment methods (like cash, check, or online) through the API.
    /// </summary>
    public class PaymentMethodApiController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentService;

        public PaymentMethodApiController(IPaymentMethodService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Gets the full list of all payment methods configured in the system.
        /// </summary>
        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false)
        {
            var data = _paymentService.GetAllPaymentMethods(includeDeleted);
            return Ok(new ApiResponse<List<MstPaymentMethodViewModel>> { Success = true, Data = data });
        }

        /// <summary>
        /// Gets the details of one specific payment method using its unique ID number.
        /// </summary>
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            var data = _paymentService.GetPaymentMethodById(id);
            if (data == null) return Ok(new ApiResponse<MstPaymentMethodViewModel> { Success = false, Message = "Billing Integration Not Found" });
            return Ok(new ApiResponse<MstPaymentMethodViewModel> { Success = true, Data = data });
        }

        /// <summary>
        /// Saves a new payment method or updates an existing one with the details you provided.
        /// </summary>
        [HttpPost("Upsert")]
        public IActionResult Upsert([FromBody] MstPaymentMethodUpsertRequest request)
        {
            int userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : 1;
            var (success, message) = _paymentService.UpsertPaymentMethod(request, userId);
            return Ok(new ApiResponse<object> { Success = success, Message = message });
        }

        /// <summary>
        /// Permanently removes a payment method from the system's records.
        /// </summary>
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            int userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int idVal) ? idVal : 1;
            var (success, message) = _paymentService.DeletePaymentMethod(id, userId);
            return Ok(new ApiResponse<object> { Success = success, Message = message });
        }

        /// <summary>
        /// Turns a payment method on or off, determining if it can be used for new payments.
        /// </summary>
        [HttpPost("ToggleStatus")]
        public IActionResult ToggleStatus(int id, bool isActive)
        {
            int userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int idVal) ? idVal : 1;
            var (success, message) = _paymentService.TogglePaymentMethodStatus(id, isActive, userId);
            return Ok(new ApiResponse<object> { Success = success, Message = message });
        }
    }
}
