using System;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This class provides business logic and data access services for SmsConfigClientService.
    /// </summary>
    public class SmsConfigClientService : BaseApiClient, ISmsConfigClientService
    {
        public SmsConfigClientService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ApiResponse<MstSmsConfigViewModel>> GetSmsConfigAsync()
        {
            return await GetAsync<MstSmsConfigViewModel>("api/SmsConfigApi/Get");
        }

        public async Task<ApiResponse<dynamic>> UpsertSmsConfigAsync(MstSmsConfigUpsertRequest request)
        {
            return await PostAsync<dynamic>("api/SmsConfigApi/Upsert", request);
        }
    }
}
