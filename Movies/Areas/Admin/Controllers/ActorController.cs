using Microsoft.AspNetCore.Mvc;
using Movies.Models;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ActorController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var actors = _context.Actors.ToList();
            return View(actors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new ViewModel
            {
                Actor = _context.Actors.ToList(),
                SelectedActorIds = new List<int>()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(Actor actor, IFormFile ImgFile)
        {
            if (ImgFile != null && ImgFile.Length > 0)
            {
                var folderPath = Path.Combine(_env.WebRootPath, "ActorsImg");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImgFile.CopyTo(stream);
                }

                actor.Img = "ActorsImg/" + fileName; 
            }

            _context.Actors.Add(actor);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var actor = _context.Actors.Find(id);
            if (actor == null)
                return NotFound();

            return View(actor);
        }

        [HttpPost]
        public IActionResult Edit(Actor actor, IFormFile? ImgFile)
        {
            var existingActor = _context.Actors.Find(actor.Act_Id);
            if (existingActor == null)
                return NotFound();

           


            if (ImgFile != null && ImgFile.Length > 0)
            {
                var folderPath = Path.Combine(_env.WebRootPath, "ActorsImg");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }


                if (!string.IsNullOrEmpty(existingActor.Img))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, existingActor.Img);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }


                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImgFile.CopyTo(stream);
                }

                existingActor.Img = "ActorsImg/" + fileName;
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.FirstOrDefault(a => a.Act_Id == id);
            if (actor == null) return NotFound();

            if (!string.IsNullOrEmpty(actor.Img))
            {
                var filePath = Path.Combine(_env.WebRootPath, actor.Img);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Actors.Remove(actor);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
