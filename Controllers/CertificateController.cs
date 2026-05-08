using Microsoft.AspNetCore.Mvc;
using SchoolERP.Net.Models;
using SchoolERP.Net.Services.Clients;
using System.Threading.Tasks;

namespace SchoolERP.Net.Controllers
{
    public class CertificateController : Controller
    {
        private readonly IStudentCertificateClientService _certificateClient;

        public CertificateController(IStudentCertificateClientService certificateClient)
        {
            _certificateClient = certificateClient;
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
    }
}
