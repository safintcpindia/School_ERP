using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public interface IHumanResourceClientService
    {
        Task<ApiResponse<List<HRDesignationViewModel>>> GetAllDesignationsAsync();
        Task<ApiResponse<HRDesignationViewModel>> GetDesignationByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertDesignationAsync(HRDesignationUpsertRequest req);
        Task<ApiResponse<dynamic>> DeleteDesignationAsync(int id);
        Task<ApiResponse<dynamic>> ToggleDesignationStatusAsync(int id, bool isActive);

        Task<ApiResponse<List<HRDepartmentViewModel>>> GetAllDepartmentsAsync();
        Task<ApiResponse<HRDepartmentViewModel>> GetDepartmentByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertDepartmentAsync(HRDepartmentUpsertRequest req);
        Task<ApiResponse<dynamic>> DeleteDepartmentAsync(int id);
        Task<ApiResponse<dynamic>> ToggleDepartmentStatusAsync(int id, bool isActive);

        Task<ApiResponse<List<HRLeaveTypeViewModel>>> GetAllLeaveTypesAsync();
        Task<ApiResponse<HRLeaveTypeViewModel>> GetLeaveTypeByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertLeaveTypeAsync(HRLeaveTypeUpsertRequest req);
        Task<ApiResponse<dynamic>> DeleteLeaveTypeAsync(int id);
        Task<ApiResponse<dynamic>> ToggleLeaveTypeStatusAsync(int id, bool isActive);

        Task<ApiResponse<List<HRStaffViewModel>>> GetAllStaffAsync();
        Task<ApiResponse<HRStaffViewModel>> GetStaffByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertStaffAsync(HRStaffUpsertRequest req);
        Task<ApiResponse<dynamic>> DeleteStaffAsync(int id);
        Task<ApiResponse<string>> GetNewStaffCodeAsync();
        
        Task<ApiResponse<List<HRStaffAttendanceViewModel>>> GetStaffAttendanceAsync(DateTime date, int? roleId);
        Task<ApiResponse<dynamic>> SaveStaffAttendanceAsync(List<HRStaffAttendanceUpsertRequest> reqs);

        // --- Apply Leave ---
        Task<ApiResponse<List<HRApplyLeaveViewModel>>> GetAllApplyLeaveAsync();
        Task<ApiResponse<HRApplyLeaveViewModel>> GetApplyLeaveByIDAsync(int id);
        Task<ApiResponse<dynamic>> UpsertApplyLeaveAsync(HRApplyLeaveUpsertRequest req);
        Task<ApiResponse<dynamic>> DeleteApplyLeaveAsync(int id);
        
        // --- Payroll ---
        Task<ApiResponse<List<HRPayrollViewModel>>> GetAllPayrollAsync(int month, int year, int? roleId);
        Task<ApiResponse<dynamic>> GeneratePayrollAsync(HRPayrollGenerateRequest req);
    }
}
