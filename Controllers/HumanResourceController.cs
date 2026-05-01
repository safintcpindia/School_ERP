using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Services.Clients;
using SchoolERP.Net.Services;
using SchoolERP.Net.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    /// <summary>
    /// This controller manages the school's human resources, including staff information, job designations, departments, and leave policies.
    /// </summary>
    public class HumanResourceController : Controller
    {
        private readonly IHumanResourceClientService _hrClient;
        private readonly IRoleClientService _roleClient;
        private readonly IUserTypeClientService _userTypeClient;
        private readonly ICompanyClientService _companyClient;
        private readonly IHumanResourceService _hrService;
        private readonly ISettingsClientService _settingsClient;

        public HumanResourceController(
            IHumanResourceClientService hrClient,
            IRoleClientService roleClient,
            IUserTypeClientService userTypeClient,
            ICompanyClientService companyClient,
            IHumanResourceService hrService,
            ISettingsClientService settingsClient)
        {
            _hrClient = hrClient;
            _roleClient = roleClient;
            _userTypeClient = userTypeClient;
            _companyClient = companyClient;
            _hrService = hrService;
            _settingsClient = settingsClient;
        }

        /// <summary>
        /// Shows the 'Designation' page where you can manage different job titles (like Teacher, Admin, or Accountant).
        /// </summary>
        public async Task<IActionResult> Designation()
        {
            // Step 1: Ask the system for all the job titles currently set up.
            var res = await _hrClient.GetAllDesignationsAsync();
            
            // Step 2: Prepare the list to be shown on the screen.
            var model = new HRDesignationPageViewModel
            {
                Items = res.Success ? res.Data : new List<HRDesignationViewModel>()
            };
            if (!res.Success) ViewBag.ErrorMessage = res.Message;
            
            // Step 3: Open the 'Designation' page.
            return View(model);
        }

        public async Task<IActionResult> Department()
        {
            var res = await _hrClient.GetAllDepartmentsAsync();
            var model = new HRDepartmentPageViewModel
            {
                Items = res.Success ? res.Data : new List<HRDepartmentViewModel>()
            };
            if (!res.Success) ViewBag.ErrorMessage = res.Message;
            return View(model);
        }

        public async Task<IActionResult> LeaveType()
        {
            var res = await _hrClient.GetAllLeaveTypesAsync();
            var model = new HRLeaveTypePageViewModel
            {
                Items = res.Success ? res.Data : new List<HRLeaveTypeViewModel>()
            };
            if (!res.Success) ViewBag.ErrorMessage = res.Message;
            return View(model);
        }

        /// <summary>
        /// Shows the 'Add Staff' page where you can register a new employee or update an existing employee's details.
        /// </summary>
        public async Task<IActionResult> AddStaff(int? id)
        {
            // Step 1: Initialize a new blank page model.
            var model = new HRStaffPageViewModel();
 
            // Step 2: Fetch all the necessary dropdown lists (Designations, Departments, Roles, User Types, and School Branches).
            var desigRes = await _hrClient.GetAllDesignationsAsync();
            model.Designations = desigRes.Success ? desigRes.Data : new List<HRDesignationViewModel>();
 
            var deptRes = await _hrClient.GetAllDepartmentsAsync();
            model.Departments = deptRes.Success ? deptRes.Data : new List<HRDepartmentViewModel>();
 
            var rolesRes = await _roleClient.GetAllRolesAsync();
            model.Roles = rolesRes.Success ? rolesRes.Data : new List<MstRoleViewModel>();
 
            var typesRes = await _userTypeClient.GetAllAsync();
            model.UserTypes = typesRes.Success ? typesRes.Data : new List<MstUserTypeViewModel>();
 
            var compRes = await _companyClient.GetAllAsync();
            model.Companies = compRes.Success ? compRes.Data : new List<MstCompanyViewModel>();

            var fieldRes = await _settingsClient.GetAllFieldsAsync(isSystemField: true, belongsTo: "Staff");
            model.SystemFields = fieldRes.Success ? fieldRes.Data : new List<FieldModel>();
 
            // Step 3: If we are editing an existing person (ID is provided), fetch their details.

            if (id.HasValue && id.Value > 0)
            {
                var staffRes = await _hrClient.GetStaffByIDAsync(id.Value);
                if (staffRes.Success)
                {
                    model.EditStaff = staffRes.Data;
                }
            }
            // Step 4: If this is a new person, generate a fresh Staff ID code for them.
            else
            {
                var codeRes = await _hrClient.GetNewStaffCodeAsync();
                model.NewStaffCode = codeRes.Success ? codeRes.Data : "";
            }
 
            // Step 5: Open the 'Add Staff' page with all the gathered information.
            return View(model);
        }

        /// <summary>
        /// Shows the 'Staff Directory' page, displaying a list of all current school employees.
        /// </summary>
        public async Task<IActionResult> Staffs()
        {
            // Step 1: Initialize the directory page model.
            var model = new HRStaffPageViewModel();
            
            // Step 2: Fetch designations, departments, and roles to populate the filter options.
            var desigRes = await _hrClient.GetAllDesignationsAsync();
            model.Designations = desigRes.Success ? desigRes.Data : new List<HRDesignationViewModel>();

            var deptRes = await _hrClient.GetAllDepartmentsAsync();
            model.Departments = deptRes.Success ? deptRes.Data : new List<HRDepartmentViewModel>();

            var rolesRes = await _roleClient.GetAllRolesAsync();
            model.Roles = rolesRes.Success ? rolesRes.Data : new List<MstRoleViewModel>();

            // Step 3: Fetch the complete list of all staff members directly from the system.
            var staffRes = await _hrClient.GetAllStaffAsync();
            model.StaffList = staffRes.Success ? staffRes.Data : new List<HRStaffViewModel>();

            // Step 4: Open the 'Staff Directory' page.
            return View(model);
        }

        public async Task<IActionResult> StaffDetails(int id)
        {
            var res = await _hrClient.GetStaffByIDAsync(id);
            if (!res.Success || res.Data == null)
            {
                return RedirectToAction("Staffs");
            }

            var fieldRes = await _settingsClient.GetAllFieldsAsync(isSystemField: true, belongsTo: "Staff");
            res.Data.SystemFields = fieldRes.Success ? fieldRes.Data : new List<FieldModel>();

            return View(res.Data);
        }

        public async Task<IActionResult> StaffAttendance()
        {
            var res = await _roleClient.GetAllRolesAsync();
            var model = new HRStaffPageViewModel
            {
                Roles = res.Data ?? new List<MstRoleViewModel>()
            };
            return View(model);
        }

        public async Task<IActionResult> ApplyLeave()
        {
            var model = new HRApplyLeavePageViewModel();
            
            var leavesRes = await _hrClient.GetAllApplyLeaveAsync();
            model.Leaves = leavesRes.Success ? leavesRes.Data : new List<HRApplyLeaveViewModel>();

            var staffRes = await _hrClient.GetAllStaffAsync();
            model.StaffList = staffRes.Success ? staffRes.Data : new List<HRStaffViewModel>();

            var leaveTypesRes = await _hrClient.GetAllLeaveTypesAsync();
            model.LeaveTypes = leaveTypesRes.Success ? leaveTypesRes.Data : new List<HRLeaveTypeViewModel>();

            return View(model);
        }


        public IActionResult DownloadStaffDocument(int id, string type)
        {
            var (bytes, fileName, contentType) = _hrService.GetStaffDocument(id, type);
            if (bytes == null || bytes.Length == 0) return NotFound();
            return File(bytes, contentType, fileName);
        }

        
    }
}
