using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNewsManagement;
namespace FUNewsManagement.Data
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<List<User>> GetAllAsync();
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<List<User>> GetByRoleAsync(int roleId);
    }
}
