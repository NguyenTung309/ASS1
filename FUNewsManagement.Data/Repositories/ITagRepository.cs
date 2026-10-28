using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNewsManagement.Data.Models;

namespace FUNewsManagement.Data.Repositories
{
    public interface ITagRepository
    {
        Task<Tag> GetByIdAsync(int id);
        Task<List<Tag>> GetAllAsync();
        Task<Tag> GetByNameAsync(string tagName);
        Task<Tag> AddAsync(Tag tag);
        Task<Tag> UpdateAsync(Tag tag);
        Task<bool> DeleteAsync(int id);
        Task<List<Tag>> GetTagsByNewsArticleIdAsync(string newsArticleId);
    }
}