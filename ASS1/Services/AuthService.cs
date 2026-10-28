using ASS1.Data;
using ASS1.Models;
using Microsoft.EntityFrameworkCore;

namespace ASS1.Services
{
    public interface IAuthService
    {
        Task<SystemAccount> Authenticate(string email, string password);
        string GetUserRole(int roleId);
    }

    public class AuthService : IAuthService
    {
        private readonly FUNewsContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(FUNewsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<SystemAccount> Authenticate(string email, string password)
        {
            // Check admin account from appsettings
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (email == adminEmail && password == adminPassword)
            {
                return new SystemAccount
                {
                    AccountEmail = adminEmail,
                    AccountRole = 0, // Admin role
                    AccountName = "Administrator"
                };
            }

            // Check system accounts
            var account = await _context.SystemAccounts
                .FirstOrDefaultAsync(a => a.AccountEmail == email && a.AccountPassword == password);

            return account;
        }

        public string GetUserRole(int roleId)
        {
            return roleId switch
            {
                0 => "Admin",
                1 => "Staff",
                2 => "Lecturer",
                _ => "Unknown"
            };
        }
    }
}