using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASS1.Data;
using ASS1.Models;

namespace ASS1.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : BaseController
    {
        private readonly FUNewsContext _context;

        public StaffController(FUNewsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Category Management
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(short? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(short id, Category category)
        {
            if (id != category.CategoryId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(short? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            // Check if category is used in any news article
            var isUsed = await _context.NewsArticles.AnyAsync(n => n.CategoryId == id);
            ViewBag.CanDelete = !isUsed;

            return View(category);
        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategoryConfirmed(short id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                // Check if category is used
                var isUsed = await _context.NewsArticles.AnyAsync(n => n.CategoryId == id);
                if (!isUsed)
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    TempData["ErrorMessage"] = "Cannot delete category because it is used in news articles.";
                    return RedirectToAction(nameof(DeleteCategory), new { id });
                }
            }
            return RedirectToAction(nameof(Categories));
        }

        // News Article Management
        public async Task<IActionResult> NewsArticles()
        {
            var articles = await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Tags)
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

                // Add tags
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
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id && n.CreatedById == CurrentAccountId);

            if (article == null) return NotFound();

            ViewBag.Categories = _context.Categories.Where(c => c.IsActive == true).ToList();
            ViewBag.Tags = _context.Tags.ToList();

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

                    // Update properties
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
                _context.NewsArticles.Remove(article);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(NewsArticles));
        }

        // Tag Management
        public async Task<IActionResult> Tags()
        {
            var tags = await _context.Tags.ToListAsync();
            return View(tags);
        }

        [HttpGet]
        public IActionResult CreateTag()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTag(Tag tag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Tags));
            }
            return View(tag);
        }

        [HttpGet]
        public async Task<IActionResult> EditTag(int? id)
        {
            if (id == null) return NotFound();

            var tag = await _context.Tags.FindAsync(id);
            if (tag == null) return NotFound();

            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTag(int id, Tag tag)
        {
            if (id != tag.TagId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(tag.TagId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Tags));
            }
            return View(tag);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteTag(int? id)
        {
            if (id == null) return NotFound();

            var tag = await _context.Tags.FindAsync(id);
            if (tag == null) return NotFound();

            return View(tag);
        }

        [HttpPost, ActionName("DeleteTag")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTagConfirmed(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Tags));
        }

        // Profile Management
        public async Task<IActionResult> Profile()
        {
            var account = await _context.SystemAccounts.FindAsync(CurrentAccountId);
            if (account == null) return NotFound();

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(SystemAccount account)
        {
            if (CurrentAccountId != account.AccountId) return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Profile updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Profile));
            }
            return View(account);
        }

        private bool CategoryExists(short id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

        private bool NewsArticleExists(string id)
        {
            return _context.NewsArticles.Any(e => e.NewsArticleId == id);
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.TagId == id);
        }

        private bool AccountExists(short id)
        {
            return _context.SystemAccounts.Any(e => e.AccountId == id);
        }
    }
}