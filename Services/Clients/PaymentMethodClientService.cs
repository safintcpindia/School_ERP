using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This class provides business logic and data access services for PaymentMethodClientService.
    /// </summary>
    public class PaymentMethodClientService : BaseApiClient, IPaymentMethodClientService
    {
        public PaymentMethodClientService(HttpClient httpClient) : base(httpClient) { }

        public async Task<ApiResponse<List<MstPaymentMethodViewModel>>> GetAllPaymentMethodsAsync()
        {
            return await GetAsync<List<MstPaymentMethodViewModel>>("api/PaymentMethodApi/GetAll");
        }

        public async Task<ApiResponse<MstPaymentMethodViewModel>> GetPaymentMethodByIdAsync(int paymentId)
        {
            return await GetAsync<MstPaymentMethodViewModel>($"api/PaymentMethodApi/GetById/{paymentId}");
        }

        public async Task<ApiResponse<object>> UpsertPaymentMethodAsync(MstPaymentMethodUpsertRequest request)
        {
            return await PostAsync<object>("api/PaymentMethodApi/Upsert", request);
        }

        public async Task<ApiResponse<object>> DeletePaymentMethodAsync(int paymentId)
        {
            return await PostAsync<object>($"api/PaymentMethodApi/Delete/{paymentId}", null);
        }

        public async Task<ApiResponse<object>> ToggleStatusAsync(int id, bool isActive)
        {
            return await PostAsync<object>($"api/PaymentMethodApi/ToggleStatus?id={id}&isActive={isActive}", null);
        }
    }
}
