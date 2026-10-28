using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNewsManagement.Data.Models;

namespace FUNewsManagement.Data.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(short id);
        Task<List<Category>> GetAllAsync();
        Task<List<Category>> GetActiveCategoriesAsync();
        Task<Category> AddAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(short id);
        Task<bool> ToggleActiveAsync(short id);
        Task<bool> HasNewsArticlesAsync(short categoryId);
        Task<List<Category>> GetParentCategoriesAsync();
    }
}