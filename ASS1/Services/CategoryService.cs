using ASS1.Models;
using ASS1.Data;
using System;
using Microsoft.EntityFrameworkCore;

namespace ASS1.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly FUNewsContext _context;

        public CategoryService(IRepository<Category> categoryRepository, FUNewsContext context)
        {
            _categoryRepository = categoryRepository;
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
            => await _categoryRepository.GetAllAsync();

        public async Task<Category> GetCategoryByIdAsync(short id)
            => await _categoryRepository.GetByIdAsync(id);

        public async Task AddCategoryAsync(Category category)
        {
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveAsync();
        }

        public async Task DeleteCategoryAsync(short id)
        {
            await _categoryRepository.DeleteAsync(id);
            await _categoryRepository.SaveAsync();
        }

        public async Task<bool> CanDeleteCategoryAsync(short id)
        {
            return !await _context.NewsArticles.AnyAsync(na => na.CategoryId == id);
        }
    }
}