using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using Movies.Repositories.IRepository;

namespace Movies.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ShowController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;

        public ShowController(IRepository<Movie> movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var movies = await _movieRepository.GetAsync(
                include: [e => e.Category!, e => e.Cinema!],
                cancellationToken: cancellationToken);

            return View(movies);
        }

        public async Task<IActionResult> Detail(int id, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetOneAsync(
                e => e.Mov_Id == id,
                include: [e => e.Category!, e => e.Cinema!, e => e.MovieActors],
                cancellationToken: cancellationToken);

            if (movie == null)
                return NotFound();

            return View(movie);
        }
    }
}
