using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Helpers;
using SchoolERP.Net.Models;
using System.Data;

namespace SchoolERP.Net.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly SqlHelper _db;

        public LibraryService(SqlHelper db)
        {
            _db = db;
        }

        public List<BookViewModel> GetBookList(int companyId, string? searchTerm)
        {
            var p = new[] {
                new SqlParameter("@Action", "LIST"),
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@SearchTerm", (object?)searchTerm ?? DBNull.Value)
            };
            var dt = _db.ExecuteQuery("sp_Library_Book_CRUD", p);
            var list = new List<BookViewModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new BookViewModel
                {
                    BookID = Convert.ToInt32(row["BookID"]),
                    BookTitle = row["BookTitle"].ToString(),
                    BookNo = row["BookNo"].ToString(),
                    ISBNNo = row["ISBNNo"].ToString(),
                    Publisher = row["Publisher"].ToString(),
                    Author = row["Author"].ToString(),
                    Subject = row["Subject"].ToString(),
                    RackNo = row["RackNo"].ToString(),
                    TotalQty = Convert.ToInt32(row["TotalQty"]),
                    AvailableQty = Convert.ToInt32(row["AvailableQty"]),
                    BookPrice = Convert.ToDecimal(row["BookPrice"]),
                    PostDate = row["PostDate"] != DBNull.Value ? Convert.ToDateTime(row["PostDate"]) : null,
                    Description = row["Description"].ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"])
                });
            }
            return list;
        }

        public BookViewModel GetBookById(int id)
        {
            var p = new[] {
                new SqlParameter("@Action", "GETBYID"),
                new SqlParameter("@BookID", id)
            };
            var dt = _db.ExecuteQuery("sp_Library_Book_CRUD", p);
            if (dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];
            return new BookViewModel
            {
                BookID = Convert.ToInt32(row["BookID"]),
                BookTitle = row["BookTitle"].ToString(),
                BookNo = row["BookNo"].ToString(),
                ISBNNo = row["ISBNNo"].ToString(),
                Publisher = row["Publisher"].ToString(),
                Author = row["Author"].ToString(),
                Subject = row["Subject"].ToString(),
                RackNo = row["RackNo"].ToString(),
                TotalQty = Convert.ToInt32(row["TotalQty"]),
                BookPrice = Convert.ToDecimal(row["BookPrice"]),
                PostDate = row["PostDate"] != DBNull.Value ? Convert.ToDateTime(row["PostDate"]) : null,
                Description = row["Description"].ToString()
            };
        }

        public (bool Success, string Message) UpsertBook(BookUpsertRequest req, int companyId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@Action", "SAVE"),
                    new SqlParameter("@BookID", (object?)req.BookID ?? DBNull.Value),
                    new SqlParameter("@BookTitle", req.BookTitle),
                    new SqlParameter("@BookNo", (object?)req.BookNo ?? DBNull.Value),
                    new SqlParameter("@ISBNNo", (object?)req.ISBNNo ?? DBNull.Value),
                    new SqlParameter("@Publisher", (object?)req.Publisher ?? DBNull.Value),
                    new SqlParameter("@Author", (object?)req.Author ?? DBNull.Value),
                    new SqlParameter("@Subject", (object?)req.Subject ?? DBNull.Value),
                    new SqlParameter("@RackNo", (object?)req.RackNo ?? DBNull.Value),
                    new SqlParameter("@TotalQty", req.TotalQty),
                    new SqlParameter("@BookPrice", req.BookPrice),
                    new SqlParameter("@PostDate", (object?)req.PostDate ?? DBNull.Value),
                    new SqlParameter("@Description", (object?)req.Description ?? DBNull.Value),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Library_Book_CRUD", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeleteBook(int id, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@Action", "DELETE"),
                    new SqlParameter("@BookID", id),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Library_Book_CRUD", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) ToggleBookStatus(int id, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@Action", "TOGGLESTATUS"),
                    new SqlParameter("@BookID", id),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Library_Book_CRUD", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        // Membership
        public List<LibraryMemberViewModel> GetMemberList(string memberType, int companyId, int? classId, int? sectionId, int? departmentId, string? search)
        {
            var p = new[] {
                new SqlParameter("@Action", "LIST"),
                new SqlParameter("@MemberType", memberType),
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@ClassID", (object?)classId ?? DBNull.Value),
                new SqlParameter("@SectionID", (object?)sectionId ?? DBNull.Value),
                new SqlParameter("@DepartmentID", (object?)departmentId ?? DBNull.Value),
                new SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value)
            };
            var dt = _db.ExecuteQuery("sp_Library_Member_CRUD", p);
            var list = new List<LibraryMemberViewModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new LibraryMemberViewModel
                {
                    LibraryMemberID = Convert.ToInt32(row["LibraryMemberID"]),
                    StudentID = row.Table.Columns.Contains("StudentID") && row["StudentID"] != DBNull.Value ? Convert.ToInt32(row["StudentID"]) : null,
                    StaffID = row.Table.Columns.Contains("StaffID") && row["StaffID"] != DBNull.Value ? Convert.ToInt32(row["StaffID"]) : null,
                    LibraryCardNo = row["LibraryCardNo"].ToString(),
                    AdmissionNo = row["AdmissionNo"].ToString(),
                    Name = memberType=="Student"? row["StudentName"].ToString() : row["StaffName"].ToString(),
                    ClassName = row["ClassName"].ToString(),
                    FatherName = row.Table.Columns.Contains("FatherName") ? row["FatherName"].ToString() : "",
                    DOB = row.Table.Columns.Contains("DOB") && row["DOB"] != DBNull.Value ? Convert.ToDateTime(row["DOB"]) : null,
                    Gender = row["Gender"].ToString(),
                    MobileNo = row["MobileNo"].ToString(),
                    RegisteredOn = row["RegisteredOn"] != DBNull.Value ? Convert.ToDateTime(row["RegisteredOn"]) : null
                });
            }
            return list;
        }

        public List<MembershipSearchViewModel> SearchForMembership(string memberType, int companyId, int? classId, int? sectionId, int? departmentId, string? search)
        {
            var action = memberType == "Student" ? "SEARCH_STUDENTS" : "SEARCH_STAFF";
            var p = new[] {
                new SqlParameter("@Action", action),
                new SqlParameter("@CompanyID", companyId),
                new SqlParameter("@ClassID", (object?)classId ?? DBNull.Value),
                new SqlParameter("@SectionID", (object?)sectionId ?? DBNull.Value),
                new SqlParameter("@DepartmentID", (object?)departmentId ?? DBNull.Value),
                new SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value)
            };
            var dt = _db.ExecuteQuery("sp_Library_Member_CRUD", p);
            var list = new List<MembershipSearchViewModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new MembershipSearchViewModel
                {
                    ID = Convert.ToInt32(row["ID"]),
                    AdmissionNo = row["AdmissionNo"].ToString(),
                    Name = row["Name"].ToString(),
                    ExtraInfo = row["ExtraInfo"].ToString()
                });
            }
            return list;
        }

        public (bool Success, string Message) AddMember(LibraryMemberUpsertRequest req, int companyId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@Action", "SAVE"),
                    new SqlParameter("@MemberType", req.MemberType),
                    new SqlParameter("@StudentID", (object?)req.StudentID ?? DBNull.Value),
                    new SqlParameter("@StaffID", (object?)req.StaffID ?? DBNull.Value),
                    new SqlParameter("@LibraryCardNo", (object?)req.LibraryCardNo ?? DBNull.Value),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Library_Member_CRUD", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeleteMember(int id, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@Action", "DELETE"),
                    new SqlParameter("@LibraryMemberID", id),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Library_Member_CRUD", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeleteMemberEx(int id, int? studentId, int? staffId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@Action", "DELETE"),
                    new SqlParameter("@LibraryMemberID", id > 0 ? (object)id : DBNull.Value),
                    new SqlParameter("@StudentID", (object?)studentId ?? DBNull.Value),
                    new SqlParameter("@StaffID", (object?)staffId ?? DBNull.Value),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Library_Member_CRUD", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }
    }
}
