using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNewsManagement.Data.Models;

namespace FUNewsManagement.Data.Repositories
{
    public interface INewsArticleRepository
    {
        Task<NewsArticle> GetByIdAsync(string id);
        Task<List<NewsArticle>> GetAllAsync();
        Task<List<NewsArticle>> GetActiveArticlesAsync();
        Task<List<NewsArticle>> GetByUserIdAsync(short userId);
        Task<List<NewsArticle>> GetByCategoryIdAsync(short categoryId);
        Task<List<NewsArticle>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<NewsArticle>> SearchAsync(string keyword);
        Task<NewsArticle> AddAsync(NewsArticle newsArticle);
        Task<NewsArticle> UpdateAsync(NewsArticle newsArticle);
        Task<bool> DeleteAsync(string id);
        Task<bool> ToggleStatusAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> GetCountAsync();
        Task<int> GetActiveCountAsync();
        Task<List<NewsArticle>> GetNewsWithTagsAsync();
    }
}