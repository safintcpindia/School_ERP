using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IFrontOfficeService
    {
        // Purpose
        List<MstFOPurposeViewModel> GetAllPurposes(int companyId, int sessionId, bool includeDeleted = false);
        MstFOPurposeViewModel? GetPurposeByID(int id);
        (bool success, string message) UpsertPurpose(MstFOPurposeUpsertRequest req, int companyId, int sessionId, int userId);
        (bool success, string message) DeletePurpose(int id, int userId);
        (bool success, string message) TogglePurposeStatus(int id, bool isActive, int userId);

        // Complaint Type
        List<MstFOComplaintTypeViewModel> GetAllComplaintTypes(int companyId, int sessionId, bool includeDeleted = false);
        MstFOComplaintTypeViewModel? GetComplaintTypeByID(int id);
        (bool success, string message) UpsertComplaintType(MstFOComplaintTypeUpsertRequest req, int companyId, int sessionId, int userId);
        (bool success, string message) DeleteComplaintType(int id, int userId);
        (bool success, string message) ToggleComplaintTypeStatus(int id, bool isActive, int userId);

        // Source
        List<MstFOSourceViewModel> GetAllSources(int companyId, int sessionId, bool includeDeleted = false);
        MstFOSourceViewModel? GetSourceByID(int id);
        (bool success, string message) UpsertSource(MstFOSourceUpsertRequest req, int companyId, int sessionId, int userId);
        (bool success, string message) DeleteSource(int id, int userId);
        (bool success, string message) ToggleSourceStatus(int id, bool isActive, int userId);

        // Reference
        List<MstFOReferenceViewModel> GetAllReferences(int companyId, int sessionId, bool includeDeleted = false);
        MstFOReferenceViewModel? GetReferenceByID(int id);
        (bool success, string message) UpsertReference(MstFOReferenceUpsertRequest req, int companyId, int sessionId, int userId);
        (bool success, string message) DeleteReference(int id, int userId);
        (bool success, string message) ToggleReferenceStatus(int id, bool isActive, int userId);

        // Complaint
        List<FOComplaintViewModel> GetAllComplaints(int companyId, int sessionId, bool includeDeleted = false);
        FOComplaintViewModel? GetComplaintByID(int id);
        (bool success, string message) UpsertComplaint(FOComplaintUpsertRequest req, int companyId, int sessionId, int userId);
        (bool success, string message) DeleteComplaint(int id, int userId);
        (bool success, string message) ToggleComplaintStatus(int id, bool isActive, int userId);

        // Postal Receive
        List<FOPostalReceiveViewModel> GetAllPostalReceives(int companyId, int sessionId, bool includeDeleted = false);
        FOPostalReceiveViewModel? GetPostalReceiveByID(int id);
        (bool success, string message) UpsertPostalReceive(FOPostalReceiveUpsertRequest req, int companyId, int sessionId, int userId);
        (bool success, string message) DeletePostalReceive(int id, int userId);
        (bool success, string message) TogglePostalReceiveStatus(int id, bool isActive, int userId);

        // Postal Dispatch
        List<FOPostalDispatchViewModel> GetAllPostalDispatches(int companyId, int sessionId, bool includeDeleted = false);
        FOPostalDispatchViewModel? GetPostalDispatchByID(int id);
        (bool success, string message) UpsertPostalDispatch(FOPostalDispatchUpsertRequest req, int companyId, int sessionId, int userId);
        (bool success, string message) DeletePostalDispatch(int id, int userId);
        (bool success, string message) TogglePostalDispatchStatus(int id, bool isActive, int userId);

        // Phone Call Log
        List<FOPhoneCallLogViewModel> GetAllPhoneCallLogs(int companyId, int sessionId, bool includeDeleted = false);
        FOPhoneCallLogViewModel? GetPhoneCallLogByID(int id);
        (bool success, string message) UpsertPhoneCallLog(FOPhoneCallLogUpsertRequest req, int companyId, int sessionId, int userId);
        (bool success, string message) DeletePhoneCallLog(int id, int userId);
        (bool success, string message) TogglePhoneCallLogStatus(int id, bool isActive, int userId);
    }
}
