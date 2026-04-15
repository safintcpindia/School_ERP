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

                // Get stored hash/salt
                string storedHash = reader["PasswordHash"]?.ToString() ?? "";
                string storedSalt = reader["PasswordSalt"]?.ToString() ?? "";
                bool isLocked = Convert.ToBoolean(reader["IsLocked"]);
                bool isActive = Convert.ToBoolean(reader["IsActive"]);

                // Step 1: Active Directory state checks
                if (isLocked)
                    return (0, "Account is locked due to failing login thresholds", null);
                if (!isActive)
                    return (0, "Account is physically deactivated from portal access", null);

                // Step 2: Verify cryptographic hashes natively in .NET (not SQL)
                // This detaches DB engine processing from brute-force math loads.
                if (!SecurityHelper.VerifyPassword(password, storedHash, storedSalt))
                {
                    // Optionally call a "sp_Users_RecordFailedAttempt" here
                    return (0, "Invalid password", null);
                }

                // Verification successful, move to next result for session data
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
                    DashboardID   = reader["DashboardID"] != DBNull.Value ? Convert.ToInt32(reader["DashboardID"]) : null
                };

                user.Token = _jwtHelper.GenerateToken(
                    user.Username,
                    user.DefaultRoleName,
                    user.UserID,
                    user.UserTypeID,
                    user.DefaultRoleID);

                return (1, "Login successful", user);
            }
            catch (Exception ex)
            {
                return (-1, $"Database error: {ex.Message}", null);
            }
        }
    }
}
