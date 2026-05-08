using System.Collections.Generic;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public interface IStudentCertificateService
    {
        List<StudentCertificateViewModel> GetAll(int companyId, int sessionId);
        StudentCertificateViewModel GetByID(int id);
        (int Result, string Message) Upsert(StudentCertificateUpsertRequest request, int userId, int companyId, int sessionId);
        (int Result, string Message) Delete(int id, int userId);
        (int Result, string Message) ToggleStatus(int id, bool isActive, int userId);
        string GenerateCertificate(int studentId, int certificateId, int companyId, int sessionId);
    }
}
