using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNewsManagement.Data.Models;

namespace FUNewsManagement.Data.Repositories
{
    public interface INewsTagRepository
    {
        Task<List<NewsTag>> GetByNewsArticleIdAsync(string newsArticleId);
        Task<NewsTag> AddAsync(NewsTag newsTag);
        Task<bool> DeleteAsync(string newsArticleId, int tagId);
        Task<bool> DeleteByNewsArticleIdAsync(string newsArticleId);
        Task<bool> ExistsAsync(string newsArticleId, int tagId);
    }
}