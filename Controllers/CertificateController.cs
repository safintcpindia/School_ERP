using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services.Clients;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    public class CertificateController : Controller
    {
        private readonly IStudentCertificateClientService _certificateClient;
        private readonly IStudentIDCardClientService _idCardClient;
        private readonly IStaffIDCardClientService _staffIdCardClient;

        public CertificateController(IStudentCertificateClientService certificateClient, 
            IStudentIDCardClientService idCardClient,
            IStaffIDCardClientService staffIdCardClient)
        {
            _certificateClient = certificateClient;
            _idCardClient = idCardClient;
            _staffIdCardClient = staffIdCardClient;
        }

        public async Task<IActionResult> StudentCertificate()
        {
            var model = new StudentCertificatePageViewModel();
            var resp = await _certificateClient.GetAll();
            if (resp.Success)
            {
                model.Certificates = resp.Data;
            }
            return View(model);
        }

        public IActionResult GenerateCertificate()
        {
            return View();
        }

        public async Task<IActionResult> StudentIdCard()
        {
            var model = new StudentIDCardPageViewModel();
            var resp = await _idCardClient.GetAll();
            if (resp.Success)
            {
                model.IDCards = resp.Data;
            }
            return View(model);
        }

        public IActionResult GenerateStudentIdCard()
        {
            return View();
        }

        public async Task<IActionResult> StaffIdCard()
        {
            var model = new StaffIDCardPageViewModel();
            var resp = await _staffIdCardClient.GetAll();
            if (resp.Success)
            {
                model.IDCards = resp.Data;
            }
            return View(model);
        }

        public IActionResult GenerateStaffIdCard()
        {
            return View();
        }
    }
}
