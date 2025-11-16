using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using Movies.Repositories.IRepository;
using Movies.Repository;
using Movies.Utilities;
using Movies.ViewModel;
using System.Threading.Tasks;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}, {SD.EMPLOYEE_ROLE}")]
    public class ActorController : Controller
    {
        private readonly IRepository<Actor> _actorRepository;
        private readonly IWebHostEnvironment _env;

        public ActorController(IRepository<Actor> actorRepository, IWebHostEnvironment env) 
        {
            _actorRepository = actorRepository;
            _env = env;
        }
   

        public async Task<IActionResult> Index()
        {
            var actors =await _actorRepository.GetAsync();
            return View(actors);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var actor = new MovieVM
            {
                Actor =new(),
                SelectedActorIds = new List<int>()
            };
            return View(actor);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile ImgFile, CancellationToken cancellationToken)
        {

            if (ImgFile != null && ImgFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ActorsImg");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgFile.CopyTo(stream);
                }

                actor.Img = fileName;
            }

            await _actorRepository.AddAsync(actor, cancellationToken);
            await _actorRepository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id)
        {
            var actor =await _actorRepository.GetOneAsync(e => e.Act_Id == id);
            if (actor == null)
                return NotFound();

            return View(actor);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(Actor actor, IFormFile? ImgFile, CancellationToken cancellationToken)
        {
            var existingActor = await _actorRepository.GetOneAsync(e => e.Act_Id == actor.Act_Id, cancellationToken: cancellationToken);
            if (existingActor == null)
                return NotFound();

            //update 
            existingActor.Name = actor.Name;
            existingActor.Age = actor.Age;

            if (ImgFile != null && ImgFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ActorsImg");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgFile.CopyTo(stream);
                }

                existingActor.Img = fileName;
            }

            await _actorRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Act_Id == id, cancellationToken: cancellationToken);
            if (actor == null)
                return NotFound();

            if (!string.IsNullOrEmpty(actor.Img))
            {
                var filePath = Path.Combine(_env.WebRootPath, "ActorsImg", actor.Img);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }


            _actorRepository.Delete(actor); 
            await _actorRepository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }
    }
}
