using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// This class provides business logic and data access services for EmailConfigClientService.
    /// </summary>
    public class EmailConfigClientService : BaseApiClient, IEmailConfigClientService
    {
        public EmailConfigClientService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ApiResponse<MstEmailConfigViewModel>> GetEmailConfigAsync()
        {
            return await GetAsync<MstEmailConfigViewModel>("api/EmailConfigApi/Get");
        }

        public async Task<ApiResponse<dynamic>> UpsertEmailConfigAsync(MstEmailConfigUpsertRequest request)
        {
            return await PostAsync<dynamic>("api/EmailConfigApi/Upsert", request);
        }
    }
}
