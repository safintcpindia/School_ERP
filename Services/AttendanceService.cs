using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;

namespace SchoolERP.Net.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly SqlHelper _db;
        public AttendanceService(SqlHelper db)
        {
            _db = db;
        }

        public List<StudentAttendanceViewModel> GetStudentAttendanceList(int classId, int sectionId, DateTime date, int companyId)
        {
            var list = new List<StudentAttendanceViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@Action", "LIST"),
                    new SqlParameter("@ClassID", classId),
                    new SqlParameter("@SectionID", sectionId),
                    new SqlParameter("@AttendanceDate", date),
                    new SqlParameter("@CompanyID", companyId)
                };
                var dt = _db.ExecuteQuery("sp_Attendance_Student_CRUD", p);
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new StudentAttendanceViewModel
                    {
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        AdmissionNo = row["AdmissionNo"].ToString(),
                        RollNo = row["RollNo"].ToString(),
                        StudentName = row["StudentName"].ToString(),
                        AttendanceStatus = Convert.ToInt32(row["AttendanceStatus"]),
                        Note = row["Note"].ToString(),
                        AttendanceID = row["AttendanceID"] != DBNull.Value ? Convert.ToInt32(row["AttendanceID"]) : null
                    });
                }
            }
            catch { }
            return list;
        }

        public (bool Success, string Message) SaveBulkAttendance(AttendanceUpsertRequest req, int companyId, int userId)
        {
            try
            {
                var json = JsonSerializer.Serialize(req.AttendanceData);
                var p = new[] {
                    new SqlParameter("@Action", "SAVE_BULK"),
                    new SqlParameter("@ClassID", req.ClassID),
                    new SqlParameter("@SectionID", req.SectionID),
                    new SqlParameter("@AttendanceDate", req.AttendanceDate),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@JsonData", json)
                };
                var dt = _db.ExecuteQuery("sp_Attendance_Student_CRUD", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }
    }
}
