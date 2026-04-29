using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface IFrontOfficeClientService
    {
        // Purpose
        Task<ApiResponse<List<MstFOPurposeViewModel>>> GetAllPurposesAsync(bool includeDeleted = false);
        Task<ApiResponse<MstFOPurposeViewModel>> GetPurposeByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertPurposeAsync(MstFOPurposeUpsertRequest req);
        Task<ApiResponse<dynamic>> DeletePurposeAsync(int id);
        Task<ApiResponse<dynamic>> TogglePurposeStatusAsync(int id, bool isActive);

        // Complaint Type
        Task<ApiResponse<List<MstFOComplaintTypeViewModel>>> GetAllComplaintTypesAsync(bool includeDeleted = false);
        Task<ApiResponse<MstFOComplaintTypeViewModel>> GetComplaintTypeByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertComplaintTypeAsync(MstFOComplaintTypeUpsertRequest req);
        Task<ApiResponse<dynamic>> DeleteComplaintTypeAsync(int id);
        Task<ApiResponse<dynamic>> ToggleComplaintTypeStatusAsync(int id, bool isActive);

        // Source
        Task<ApiResponse<List<MstFOSourceViewModel>>> GetAllSourcesAsync(bool includeDeleted = false);
        Task<ApiResponse<MstFOSourceViewModel>> GetSourceByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertSourceAsync(MstFOSourceUpsertRequest req);
        Task<ApiResponse<dynamic>> DeleteSourceAsync(int id);
        Task<ApiResponse<dynamic>> ToggleSourceStatusAsync(int id, bool isActive);

        // Reference
        Task<ApiResponse<List<MstFOReferenceViewModel>>> GetAllReferencesAsync(bool includeDeleted = false);
        Task<ApiResponse<MstFOReferenceViewModel>> GetReferenceByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertReferenceAsync(MstFOReferenceUpsertRequest req);
        Task<ApiResponse<dynamic>> DeleteReferenceAsync(int id);
        Task<ApiResponse<dynamic>> ToggleReferenceStatusAsync(int id, bool isActive);

        // Complaint
        Task<ApiResponse<List<FOComplaintViewModel>>> GetAllComplaintsAsync(bool includeDeleted = false);
        Task<ApiResponse<FOComplaintViewModel>> GetComplaintByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertComplaintAsync(FOComplaintUpsertRequest req);
        Task<ApiResponse<dynamic>> DeleteComplaintAsync(int id);
        Task<ApiResponse<dynamic>> ToggleComplaintStatusAsync(int id, bool isActive);

        // Postal Receive
        Task<ApiResponse<List<FOPostalReceiveViewModel>>> GetAllPostalReceivesAsync(bool includeDeleted = false);
        Task<ApiResponse<FOPostalReceiveViewModel>> GetPostalReceiveByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertPostalReceiveAsync(FOPostalReceiveUpsertRequest req);
        Task<ApiResponse<dynamic>> DeletePostalReceiveAsync(int id);
        Task<ApiResponse<dynamic>> TogglePostalReceiveStatusAsync(int id, bool isActive);

        // Postal Dispatch
        Task<ApiResponse<List<FOPostalDispatchViewModel>>> GetAllPostalDispatchesAsync(bool includeDeleted = false);
        Task<ApiResponse<FOPostalDispatchViewModel>> GetPostalDispatchByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertPostalDispatchAsync(FOPostalDispatchUpsertRequest req);
        Task<ApiResponse<dynamic>> DeletePostalDispatchAsync(int id);
        Task<ApiResponse<dynamic>> TogglePostalDispatchStatusAsync(int id, bool isActive);

        // Phone Call Log
        Task<ApiResponse<List<FOPhoneCallLogViewModel>>> GetAllPhoneCallLogsAsync(bool includeDeleted = false);
        Task<ApiResponse<FOPhoneCallLogViewModel>> GetPhoneCallLogByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertPhoneCallLogAsync(FOPhoneCallLogUpsertRequest req);
        Task<ApiResponse<dynamic>> DeletePhoneCallLogAsync(int id);
        Task<ApiResponse<dynamic>> TogglePhoneCallLogStatusAsync(int id, bool isActive);
    }
}
