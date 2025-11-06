using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies;
using Movies.Models;
using Movies.Utilities;

namespace TaskFilmInCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}, {SD.EMPLOYEE_ROLE}")]
    public class DashController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var stats = new
            {
                MoviesCount = _context.Movies.Count(),
                ActorsCount = _context.Actors.Count(),
                CategoriesCount = _context.Categories.Count(),
                CinemasCount = _context.Directors.Count()
            };

            return View(stats);
        }
    }
}
