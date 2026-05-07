using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using SchoolERP.Net.Data;
using SchoolERP.Net.Models;
using System.Text.Json;

namespace SchoolERP.Net.Services
{
    public class StudentInformationService : IStudentInformationService
    {
        private readonly SqlHelper _db;
        private readonly IFieldService _fieldService;
        public StudentInformationService(SqlHelper db, IFieldService fieldService)
        {
            _db = db;
            _fieldService = fieldService;

        }

        public List<StudentDisableReasonViewModel> GetAllDisableReasons(int companyId, int sessionId)
        {
            var list = new List<StudentDisableReasonViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId)
                };
                foreach (DataRow row in _db.ExecuteQuery("sp_Student_DisableReason_GetAll", p).Rows)
                {
                    list.Add(new StudentDisableReasonViewModel
                    {
                        DisableReasonID = Convert.ToInt32(row["DisableReasonID"]),
                        DisableReasonTitle = row["DisableReasonTitle"].ToString()!,
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        CreatedOn = Convert.ToDateTime(row["CreatedOn"])
                    });
                }
            }
            catch { }
            return list;
        }

        public (bool Success, string Message) UpsertDisableReason(StudentDisableReasonUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@DisableReasonID", req.DisableReasonID),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@DisableReasonTitle", req.DisableReasonTitle),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Student_DisableReason_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeleteDisableReason(int id, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@DisableReasonID", id),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Student_DisableReason_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public List<StudentHouseViewModel> GetAllStudentHouses(int companyId, int sessionId)
        {
            var list = new List<StudentHouseViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId)
                };
                foreach (DataRow row in _db.ExecuteQuery("sp_MST_StudentHouse_GetAll", p).Rows)
                {
                    list.Add(new StudentHouseViewModel
                    {
                        StudentHouseID = Convert.ToInt32(row["StudentHouseID"]),
                        StudentHouseName = row["StudentHouseName"].ToString()!,
                        StudentHouseDescription = row["StudentHouseDescription"]?.ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        CreatedOn = Convert.ToDateTime(row["CreatedOn"])
                    });
                }
            }
            catch { }
            return list;
        }

        public (bool Success, string Message) UpsertStudentHouse(StudentHouseUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@StudentHouseID", req.StudentHouseID),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@StudentHouseName", req.StudentHouseName),
                    new SqlParameter("@StudentHouseDescription", (object?)req.StudentHouseDescription ?? DBNull.Value),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_MST_StudentHouse_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeleteStudentHouse(int id, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@StudentHouseID", id),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_MST_StudentHouse_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public List<StudentCategoryViewModel> GetAllStudentCategories(int companyId, int sessionId)
        {
            var list = new List<StudentCategoryViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId)
                };
                foreach (DataRow row in _db.ExecuteQuery("sp_MST_StudentCategory_GetAll", p).Rows)
                {
                    list.Add(new StudentCategoryViewModel
                    {
                        StudentCategoryID = Convert.ToInt32(row["StudentCategoryID"]),
                        StudentCategoryName = row["StudentCategoryName"].ToString()!,
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        CreatedOn = Convert.ToDateTime(row["CreatedOn"])
                    });
                }
            }
            catch { }
            return list;
        }

        public (bool Success, string Message) UpsertStudentCategory(StudentCategoryUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@StudentCategoryID", req.StudentCategoryID),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@StudentCategoryName", req.StudentCategoryName),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_MST_StudentCategory_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeleteStudentCategory(int id, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@StudentCategoryID", id),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_MST_StudentCategory_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public string GetNewStudentRollNo(int companyId, int sessionId, Dictionary<string, string> dynamicValues = null)
        {
            try
            {
                string jsonValues = dynamicValues != null ? System.Text.Json.JsonSerializer.Serialize(dynamicValues) : null;
                var p = new[] {
                    new SqlParameter("@EntityType", "Student"),
                    new SqlParameter("@CompanyID", companyId),
                    new SqlParameter("@SessionID", sessionId),
                    new SqlParameter("@FieldValues", (object?)jsonValues ?? DBNull.Value)
                };
                var dt = _db.ExecuteQuery("sp_Settings_IDAutoGen_GetNext", p);
                return dt.Rows.Count > 0 ? dt.Rows[0]["NextID"].ToString()! : "STU" + DateTime.Now.Ticks.ToString().Substring(10);
            }
            catch { return "STU" + DateTime.Now.Ticks.ToString().Substring(10); }
        }

        public string GetNextSimpleAdmissionNo(int companyId)
        {
            try
            {
                var p = new[] { new SqlParameter("@CompanyID", companyId) };
                var dt = _db.ExecuteQuery("sp_Student_GetNextAdmissionNo", p);
                return dt.Rows.Count > 0 ? dt.Rows[0]["NEXTADMISSIONNO"].ToString()! : "1";
            }
            catch { return "1"; }
        }

        private string GenerateRandomPassword(int length = 6)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public (bool Success, string Message, int StudentID) UpsertStudentAdmission(StudentAdmissionUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                // Fetch ALL fields for this company/session to ensure we find Misc fields too
                var allFields = _fieldService.GetAllFields(companyId, sessionId, belongsTo: null);
                
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@STUDENTID", req.StudentID),
                    new SqlParameter("@COMPANYID", companyId),
                    new SqlParameter("@SESSIONID", sessionId),
                    new SqlParameter("@USERID", userId)
                };

                // Auto-generate credentials
                string rollNo = req.RollNo;
                if (string.IsNullOrEmpty(rollNo) && req.FieldValues != null) 
                    rollNo = req.FieldValues.ContainsKey("Roll No") ? req.FieldValues["Roll No"] : "";

                if (!string.IsNullOrEmpty(rollNo) && req.FieldValues != null)
                {
                    if (!req.FieldValues.ContainsKey("Student Username") || string.IsNullOrEmpty(req.FieldValues["Student Username"]))
                        req.FieldValues["Student Username"] = rollNo;
                    if (!req.FieldValues.ContainsKey("Student Password") || string.IsNullOrEmpty(req.FieldValues["Student Password"]))
                        req.FieldValues["Student Password"] = GenerateRandomPassword(6);
                    if (!req.FieldValues.ContainsKey("Parent Username") || string.IsNullOrEmpty(req.FieldValues["Parent Username"]))
                        req.FieldValues["Parent Username"] = "P_" + rollNo;
                    if (!req.FieldValues.ContainsKey("Parent Password") || string.IsNullOrEmpty(req.FieldValues["Parent Password"]))
                        req.FieldValues["Parent Password"] = GenerateRandomPassword(6);
                }

                // Auto-generate Admission No for new admissions if empty
                if (req.StudentID == 0)
                {
                    var admissionKey = req.FieldValues.Keys.FirstOrDefault(k => string.Equals(k, "Admission No", StringComparison.OrdinalIgnoreCase) 
                                                                             || string.Equals(k, "AdmissionNo", StringComparison.OrdinalIgnoreCase)
                                                                             || string.Equals(k, "Admission Number", StringComparison.OrdinalIgnoreCase));
                    
                    if (admissionKey == null)
                    {
                        req.FieldValues["Admission No"] = GetNextSimpleAdmissionNo(companyId);
                    }
                    else if (string.IsNullOrEmpty(req.FieldValues[admissionKey]))
                    {
                        req.FieldValues[admissionKey] = GetNextSimpleAdmissionNo(companyId);
                    }
                }

                var customFieldsDt = new DataTable();
                customFieldsDt.Columns.Add("FIELDID", typeof(int));
                customFieldsDt.Columns.Add("FIELDVALUE", typeof(string));

                var mapping = new Dictionary<string, (string ParamName, string Type)>(StringComparer.OrdinalIgnoreCase) {
                    
                    { "Roll No", ("@ROLLNO", "string") },
                    { "Admission No", ("@ADMISSIONNO", "string") },
                    
                    { "Admission Number", ("@ADMISSIONNO", "string") },
                    { "Admission Date", ("@ADMISSIONDATE", "date") },
                    { "Class", ("@CLASSID", "int") },
                    { "Section", ("@SECTIONID", "int") },
                    { "First Name", ("@FIRSTNAME", "string") },
                    { "Middle Name", ("@MIDDLENAME", "string") },
                    { "Last Name", ("@LASTNAME", "string") },
                    { "Gender", ("@GENDER", "string") },
                    { "Date of Birth", ("@DOB", "date") },
                    { "Category", ("@CATEGORYID", "int") },
                    { "Religion", ("@RELIGION", "string") },
                    { "Caste", ("@CASTE", "string") },
                    
                    { "Mobile Number", ("@MOBILENO", "string") },
                    { "Email", ("@EMAIL", "string") },
                    { "Blood Group", ("@BLOODGROUP", "string") },
                    { "House", ("@HOUSEID", "int") },
                    { "Height", ("@HEIGHT", "string") },
                    { "Weight", ("@WEIGHT", "string") },
                    { "Route List", ("@VEHICLEID", "int") },
                    { "TransportRouteID", ("@ROUTEID", "int") },
                    { "Pickup Point", ("@PICKUPPOINTID", "int") },
                    { "Fees Month", ("@TRANSPORTMONTH", "string") },
                    { "Hostel", ("@HOSTELID", "int") },
                    { "Room No", ("@ROOMID", "int") },
                    { "Father Name", ("@FATHERNAME", "string") },
                    { "Father Phone", ("@FATHERPHONE", "string") },
                    { "Father Mobile", ("@FATHERPHONE", "string") },
                    { "Father Occupation", ("@FATHEROCCUPATION", "string") },
                    { "Mother Name", ("@MOTHERNAME", "string") },
                    { "Mother Phone", ("@MOTHERPHONE", "string") },
                    { "Mother Mobile", ("@MOTHERPHONE", "string") },
                    { "Mother Occupation", ("@MOTHEROCCUPATION", "string") },
                    { "If Guardian Is", ("@IFGUARDIANIS", "string") },
                    { "Guardian Name", ("@GUARDIANNAME", "string") },
                    { "Guardian Phone", ("@GUARDIANPHONE", "string") },
                    { "Guardian Mobile", ("@GUARDIANPHONE", "string") },
                    { "Guardian Occupation", ("@GUARDIANOCCUPATION", "string") },
                    
                    { "Guardian Relation", ("@GUARDIANRELATION", "string") },
                    { "Guardian Email", ("@GUARDIANEMAIL", "string") },
                    { "CurrentAddress", ("@CURRENTADDRESS", "string") },
                    { "PermanentAddress", ("@PERMANENTADDRESS", "string") },
                    { "Student Username", ("@STUDENTUSERNAME", "string") },
                    { "Student Password", ("@STUDENTPASSWORD", "string") },
                    { "Parent Username", ("@PARENTUSERNAME", "string") },
                    { "Parent Password", ("@PARENTPASSWORD", "string") },
                    { "Student Photo", ("@STUDENTPHOTO", "byte[]") },
                    { "Student Photo Name", ("@STUDENTPHOTONAME", "string") },
                    { "Student Photo Type", ("@STUDENTPHOTOTYPE", "string") },
                    { "Father Photo", ("@FATHERPHOTO", "byte[]") },
                    { "Father Photo Name", ("@FATHERPHOTONAME", "string") },
                    { "Father Photo Type", ("@FATHERPHOTOTYPE", "string") },
                    { "Mother Photo", ("@MOTHERPHOTO", "byte[]") },
                    { "Mother Photo Name", ("@MOTHERPHOTONAME", "string") },
                    { "Mother Photo Type", ("@MOTHERPHOTOTYPE", "string") },
                    { "Guardian Photo", ("@GUARDIANPHOTO", "byte[]") },
                    { "Guardian Photo Name", ("@GUARDIANPHOTONAME", "string") },
                    { "Guardian Photo Type", ("@GUARDIANPHOTOTYPE", "string") },
                   
                };

                // 2. Initialize all mapped parameters with DBNull.Value and correct SqlDbType
                foreach (var mapGrp in mapping.Values.GroupBy(v => v.ParamName))
                {
                    var map = mapGrp.First();
                    var param = new SqlParameter(map.ParamName, DBNull.Value);
                    
                    if (map.Type == "int") param.SqlDbType = SqlDbType.Int;
                    else if (map.Type == "date" || map.Type == "datetime") param.SqlDbType = SqlDbType.DateTime;
                    else if (map.Type == "byte[]") param.SqlDbType = SqlDbType.VarBinary;
                    else param.SqlDbType = SqlDbType.NVarChar;

                    parameters.Add(param);
                }

                // 3. Process incoming FieldValues
                if (req.FieldValues != null)
                {
                    foreach (var field in req.FieldValues)
                    {
                        string trimmedKey = field.Key?.Trim() ?? "";
                        if (mapping.ContainsKey(trimmedKey))
                        {
                            var map = mapping[trimmedKey];
                            var existingParam = parameters.FirstOrDefault(p => p.ParameterName == map.ParamName);
                            object val = null;
                            try 
                            {
                                if (string.IsNullOrEmpty(field.Value)) val = DBNull.Value;
                                else if (map.Type == "int") val = int.Parse(field.Value);
                                else if (map.Type == "date" || map.Type == "datetime") 
                                {
                                    DateTime dtVal;
                                    if (DateTime.TryParse(field.Value, out dtVal)) val = dtVal;
                                    else val = DBNull.Value;
                                }
                                else if (map.Type == "byte[]") 
                                {
                                    try { val = Convert.FromBase64String(field.Value); }
                                    catch { val = DBNull.Value; }
                                }
                                else val = field.Value;
                                if (existingParam != null) existingParam.Value = val ?? DBNull.Value;
                                if (map.ParamName == "@ROLLNO" && string.IsNullOrEmpty(rollNo)) rollNo = field.Value;
                            } 
                            catch { /* Keep DBNull */ }
                        }
                        else
                        {
                            // Case-insensitive and trimmed lookup for custom fields
                            var fieldDef = allFields.FirstOrDefault(f => 
                                string.Equals(f.FieldName?.Trim(), field.Key?.Trim(), StringComparison.OrdinalIgnoreCase)
                            );
                            
                            if (fieldDef != null)
                            {
                                customFieldsDt.Rows.Add(fieldDef.FieldId, field.Value ?? "");
                            }
                        }
                    }
                }

                // 3. Handle Documents (DocumentTitle_X, DocumentFile_X)
                var documentsDt = new DataTable();
                documentsDt.Columns.Add("DocumentTitle", typeof(string));
                documentsDt.Columns.Add("DocumentContent", typeof(byte[]));
                documentsDt.Columns.Add("DocumentPath", typeof(string));

                for (int i = 1; i <= 10; i++) // Support up to 10 docs for safety
                {
                    string titleKey = $"DocumentTitle_{i}";
                    string fileKey = $"DocumentFile_{i}";
                    string nameKey = $"DocumentFile_{i} Name";

                    if (req.FieldValues.ContainsKey(titleKey) || req.FieldValues.ContainsKey(fileKey))
                    {
                        string title = req.FieldValues.GetValueOrDefault(titleKey) ?? "";
                        string base64 = req.FieldValues.GetValueOrDefault(fileKey) ?? "";
                        string fileName = req.FieldValues.GetValueOrDefault(nameKey) ?? "";

                        if (!string.IsNullOrEmpty(base64))
                        {
                            try 
                            {
                                byte[] content = Convert.FromBase64String(base64);
                                documentsDt.Rows.Add(title, content, fileName);
                            }
                            catch { /* Skip invalid base64 */ }
                        }
                    }
                }

                var customFieldsParam = new SqlParameter("@CUSTOMFIELDS", customFieldsDt);
                customFieldsParam.SqlDbType = SqlDbType.Structured;
                customFieldsParam.TypeName = "TYPE_STUDENTCUSTOMFIELDS";
                parameters.Add(customFieldsParam);

                var documentsParam = new SqlParameter("@DOCUMENTS", documentsDt);
                documentsParam.SqlDbType = SqlDbType.Structured;
                documentsParam.TypeName = "TYPE_STUDENTDOCUMENTS";
                parameters.Add(documentsParam);

                var dt = _db.ExecuteQuery("SP_STUDENT_ADMISSION_UPSERT", parameters.ToArray());
                
                return (
                    Convert.ToInt32(dt.Rows[0]["RESULT"]) > 0, 
                    "Student record saved successfully.", 
                    Convert.ToInt32(dt.Rows[0]["RESULT"])
                );
            }
            catch (Exception ex) { return (false, ex.Message, 0); }
        }

        public StudentDetailsViewModel GetStudentDetails(int studentId, int companyId, int sessionId)
        {
            var model = new StudentDetailsViewModel();
            try
            {
                var p = new[] {
                    new SqlParameter("@STUDENTID", studentId),
                    new SqlParameter("@COMPANYID", companyId),
                    new SqlParameter("@SESSIONID", sessionId)
                };
                var ds = _db.ExecuteDataSet("SP_STUDENT_DETAILS_GET", p);
                
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var row = ds.Tables[0].Rows[0];
                    var cols = row.Table.Columns;

                    model.BasicInfo = new StudentBasicInfoViewModel
                    {
                        StudentID = cols.Contains("STUDENTID") ? Convert.ToInt32(row["STUDENTID"]) : 0,
                        RollNo = cols.Contains("ROLLNO") ? row["ROLLNO"]?.ToString() : null,
                        AdmissionNo = cols.Contains("ADMISSIONNO") ? row["ADMISSIONNO"]?.ToString() : null,
                        AdmissionDate = cols.Contains("ADMISSIONDATE") && row["ADMISSIONDATE"] != DBNull.Value ? Convert.ToDateTime(row["ADMISSIONDATE"]) : null,
                        FirstName = cols.Contains("FIRSTNAME") ? row["FIRSTNAME"]?.ToString() : null,
                        MiddleName = cols.Contains("MIDDLENAME") ? row["MIDDLENAME"]?.ToString() : null,
                        LastName = cols.Contains("LASTNAME") ? row["LASTNAME"]?.ToString() : null,
                        Gender = cols.Contains("GENDER") ? row["GENDER"]?.ToString() : null,
                        DOB = cols.Contains("DOB") && row["DOB"] != DBNull.Value ? Convert.ToDateTime(row["DOB"]) : null,
                        
                        CategoryName = cols.Contains("StudentCategoryName") ? row["StudentCategoryName"]?.ToString() : (cols.Contains("CATEGORYNAME") ? row["CATEGORYNAME"]?.ToString() : null),
                        Religion = cols.Contains("RELIGION") ? row["RELIGION"]?.ToString() : null,
                        Caste = cols.Contains("CASTE") ? row["CASTE"]?.ToString() : null,
                        MobileNo = cols.Contains("MOBILENO") ? row["MOBILENO"]?.ToString() : null,
                        Email = cols.Contains("EMAIL") ? row["EMAIL"]?.ToString() : null,
                        BloodGroup = cols.Contains("BLOODGROUP") ? row["BLOODGROUP"]?.ToString() : null,
                        HouseName = cols.Contains("StudentHouseName") ? row["StudentHouseName"]?.ToString() : (cols.Contains("HOUSENAME") ? row["HOUSENAME"]?.ToString() : null),
                        Height = cols.Contains("HEIGHT") ? row["HEIGHT"]?.ToString() : null,
                        Weight = cols.Contains("WEIGHT") ? row["WEIGHT"]?.ToString() : null,
                        
                        ClassName = cols.Contains("ClassName") ? row["ClassName"]?.ToString() : (cols.Contains("CLASSNAME") ? row["CLASSNAME"]?.ToString() : null),
                        ClassID = cols.Contains("CLASSID") ? Convert.ToInt32(row["CLASSID"]) : (cols.Contains("ClassID") ? Convert.ToInt32(row["ClassID"]) : 0),
                        SectionName = cols.Contains("SectionName") ? row["SectionName"]?.ToString() : (cols.Contains("SECTIONNAME") ? row["SECTIONNAME"]?.ToString() : null),
                        SectionID = cols.Contains("SECTIONID") ? Convert.ToInt32(row["SECTIONID"]) : (cols.Contains("SectionID") ? Convert.ToInt32(row["SectionID"]) : 0),
                        
                        StudentCategoryID = cols.Contains("STUDENTCATEGORYID") ? Convert.ToInt32(row["STUDENTCATEGORYID"]) : (cols.Contains("CATEGORYID") ? Convert.ToInt32(row["CATEGORYID"]) : 0),
                        StudentHouseID = cols.Contains("STUDENTHOUSEID") && row["STUDENTHOUSEID"] != DBNull.Value ? Convert.ToInt32(row["STUDENTHOUSEID"]) : (cols.Contains("HOUSEID") && row["HOUSEID"] != DBNull.Value ? Convert.ToInt32(row["HOUSEID"]) : null),
                        
                        StudentPhoto = cols.Contains("STUDENTPHOTO") && row["STUDENTPHOTO"] != DBNull.Value ? (byte[])row["STUDENTPHOTO"] : null,
                        StudentPhotoType = cols.Contains("STUDENTPHOTOTYPE") ? row["STUDENTPHOTOTYPE"]?.ToString() : null,
                        
                        FatherName = cols.Contains("FATHERNAME") ? row["FATHERNAME"]?.ToString() : null,
                        FatherPhone = cols.Contains("FATHERPHONE") ? row["FATHERPHONE"]?.ToString() : null,
                        FatherOccupation = cols.Contains("FATHEROCCUPATION") ? row["FATHEROCCUPATION"]?.ToString() : null,
                        FatherPhoto = cols.Contains("FATHERPHOTO") && row["FATHERPHOTO"] != DBNull.Value ? (byte[])row["FATHERPHOTO"] : null,
                        FatherPhotoType = cols.Contains("FATHERPHOTOTYPE") ? row["FATHERPHOTOTYPE"]?.ToString() : null,
                        
                        MotherName = cols.Contains("MOTHERNAME") ? row["MOTHERNAME"]?.ToString() : null,
                        MotherPhone = cols.Contains("MOTHERPHONE") ? row["MOTHERPHONE"]?.ToString() : null,
                        MotherOccupation = cols.Contains("MOTHEROCCUPATION") ? row["MOTHEROCCUPATION"]?.ToString() : null,
                        MotherPhoto = cols.Contains("MOTHERPHOTO") && row["MOTHERPHOTO"] != DBNull.Value ? (byte[])row["MOTHERPHOTO"] : null,
                        MotherPhotoType = cols.Contains("MOTHERPHOTOTYPE") ? row["MOTHERPHOTOTYPE"]?.ToString() : null,
                        
                        IfGuardianIs = cols.Contains("IFGUARDIANIS") ? row["IFGUARDIANIS"]?.ToString() : null,
                        GuardianName = cols.Contains("GUARDIANNAME") ? row["GUARDIANNAME"]?.ToString() : null,
                        GuardianPhone = cols.Contains("GUARDIANPHONE") ? row["GUARDIANPHONE"]?.ToString() : null,
                        GuardianOccupation = cols.Contains("GUARDIANOCCUPATION") ? row["GUARDIANOCCUPATION"]?.ToString() : null,
                        GuardianRelation = cols.Contains("GUARDIANRELATION") ? row["GUARDIANRELATION"]?.ToString() : null,
                        GuardianEmail = cols.Contains("GUARDIANEMAIL") ? row["GUARDIANEMAIL"]?.ToString() : null,
                        GuardianPhoto = cols.Contains("GUARDIANPHOTO") && row["GUARDIANPHOTO"] != DBNull.Value ? (byte[])row["GUARDIANPHOTO"] : null,
                        GuardianPhotoType = cols.Contains("GUARDIANPHOTOTYPE") ? row["GUARDIANPHOTOTYPE"]?.ToString() : null,
                        
                        StudentUsername = cols.Contains("StudentUsername") ? row["StudentUsername"]?.ToString() : (cols.Contains("STUDENTUSERNAME") ? row["STUDENTUSERNAME"]?.ToString() : null),
                        ParentUsername = cols.Contains("ParentUsername") ? row["ParentUsername"]?.ToString() : (cols.Contains("PARENTUSERNAME") ? row["PARENTUSERNAME"]?.ToString() : null),
                        StudentPassword = cols.Contains("StudentPlainPassword") ? row["StudentPlainPassword"]?.ToString() : null,
                        ParentPassword = cols.Contains("ParentPlainPassword") ? row["ParentPlainPassword"]?.ToString() : null,
                        ParentUserID = cols.Contains("PARENTUSERID") && row["PARENTUSERID"] != DBNull.Value ? Convert.ToInt32(row["PARENTUSERID"]) : null
                    };
                }

                if (ds.Tables.Count > 1)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        model.Addresses.Add(new StudentAddressViewModel
                        {
                            AddressType = row.Table.Columns.Contains("ADDRESSTYPE") ? row["ADDRESSTYPE"]?.ToString() : null,
                            AddressDetails = row.Table.Columns.Contains("ADDRESSDETAILS") ? row["ADDRESSDETAILS"]?.ToString() : null
                        });
                    }
                }

                if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                {
                    var row = ds.Tables[2].Rows[0];
                    model.Transport = new StudentTransportDetailsViewModel
                    {
                        RouteName = row.Table.Columns.Contains("RouteName") ? row["RouteName"]?.ToString() : null,
                        RouteID = row.Table.Columns.Contains("ROUTEID") && row["ROUTEID"] != DBNull.Value ? Convert.ToInt32(row["ROUTEID"]) : null,
                        VehicleID = row.Table.Columns.Contains("VEHICLEID") && row["VEHICLEID"] != DBNull.Value ? Convert.ToInt32(row["VEHICLEID"]) : (row.Table.Columns.Contains("VehicleID") && row["VehicleID"] != DBNull.Value ? Convert.ToInt32(row["VehicleID"]) : null),
                        PickupPointName = row.Table.Columns.Contains("PickupPointName") ? row["PickupPointName"]?.ToString() : null,
                        PickupPointID = row.Table.Columns.Contains("PICKUPPOINTID") && row["PICKUPPOINTID"] != DBNull.Value ? Convert.ToInt32(row["PICKUPPOINTID"]) : null,
                        StartMonth = row.Table.Columns.Contains("STARTMONTH") ? row["STARTMONTH"]?.ToString() : null
                    };
                }

                if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
                {
                    var row = ds.Tables[3].Rows[0];
                    model.Hostel = new StudentHostelDetailsViewModel
                    {
                        HostelName = row.Table.Columns.Contains("HostelName") ? row["HostelName"]?.ToString() : null,
                        HostelID = row.Table.Columns.Contains("HOSTELID") && row["HOSTELID"] != DBNull.Value ? Convert.ToInt32(row["HOSTELID"]) : null,
                        RoomTitle = row.Table.Columns.Contains("RoomTitle") ? row["RoomTitle"]?.ToString() : null,
                        RoomID = row.Table.Columns.Contains("ROOMID") && row["ROOMID"] != DBNull.Value ? Convert.ToInt32(row["ROOMID"]) : null
                    };
                }

                if (ds.Tables.Count > 4)
                {
                    foreach (DataRow row in ds.Tables[4].Rows)
                    {
                        model.CustomFields.Add(new StudentCustomFieldValueViewModel
                        {
                            FieldID = row.Table.Columns.Contains("FIELDID") ? Convert.ToInt32(row["FIELDID"]) : 0,
                            FieldName = row.Table.Columns.Contains("FIELDNAME") ? row["FIELDNAME"]?.ToString() : null,
                            FieldValue = row.Table.Columns.Contains("FIELDVALUE") ? row["FIELDVALUE"]?.ToString() : null
                        });
                    }
                }

                if (ds.Tables.Count > 5)
                {
                    foreach (DataRow row in ds.Tables[5].Rows)
                    {
                        model.Documents.Add(new StudentDocumentViewModel
                        {
                            DocID = row.Table.Columns.Contains("DOCID") ? Convert.ToInt32(row["DOCID"]) : 0,
                            DocumentTitle = row.Table.Columns.Contains("DOCUMENTTITLE") ? row["DOCUMENTTITLE"]?.ToString() : null,
                            DocumentPath = row.Table.Columns.Contains("DOCUMENTPATH") ? row["DOCUMENTPATH"]?.ToString() : null,
                            DocumentContent = row.Table.Columns.Contains("DOCUMENTCONTENT") && row["DOCUMENTCONTENT"] != DBNull.Value ? (byte[])row["DOCUMENTCONTENT"] : null
                        });
                    }
                }
            }
            catch (Exception ex) 
            { 
                // Consider logging ex here
            }
            return model;
        }

        public List<StudentListViewModel> GetStudentList(int companyId, int sessionId, int? classId, int? sectionId, string? searchTerm)
        {
            var list = new List<StudentListViewModel>();
            try
            {
                var p = new[] {
                    new SqlParameter("@COMPANYID", companyId),
                    new SqlParameter("@SESSIONID", sessionId),
                    new SqlParameter("@CLASSID", (object?)classId ?? DBNull.Value),
                    new SqlParameter("@SECTIONID", (object?)sectionId ?? DBNull.Value),
                    new SqlParameter("@SEARCHTERM", (object?)searchTerm ?? DBNull.Value)
                };
                var dt = _db.ExecuteQuery("SP_STUDENT_LIST_GET", p);
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new StudentListViewModel
                    {
                        StudentID = Convert.ToInt32(row["STUDENTID"]),
                        AdmissionNo = row["ADMISSIONNO"]?.ToString(),
                        RollNo = row["ROLLNO"]?.ToString(),
                        FullName = $"{row["FIRSTNAME"]} {row["MIDDLENAME"]} {row["LASTNAME"]}".Trim().Replace("  ", " "),
                        ClassName = row["ClassName"]?.ToString(),
                        SectionName = row["SectionName"]?.ToString(),
                        FatherName = row["FATHERNAME"]?.ToString(),
                        FatherPhone = row["FATHERPHONE"]?.ToString(),
                        Gender = row["GENDER"]?.ToString(),
                        DOB = row["DOB"] != DBNull.Value ? Convert.ToDateTime(row["DOB"]) : null,
                        CategoryName = row["StudentCategoryName"]?.ToString(),
                        MobileNo = row["MOBILENO"]?.ToString(),
                        StudentPhoto = row["STUDENTPHOTO"] != DBNull.Value ? (byte[])row["STUDENTPHOTO"] : null,
                        StudentPhotoType = row["STUDENTPHOTOTYPE"]?.ToString()
                    });
                }
            }
            catch { }
            return list;
        }

        public List<StudentTimelineViewModel> GetStudentTimeline(int studentId)
        {
            var list = new List<StudentTimelineViewModel>();
            try
            {
                var dt = _db.ExecuteQuery("sp_Student_Timeline_Get", new[] { new SqlParameter("@StudentID", studentId) });
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new StudentTimelineViewModel
                    {
                        TimelineID = Convert.ToInt32(row["TimelineID"]),
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        Title = row["Title"]?.ToString() ?? "",
                        TimelineDate = Convert.ToDateTime(row["TimelineDate"]),
                        Description = row["Description"]?.ToString(),
                        DocumentName = row["DocumentName"]?.ToString(),
                        DocumentType = row["DocumentType"]?.ToString(),
                        IsVisibleToStudent = Convert.ToBoolean(row["IsVisibleToStudent"]),
                        HasDocument = Convert.ToBoolean(row["HasDocument"])
                    });
                }
            }
            catch { }
            return list;
        }

        public (bool Success, string Message) UpsertStudentTimeline(StudentTimelineUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                byte[]? docBytes = null;
                if (!string.IsNullOrEmpty(req.DocumentBase64))
                {
                    docBytes = Convert.FromBase64String(req.DocumentBase64);
                }

                var p = new[] {
                    new SqlParameter("@TimelineID", req.TimelineID),
                    new SqlParameter("@StudentID", req.StudentID),
                    new SqlParameter("@Title", req.Title),
                    new SqlParameter("@TimelineDate", req.TimelineDate),
                    new SqlParameter("@Description", (object?)req.Description ?? DBNull.Value),
                    new SqlParameter("@DocumentContent", (object?)docBytes ?? DBNull.Value),
                    new SqlParameter("@DocumentName", (object?)req.DocumentName ?? DBNull.Value),
                    new SqlParameter("@DocumentType", (object?)req.DocumentType ?? DBNull.Value),
                    new SqlParameter("@IsVisibleToStudent", req.IsVisibleToStudent),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Student_Timeline_Upsert", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (bool Success, string Message) DeleteStudentTimeline(int id, int userId)
        {
            try
            {
                var p = new[] {
                    new SqlParameter("@TimelineID", id),
                    new SqlParameter("@UserID", userId)
                };
                var dt = _db.ExecuteQuery("sp_Student_Timeline_Delete", p);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString()!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public (byte[] Bytes, string FileName, string ContentType) GetStudentTimelineDocument(int id)
        {
            try
            {
                var p = new[] { new SqlParameter("@TimelineID", id) };
                var dt = _db.ExecuteQuery("sp_Student_Timeline_GetDocument", p);
                if (dt.Rows.Count > 0 && dt.Rows[0]["DocumentContent"] != DBNull.Value)
                {
                    return ((byte[])dt.Rows[0]["DocumentContent"], dt.Rows[0]["DocumentName"].ToString()!, dt.Rows[0]["DocumentType"].ToString()!);
                }
            }
            catch { }
            return (null!, null!, null!);
        }
        public (bool Success, string Message) DeleteStudent(int id, int userId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@STUDENTID", id),
                    new SqlParameter("@USERID", userId)
                };

                var dt = _db.ExecuteQuery("SP_STUDENT_DELETE", parameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return (Convert.ToInt32(dt.Rows[0]["RESULT"]) > 0, dt.Rows[0]["MESSAGE"].ToString());
                }
                return (false, "Failed to delete student record.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
