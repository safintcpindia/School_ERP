using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services;
using SchoolERP.Net.Models.Common;
using System.Security.Claims;

namespace SchoolERP.Net.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    /// <summary>
    /// This class handles HTTP routing and API requests for PaymentMethodApiController.
    /// </summary>
    public class PaymentMethodApiController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentService;

        public PaymentMethodApiController(IPaymentMethodService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Gets all configured billing interfaces.
        /// </summary>
        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false)
        {
            var data = _paymentService.GetAllPaymentMethods(includeDeleted);
            return Ok(new ApiResponse<List<MstPaymentMethodViewModel>> { Success = true, Data = data });
        }

        /// <summary>
        /// Queries a single transaction provider for gateway mutation.
        /// </summary>
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            var data = _paymentService.GetPaymentMethodById(id);
            if (data == null) return Ok(new ApiResponse<MstPaymentMethodViewModel> { Success = false, Message = "Billing Integration Not Found" });
            return Ok(new ApiResponse<MstPaymentMethodViewModel> { Success = true, Data = data });
        }

        /// <summary>
        /// Modifies or introduces a new bank/financial gateway option natively.
        /// </summary>
        [HttpPost("Upsert")]
        public IActionResult Upsert([FromBody] MstPaymentMethodUpsertRequest request)
        {
            int userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : 1;
            var (success, message) = _paymentService.UpsertPaymentMethod(request, userId);
            return Ok(new ApiResponse<object> { Success = success, Message = message });
        }

        /// <summary>
        /// Logically drops a transaction gateway from active invoice selections.
        /// </summary>
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            int userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int idVal) ? idVal : 1;
            var (success, message) = _paymentService.DeletePaymentMethod(id, userId);
            return Ok(new ApiResponse<object> { Success = success, Message = message });
        }

        /// <summary>
        /// Quickly hides a payment gateway globally without stripping historical transaction data.
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
