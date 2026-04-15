using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Services.Clients
{
    public interface IPaymentMethodClientService
    {
        Task<ApiResponse<List<MstPaymentMethodViewModel>>> GetAllPaymentMethodsAsync();
        Task<ApiResponse<MstPaymentMethodViewModel>> GetPaymentMethodByIdAsync(int paymentId);
        Task<ApiResponse<object>> UpsertPaymentMethodAsync(MstPaymentMethodUpsertRequest request);
        Task<ApiResponse<object>> DeletePaymentMethodAsync(int paymentId);
        Task<ApiResponse<object>> ToggleStatusAsync(int id, bool isActive);
    }
}
