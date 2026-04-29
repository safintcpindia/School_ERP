using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SchoolERP.Net.Models;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    public class HumanResourceClientService : BaseApiClient, IHumanResourceClientService
    {
        public HumanResourceClientService(HttpClient httpClient) : base(httpClient) { }

        public Task<ApiResponse<List<HRDesignationViewModel>>> GetAllDesignationsAsync()
            => GetAsync<List<HRDesignationViewModel>>("api/HumanResourceApi/GetAllDesignations");

        public Task<ApiResponse<HRDesignationViewModel>> GetDesignationByIDAsync(int id)
            => GetAsync<HRDesignationViewModel>($"api/HumanResourceApi/GetDesignationByID/{id}");

        public Task<ApiResponse<dynamic>> UpsertDesignationAsync(HRDesignationUpsertRequest req)
            => PostAsync<dynamic>("api/HumanResourceApi/UpsertDesignation", req);

        public Task<ApiResponse<dynamic>> DeleteDesignationAsync(int id)
            => PostAsync<dynamic>($"api/HumanResourceApi/DeleteDesignation/{id}", null!);

        public Task<ApiResponse<dynamic>> ToggleDesignationStatusAsync(int id, bool isActive)
            => PostAsync<dynamic>($"api/HumanResourceApi/ToggleDesignationStatus?id={id}&isActive={isActive}", null!);

        public Task<ApiResponse<List<HRDepartmentViewModel>>> GetAllDepartmentsAsync()
            => GetAsync<List<HRDepartmentViewModel>>("api/HumanResourceApi/GetAllDepartments");

        public Task<ApiResponse<HRDepartmentViewModel>> GetDepartmentByIDAsync(int id)
            => GetAsync<HRDepartmentViewModel>($"api/HumanResourceApi/GetDepartmentByID/{id}");

        public Task<ApiResponse<dynamic>> UpsertDepartmentAsync(HRDepartmentUpsertRequest req)
            => PostAsync<dynamic>("api/HumanResourceApi/UpsertDepartment", req);

        public Task<ApiResponse<dynamic>> DeleteDepartmentAsync(int id)
            => PostAsync<dynamic>($"api/HumanResourceApi/DeleteDepartment/{id}", null!);

        public Task<ApiResponse<dynamic>> ToggleDepartmentStatusAsync(int id, bool isActive)
            => PostAsync<dynamic>($"api/HumanResourceApi/ToggleDepartmentStatus?id={id}&isActive={isActive}", null!);

        public Task<ApiResponse<List<HRLeaveTypeViewModel>>> GetAllLeaveTypesAsync()
            => GetAsync<List<HRLeaveTypeViewModel>>("api/HumanResourceApi/GetAllLeaveTypes");

        public Task<ApiResponse<HRLeaveTypeViewModel>> GetLeaveTypeByIDAsync(int id)
            => GetAsync<HRLeaveTypeViewModel>($"api/HumanResourceApi/GetLeaveTypeByID/{id}");

        public Task<ApiResponse<dynamic>> UpsertLeaveTypeAsync(HRLeaveTypeUpsertRequest req)
            => PostAsync<dynamic>("api/HumanResourceApi/UpsertLeaveType", req);

        public Task<ApiResponse<dynamic>> DeleteLeaveTypeAsync(int id)
            => PostAsync<dynamic>($"api/HumanResourceApi/DeleteLeaveType/{id}", null!);

        public Task<ApiResponse<dynamic>> ToggleLeaveTypeStatusAsync(int id, bool isActive)
            => PostAsync<dynamic>($"api/HumanResourceApi/ToggleLeaveTypeStatus?id={id}&isActive={isActive}", null!);

        public Task<ApiResponse<List<HRStaffViewModel>>> GetAllStaffAsync()
            => GetAsync<List<HRStaffViewModel>>("api/HumanResourceApi/GetAllStaff");

        public Task<ApiResponse<HRStaffViewModel>> GetStaffByIDAsync(int id)
            => GetAsync<HRStaffViewModel>($"api/HumanResourceApi/GetStaffByID/{id}");

        public Task<ApiResponse<dynamic>> UpsertStaffAsync(HRStaffUpsertRequest req)
            => PostAsync<dynamic>("api/HumanResourceApi/UpsertStaff", req);

        public Task<ApiResponse<dynamic>> DeleteStaffAsync(int id)
            => PostAsync<dynamic>($"api/HumanResourceApi/DeleteStaff/{id}", null!);

        public Task<ApiResponse<string>> GetNewStaffCodeAsync()
            => GetAsync<string>("api/HumanResourceApi/GetNewStaffCode");
    }
}
