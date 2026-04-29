using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Services.Clients;
using SchoolERP.Net.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    public class HumanResourceController : Controller
    {
        private readonly IHumanResourceClientService _hrClient;
        private readonly IRoleClientService _roleClient;
        private readonly IUserTypeClientService _userTypeClient;
        private readonly ICompanyClientService _companyClient;

        public HumanResourceController(
            IHumanResourceClientService hrClient,
            IRoleClientService roleClient,
            IUserTypeClientService userTypeClient,
            ICompanyClientService companyClient)
        {
            _hrClient = hrClient;
            _roleClient = roleClient;
            _userTypeClient = userTypeClient;
            _companyClient = companyClient;
        }

        public async Task<IActionResult> Designation()
        {
            var res = await _hrClient.GetAllDesignationsAsync();
            var model = new HRDesignationPageViewModel
            {
                Items = res.Success ? res.Data : new List<HRDesignationViewModel>()
            };
            if (!res.Success) ViewBag.ErrorMessage = res.Message;
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

        public async Task<IActionResult> AddStaff(int? id)
        {
            var model = new HRStaffPageViewModel();

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

            if (id.HasValue && id.Value > 0)
            {
                var staffRes = await _hrClient.GetStaffByIDAsync(id.Value);
                if (staffRes.Success)
                {
                    model.EditStaff = staffRes.Data;
                }
            }
            else
            {
                var codeRes = await _hrClient.GetNewStaffCodeAsync();
                model.NewStaffCode = codeRes.Success ? codeRes.Data : "";
            }

            return View(model);
        }

        public async Task<IActionResult> Staffs()
        {
            var model = new HRStaffPageViewModel();
            
            var desigRes = await _hrClient.GetAllDesignationsAsync();
            model.Designations = desigRes.Success ? desigRes.Data : new List<HRDesignationViewModel>();

            var deptRes = await _hrClient.GetAllDepartmentsAsync();
            model.Departments = deptRes.Success ? deptRes.Data : new List<HRDepartmentViewModel>();

            var rolesRes = await _roleClient.GetAllRolesAsync();
            model.Roles = rolesRes.Success ? rolesRes.Data : new List<MstRoleViewModel>();

            return View(model);
        }
    }
}
