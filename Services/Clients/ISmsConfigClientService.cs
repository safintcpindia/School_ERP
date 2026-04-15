using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface ISmsConfigClientService
    {
        Task<ApiResponse<MstSmsConfigViewModel>> GetSmsConfigAsync();
        Task<ApiResponse<dynamic>> UpsertSmsConfigAsync(MstSmsConfigUpsertRequest request);
    }
}
