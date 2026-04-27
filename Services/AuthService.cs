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
    /// This service handles the actual work of logging users in. It checks the database and provides a secure pass (token) if the user is who they say they are.
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
        /// This function takes a username and password, asks the database if they match, and returns the user's information along with a security token if successful.
        /// </summary>
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

        /// <summary>
        /// A small tool used to safely get text information from the database, even if it might be missing.
        /// </summary>
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
