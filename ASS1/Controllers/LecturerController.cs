using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASS1.Data;
using ASS1.Models;

namespace ASS1.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : BaseController
    {
        private readonly FUNewsContext _context;

        public LecturerController(FUNewsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // News Article CRUD
        public async Task<IActionResult> NewsArticles()
        {
            var articles = await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags) // Include tags directly
                .Where(n => n.CreatedById == CurrentAccountId)
                .ToListAsync();
            return View(articles);
        }

        [HttpGet]
        public IActionResult CreateNewsArticle()
        {
            ViewBag.Categories = _context.Categories.Where(c => c.IsActive == true).ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewsArticle(NewsArticle article, List<int> selectedTags)
        {
            if (ModelState.IsValid)
            {
                article.CreatedById = (short?)CurrentAccountId;
                article.CreatedDate = DateTime.Now;
                article.ModifiedDate = DateTime.Now;

                // Add tags through the many-to-many relationship
                if (selectedTags != null)
                {
                    var selectedTagsList = await _context.Tags
                        .Where(t => selectedTags.Contains(t.TagId))
                        .ToListAsync();

                    foreach (var tag in selectedTagsList)
                    {
                        article.Tags.Add(tag);
                    }
                }

                _context.Add(article);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(NewsArticles));
            }
            ViewBag.Categories = _context.Categories.Where(c => c.IsActive == true).ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View(article);
        }

        [HttpGet]
        public async Task<IActionResult> EditNewsArticle(string id)
        {
            if (id == null) return NotFound();

            var article = await _context.NewsArticles
                .Include(n => n.Tags) // Include tags
                .FirstOrDefaultAsync(n => n.NewsArticleId == id && n.CreatedById == CurrentAccountId);

            if (article == null) return NotFound();

            ViewBag.Categories = _context.Categories.Where(c => c.IsActive == true).ToList();
            ViewBag.Tags = _context.Tags.ToList();

            // Get selected tags for this article
            var selectedTagIds = article.Tags.Select(t => t.TagId).ToList();
            ViewBag.SelectedTags = selectedTagIds;

            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNewsArticle(string id, NewsArticle article, List<int> selectedTags)
        {
            if (id != article.NewsArticleId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingArticle = await _context.NewsArticles
                        .Include(a => a.Tags)
                        .FirstOrDefaultAsync(a => a.NewsArticleId == id);

                    if (existingArticle == null) return NotFound();

                    // Update scalar properties
                    existingArticle.NewsTitle = article.NewsTitle;
                    existingArticle.Headline = article.Headline;
                    existingArticle.NewsContent = article.NewsContent;
                    existingArticle.NewsSource = article.NewsSource;
                    existingArticle.CategoryId = article.CategoryId;
                    existingArticle.NewsStatus = article.NewsStatus;
                    existingArticle.ModifiedDate = DateTime.Now;
                    existingArticle.UpdatedById = (short?)CurrentAccountId;

                    // Update tags
                    existingArticle.Tags.Clear();

                    if (selectedTags != null)
                    {
                        var selectedTagsList = await _context.Tags
                            .Where(t => selectedTags.Contains(t.TagId))
                            .ToListAsync();

                        foreach (var tag in selectedTagsList)
                        {
                            existingArticle.Tags.Add(tag);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsArticleExists(article.NewsArticleId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(NewsArticles));
            }
            ViewBag.Categories = _context.Categories.Where(c => c.IsActive == true).ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View(article);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteNewsArticle(string id)
        {
            if (id == null) return NotFound();

            var article = await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id && n.CreatedById == CurrentAccountId);

            if (article == null) return NotFound();

            return View(article);
        }

        [HttpPost, ActionName("DeleteNewsArticle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNewsArticleConfirmed(string id)
        {
            var article = await _context.NewsArticles
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id && n.CreatedById == CurrentAccountId);

            if (article != null)
            {
                // EF Core will automatically handle the many-to-many relationship deletion
                _context.NewsArticles.Remove(article);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(NewsArticles));
        }

        private bool NewsArticleExists(string id)
        {
            return _context.NewsArticles.Any(e => e.NewsArticleId == id);
        }
    }
}