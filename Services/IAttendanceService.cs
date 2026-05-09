using SchoolERP.Net.Models;
using System;
using System.Collections.Generic;

namespace SchoolERP.Net.Services
{
    public interface IAttendanceService
    {
        List<StudentAttendanceViewModel> GetStudentAttendanceList(int classId, int sectionId, DateTime date, int companyId);
        (bool Success, string Message) SaveBulkAttendance(AttendanceUpsertRequest req, int companyId, int userId);
        StudentAttendanceHistoryViewModel GetStudentAttendanceHistory(int studentId, int year, int companyId);
    }
}
