using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SchoolERP.Net.Models;
using SchoolERP.Net.Utilities;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This class provides business logic and data access services for AuthService.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IConfiguration configuration, JwtHelper jwtHelper)
        {
            _configuration = configuration;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// Validates plaintext passwords against database hashes via 'sp_User_Login_Secure'.
        /// Produces a JWT Bearer token natively if validation succeeds.
        /// </summary>
        /// <param name="username">User's registered ID or email.</param>
        /// <param name="password">Raw plaintext password submitted from client.</param>
        public async Task<(int Success, string Message, UserSessionModel User)> LoginAsync(string username, string password)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_User_Login_Secure", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordPlain", password);
                
                await conn.OpenAsync();

                // Execute secure stored procedure bypassing raw table access
                using var reader = await cmd.ExecuteReaderAsync();
                
                // Terminate if the identity doesn't physically exist
                if (!await reader.ReadAsync())
                    return (0, "User not found", null);

                int result = Convert.ToInt32(reader["Result"]);
                string message = reader["Message"]?.ToString() ?? "Unknown error";

                if (result == 0)
                    return (0, message, null);

                // Password verification is performed inside the stored procedure.
                // If result is success, move to next result for session data
                if (!await reader.NextResultAsync() || !await reader.ReadAsync())
                    return (0, "Error fetching user details", null);

                TimeSpan startTime = TimeSpan.Zero;
                TimeSpan endTime = TimeSpan.Zero;

                if (reader["StartTime"] != DBNull.Value)
                {
                    startTime = TimeSpan.Parse(reader["StartTime"].ToString());
                }
                if (reader["EndTime"] != DBNull.Value)
                {
                    endTime = TimeSpan.Parse(reader["EndTime"].ToString());
                }
                TimeSpan currentTime = DateTime.Now.TimeOfDay;

                bool isEligible;

                if (startTime <= endTime)
                {
                    // Normal same-day range
                    isEligible = currentTime >= startTime && currentTime <= endTime;
                }
                else
                {
                    // Overnight range (crosses midnight)
                    isEligible = currentTime >= startTime || currentTime <= endTime;
                }
                if(startTime != TimeSpan.Zero && endTime != TimeSpan.Zero)
                {
                    if (isEligible == false)
                        return (0, "Login is not allowed at this time.", null);
                }

                

                var user = new UserSessionModel
                {
                    UserID        = Convert.ToInt32(reader["UserID"]),
                    Username      = reader["Username"]?.ToString()      ?? "",
                    FullName      = reader["FullName"]?.ToString()       ?? "",
                    DefaultRoleID = reader["DefaultRoleID"] != DBNull.Value ? Convert.ToInt32(reader["DefaultRoleID"]) : 0,
                    DefaultRoleName = reader["DefaultRoleName"]?.ToString() ?? "",
                    UserTypeID    = reader["UserTypeID"] != DBNull.Value ? Convert.ToInt32(reader["UserTypeID"]) : 0,
                    DashboardID   = reader["DashboardID"] != DBNull.Value ? Convert.ToInt32(reader["DashboardID"]) : null,
                    

                };

                var userTypeName = GetOptionalString(reader, "UserTypeName")
                                   ?? GetOptionalString(reader, "TypeName")
                                   ?? string.Empty;

                user.Token = _jwtHelper.GenerateToken(
                    user.Username,
                    user.DefaultRoleName,
                    user.UserID,
                    user.UserTypeID,
                    user.DefaultRoleID,
                    userTypeName);

                return (1, "Login successful", user);
            }
            catch (Exception ex)
            {
                return (-1, $"Database error: {ex.Message}", null);
            }
        }

        private static string? GetOptionalString(IDataRecord record, string columnName)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                if (string.Equals(record.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return record.IsDBNull(i) ? null : record.GetValue(i)?.ToString();
                }
            }
            return null;
        }
    }
}
