using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using Movies.Repositories.IRepository;
using Movies.Repository;
using Movies.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}, {SD.EMPLOYEE_ROLE}")]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;
        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var categories =await _categoryRepository.GetAsync(cancellationToken : cancellationToken);
            return View(categories); 
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
               await _categoryRepository.AddAsync(category, cancellationToken : cancellationToken);
               await _categoryRepository.CommitAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var category =await _categoryRepository.GetOneAsync(e => e.Cat_Id == id, cancellationToken: cancellationToken);
            if (category == null) return NotFound();
            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(Category category, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Update(category);
                await _categoryRepository.CommitAsync(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var category =await _categoryRepository.GetOneAsync(e => e.Cat_Id == id, cancellationToken:cancellationToken);
            if (category == null) return NotFound();

            _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
