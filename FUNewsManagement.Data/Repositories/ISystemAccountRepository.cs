using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNewsManagement.Data.Models;

namespace FUNewsManagement.Data.Repositories
{
    public interface ISystemAccountRepository
    {
        Task<SystemAccount> GetByIdAsync(short id);
        Task<List<SystemAccount>> GetAllAsync();
        Task<SystemAccount> GetByEmailAsync(string email);
        Task<List<SystemAccount>> GetByRoleAsync(int role);
        Task<SystemAccount> AddAsync(SystemAccount account);
        Task<SystemAccount> UpdateAsync(SystemAccount account);
        Task<bool> DeleteAsync(short id);
        Task<bool> EmailExistsAsync(string email, short? excludeAccountId = null);
    }
}