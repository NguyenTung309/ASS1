using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASS1.Data;
using ASS1.Models;

namespace ASS1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly FUNewsContext _context;

        public AdminController(FUNewsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Account CRUD
        public async Task<IActionResult> Accounts()
        {
            var accounts = await _context.SystemAccounts.ToListAsync();
            return View(accounts);
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(SystemAccount account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Accounts));
            }
            return View(account);
        }

        [HttpGet]
        public async Task<IActionResult> EditAccount(short? id)
        {
            if (id == null) return NotFound();

            var account = await _context.SystemAccounts.FindAsync(id);
            if (account == null) return NotFound();

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAccount(short id, SystemAccount account)
        {
            if (id != account.AccountId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Accounts));
            }
            return View(account);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAccount(short? id)
        {
            if (id == null) return NotFound();

            var account = await _context.SystemAccounts.FindAsync(id);
            if (account == null) return NotFound();

            return View(account);
        }

        [HttpPost, ActionName("DeleteAccount")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccountConfirmed(short id)
        {
            var account = await _context.SystemAccounts.FindAsync(id);
            if (account != null)
            {
                _context.SystemAccounts.Remove(account);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Accounts));
        }

        // News Article Management
        public async Task<IActionResult> NewsArticles()
        {
            var articles = await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .ToListAsync();
            return View(articles);
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

            return View(category);
        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategoryConfirmed(short id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Categories));
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

        // Statistics Report
        public async Task<IActionResult> Statistics(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.NewsArticles.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(n => n.CreatedDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(n => n.CreatedDate <= endDate.Value);

            var statistics = await query
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(statistics);
        }

        private bool AccountExists(short id)
        {
            return _context.SystemAccounts.Any(e => e.AccountId == id);
        }

        private bool CategoryExists(short id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.TagId == id);
        }
    }
}