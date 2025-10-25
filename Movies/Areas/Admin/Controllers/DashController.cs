using Microsoft.AspNetCore.Mvc;
using Movies;
using Movies.Models;

namespace TaskFilmInCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
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
