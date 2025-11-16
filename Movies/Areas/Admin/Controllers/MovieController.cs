using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Models;
using Movies.Repositories.IRepository;
using Movies.Utilities;
using System.Threading.Tasks;

namespace Movies.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}, {SD.EMPLOYEE_ROLE}")]
    public class MovieController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<MovieActor> _movieActorRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Cinema> _directorRepository;
        private readonly IRepository<Actor> _actorRepository;

        public MovieController(
            IRepository<Movie> movieRepository,
            IRepository<MovieActor> movieActorRepository,
            IRepository<Category> categoryRepository,
            IRepository<Cinema> directorRepository,
            IRepository<Actor> actorRepository)
        {
            _movieRepository = movieRepository;
            _movieActorRepository = movieActorRepository;
            _categoryRepository = categoryRepository;
            _directorRepository = directorRepository;
            _actorRepository = actorRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var movies = await _movieRepository.GetAsync(
                include: [e => e.Category, e => e.Cinema, e => e.MovieActors],
                cancellationToken: cancellationToken);

            return View(movies);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            ViewBag.Categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Directors = await _directorRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Actors = await _actorRepository.GetAsync(cancellationToken: cancellationToken);

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(
            Movie movie,
            IFormFile MainImgFile,
            IFormFileCollection SubImgFiles,
            int CategoryId,
            int CinemaId,
            string SelectedActors,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
                ViewBag.Directors = await _directorRepository.GetAsync(cancellationToken: cancellationToken);
                ViewBag.Actors = await _actorRepository.GetAsync(cancellationToken: cancellationToken);
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
                await MainImgFile.CopyToAsync(stream);
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
                    await file.CopyToAsync(stream);
                    movie.SubImg += fileName + ";";
                }
            }

            await _movieRepository.AddAsync(movie, cancellationToken);
            await _movieRepository.CommitAsync(cancellationToken);


            if (!string.IsNullOrEmpty(SelectedActors))
            {
                var actorIds = SelectedActors.Split(',').Select(int.Parse);
                foreach (var id in actorIds)
                {
                    await _movieActorRepository.AddAsync(new MovieActor
                    {
                        Mov_Id = movie.Mov_Id,
                        Act_Id = id
                    }, cancellationToken);
                }
                await _movieRepository.CommitAsync(cancellationToken);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetOneAsync(
                e => e.Mov_Id == id,
                include: [e => e.Category, e => e.Cinema, e => e.MovieActors],
                cancellationToken: cancellationToken
            );

            if (movie == null)
                return NotFound();

            // جلب البيانات مع fallback لقائمة فارغة
            ViewBag.Categories = await _categoryRepository.GetAsync(cancellationToken : cancellationToken) ?? new List<Category>();
            ViewBag.Directors = await _directorRepository.GetAsync(cancellationToken: cancellationToken) ?? new List<Cinema>();
            ViewBag.Actors = await _actorRepository.GetAsync(cancellationToken: cancellationToken) ?? new List<Actor>();

            // قائمة الممثلين المختارين مسبقًا
            ViewBag.SelectedActors = movie.MovieActors?.Select(ma => ma.Act_Id).ToList() ?? new List<int>();

            return View(movie);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(
            int id,
            Movie movie,
            int CategoryId,
            int CinemaId,
            string? SelectedActors,
            CancellationToken cancellationToken)
        {
            var oldMovie = await _movieRepository.GetOneAsync(
                e => e.Mov_Id == id,
                include: [e => e.MovieActors],
                cancellationToken: cancellationToken
            );

            if (oldMovie == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken) ?? new List<Category>();
                ViewBag.Directors = await _directorRepository.GetAsync(cancellationToken: cancellationToken) ?? new List<Cinema>();
                ViewBag.Actors = await _actorRepository.GetAsync(cancellationToken: cancellationToken) ?? new List<Actor>();
                ViewBag.SelectedActors = oldMovie.MovieActors?.Select(ma => ma.Act_Id).ToList() ?? new List<int>();
                return View(movie);
            }

            oldMovie.Name = movie.Name;
            oldMovie.Description = movie.Description;
            oldMovie.Price = movie.Price;
            oldMovie.Date = movie.Date;
            oldMovie.CategoryId = CategoryId;
            oldMovie.CinemaId = CinemaId;

            // حذف الممثلين القدامى
            var allOldActors = oldMovie.MovieActors.ToList();
            foreach (var oldActor in allOldActors)
                _movieActorRepository.Delete(oldActor);

            // إضافة الممثلين الجدد
            if (!string.IsNullOrEmpty(SelectedActors))
            {
                var actorIds = SelectedActors.Split(',').Select(int.Parse);
                foreach (var idAct in actorIds)
                {
                    await _movieActorRepository.AddAsync(new MovieActor
                    {
                        Mov_Id = oldMovie.Mov_Id,
                        Act_Id = idAct
                    }, cancellationToken);
                }
            }

            _movieRepository.Update(oldMovie);
            await _movieRepository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Deletes(int id, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetOneAsync(e => e.Mov_Id == id, cancellationToken: cancellationToken);
            if (movie == null)
                return NotFound();

            // احذف الصور من المجلد
            if (!string.IsNullOrEmpty(movie.MainImg))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\MovieImages", movie.MainImg);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            if (!string.IsNullOrEmpty(movie.SubImg))
            {
                var imgs = movie.SubImg.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var img in imgs)
                {
                    var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\MovieImages", img);
                    if (System.IO.File.Exists(imgPath))
                        System.IO.File.Delete(imgPath);
                }
            }

            _movieRepository.Delete(movie);
            await _movieRepository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }



    }
}


