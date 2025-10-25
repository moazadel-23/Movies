using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using Microsoft.EntityFrameworkCore;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var movies = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .ToList();

            return View(movies);
        }

      
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Directors = _context.Directors.ToList(); 
            ViewBag.Actors = _context.Actors.ToList();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            Movie movie,
            IFormFile MainImgFile,
            IFormFileCollection SubImgFiles,
            int CategoryId,
            int CinemaId,
            string SelectedActors)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Directors = _context.Directors.ToList();
                ViewBag.Actors = _context.Actors.ToList();
                return View(movie);
            }

         
            movie.CategoryId = CategoryId;
            movie.CinemaId = CinemaId;


            if (MainImgFile != null && MainImgFile.Length > 0)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\MovieImages");
                Directory.CreateDirectory(folder);
                var fileName = Guid.NewGuid() + Path.GetExtension(MainImgFile.FileName);
                var filePath = Path.Combine(folder, fileName);
                using var stream = System.IO.File.Create(filePath);
                MainImgFile.CopyTo(stream);
                movie.MainImg = fileName;
            }


            if (SubImgFiles != null && SubImgFiles.Count > 0)
            {
                movie.SubImg = "";
                foreach (var file in SubImgFiles)
                {
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\MovieImages");
                    Directory.CreateDirectory(folder);
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(folder, fileName);
                    using var stream = System.IO.File.Create(filePath);
                    file.CopyTo(stream);
                    movie.SubImg += fileName + ";";
                }
            }

  
            _context.Movies.Add(movie);
            _context.SaveChanges();

    
            if (!string.IsNullOrEmpty(SelectedActors))
            {
                var actorIds = SelectedActors.Split(',').Select(int.Parse);
                foreach (var id in actorIds)
                {
                    _context.MovieActors.Add(new MovieActor
                    {
                        Mov_Id = movie.Mov_Id,
                        Act_Id = id
                    });
                }
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Movie/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var movie = _context.Movies
                .Include(m => m.MovieActors)
                .FirstOrDefault(m => m.Mov_Id == id);

            if (movie == null)
                return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Directors = _context.Directors.ToList();
            ViewBag.Actors = _context.Actors.ToList();
            ViewBag.SelectedActors = movie.MovieActors.Select(ma => ma.Act_Id).ToList();

            return View(movie);
        }

        // POST: /Admin/Movie/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int id,
            Movie movie,
            IFormFile MainImgFile,
            IFormFileCollection SubImgFiles,
            int CategoryId,
            int CinemaId,
            string SelectedActors)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Directors = _context.Directors.ToList();
                ViewBag.Actors = _context.Actors.ToList();
                return View(movie);
            }

            var movieInDb = _context.Movies.Include(m => m.MovieActors).FirstOrDefault(m => m.Mov_Id == id);
            if (movieInDb == null) return NotFound();

            // Update basic fields
            movieInDb.Name = movie.Name;
            movieInDb.Description = movie.Description;
            movieInDb.Price = movie.Price;
            movieInDb.Date = movie.Date;
            movieInDb.Status = movie.Status;
            movieInDb.CategoryId = CategoryId;
            movieInDb.CinemaId = CinemaId;

            if (MainImgFile != null && MainImgFile.Length > 0)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\MovieImages");
                Directory.CreateDirectory(folder);
                var fileName = Guid.NewGuid() + Path.GetExtension(MainImgFile.FileName);
                var filePath = Path.Combine(folder, fileName);
                using var stream = System.IO.File.Create(filePath);
                MainImgFile.CopyTo(stream);
                movieInDb.MainImg = fileName;
            }

            if (SubImgFiles != null && SubImgFiles.Count > 0)
            {
                movieInDb.SubImg = "";
                foreach (var file in SubImgFiles)
                {
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\MovieImages");
                    Directory.CreateDirectory(folder);
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(folder, fileName);
                    using var stream = System.IO.File.Create(filePath);
                    file.CopyTo(stream);
                    movieInDb.SubImg += fileName + ";";
                }
            }

            // Update actors
            var existingActors = _context.MovieActors.Where(ma => ma.Mov_Id == id);
            _context.MovieActors.RemoveRange(existingActors);

            if (!string.IsNullOrEmpty(SelectedActors))
            {
                var actorIds = SelectedActors.Split(',').Select(int.Parse);
                foreach (var actId in actorIds)
                {
                    _context.MovieActors.Add(new MovieActor { Mov_Id = id, Act_Id = actId });
                }
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Movie/Delete/{id}
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Mov_Id == id);
            if (movie == null) return NotFound();

            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
