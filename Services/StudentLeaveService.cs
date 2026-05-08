using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;

namespace SchoolERP.Net.Services
{
    public class StudentLeaveService : IStudentLeaveService
    {
        private readonly SqlHelper _sqlHelper;

        public StudentLeaveService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public List<StudentLeaveViewModel> GetLeaveApplications(int? classId, int? sectionId, int? status, int companyId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Action", "LIST"),
                new SqlParameter("@CompanyID", companyId)
            };

            if (classId.HasValue && classId > 0) parameters.Add(new SqlParameter("@ClassID", classId.Value));
            if (sectionId.HasValue && sectionId > 0) parameters.Add(new SqlParameter("@SectionID", sectionId.Value));
            if (status.HasValue && status >= 0) parameters.Add(new SqlParameter("@Status", status.Value));

            var dt = _sqlHelper.ExecuteQuery("sp_Student_LeaveApp_CRUD", parameters.ToArray());
            var list = new List<StudentLeaveViewModel>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new StudentLeaveViewModel
                {
                    LeaveAppID = Convert.ToInt32(row["LeaveAppID"]),
                    StudentID = Convert.ToInt32(row["StudentID"]),
                    ClassID = Convert.ToInt32(row["ClassID"]),
                    SectionID = Convert.ToInt32(row["SectionID"]),
                    AdmissionNo = row["AdmissionNo"].ToString(),
                    RollNo = row["RollNo"].ToString(),
                    StudentName = row["StudentName"].ToString(),
                    ClassName = row["ClassName"].ToString(),
                    SectionName = row["SectionName"].ToString(),
                    FromDate = Convert.ToDateTime(row["FromDate"]),
                    ToDate = Convert.ToDateTime(row["ToDate"]),
                    Reason = row["Reason"].ToString(),
                    Status = Convert.ToInt32(row["Status"]),
                    ApplyDate = Convert.ToDateTime(row["ApplyDate"]),
                    AttachmentName = row["AttachmentName"].ToString(),
                    AttachmentType = row["AttachmentType"].ToString(),
                    HasAttachment = Convert.ToInt32(row["HasAttachment"])
                });
            }

            return list;
        }

        public (bool Success, string Message) UpdateLeaveStatus(int leaveAppId, int status, int companyId, int userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Action", "UPDATE_STATUS"),
                new SqlParameter("@LeaveAppID", leaveAppId),
                new SqlParameter("@Status", status),
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@UserID", userId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_Student_LeaveApp_CRUD", parameters.ToArray());
            if (dt.Rows.Count > 0)
            {
                return (true, dt.Rows[0]["Message"].ToString());
            }

            return (false, "Failed to update status");
        }

        public (bool Success, string Message) UpsertLeaveApplication(StudentLeaveUpsertRequest req, int companyId, int userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Action", "UPSERT"),
                new SqlParameter("@LeaveAppID", req.LeaveAppID),
                new SqlParameter("@StudentID", req.StudentID),
                new SqlParameter("@FromDate", req.FromDate),
                new SqlParameter("@ToDate", req.ToDate),
                new SqlParameter("@ApplyDate", req.ApplyDate),
                new SqlParameter("@Reason", (object?)req.Reason ?? DBNull.Value),
                new SqlParameter("@Attachment", (object?)req.Attachment ?? DBNull.Value) { SqlDbType = SqlDbType.VarBinary },
                new SqlParameter("@AttachmentType", (object?)req.AttachmentType ?? DBNull.Value),
                new SqlParameter("@AttachmentName", (object?)req.AttachmentName ?? DBNull.Value),
                new SqlParameter("@Status", req.Status),
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@UserID", userId)
            };

            var dt = _sqlHelper.ExecuteQuery("sp_Student_LeaveApp_CRUD", parameters.ToArray());
            if (dt.Rows.Count > 0)
            {
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString());
            }

            return (false, "Failed to save leave application");
        }

        public (byte[]? Bytes, string? FileName, string? ContentType) GetLeaveAttachment(int leaveAppId, int companyId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Action", "GET_ATTACHMENT"),
                new SqlParameter("@LeaveAppID", leaveAppId),
                new SqlParameter("@CompanyID", companyId)
            };

            // I'll need to update the SP to handle GET_ATTACHMENT
            var dt = _sqlHelper.ExecuteQuery("sp_Student_LeaveApp_CRUD", parameters.ToArray());
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                if (row["Attachment"] != DBNull.Value)
                {
                    return (
                        (byte[])row["Attachment"],
                        row["AttachmentName"].ToString(),
                        row["AttachmentType"].ToString()
                    );
                }
            }

            return (null, null, null);
        }
    }
}
