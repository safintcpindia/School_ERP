using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;
using System.Threading.Tasks;

namespace SchoolERP.Net.Services.Clients
{
    public interface IEmailConfigClientService
    {
        Task<ApiResponse<MstEmailConfigViewModel>> GetEmailConfigAsync();
        Task<ApiResponse<dynamic>> UpsertEmailConfigAsync(MstEmailConfigUpsertRequest request);
    }
}
