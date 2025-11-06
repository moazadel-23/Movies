using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using Movies.Repositories.IRepository;
using Movies.Repository;
using Movies.Utilities;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}, {SD.EMPLOYEE_ROLE}")]
    public class CinemaController : Controller
    {

        private readonly IRepository<Cinema> _cinemaRepository;
        public CinemaController(IRepository<Cinema> cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var Directors =await _cinemaRepository.GetAsync(cancellationToken: cancellationToken);
            return View(Directors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile ImgFile, CancellationToken cancellationToken)
        {
            if (ImgFile != null && ImgFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\CinemaImage");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgFile.CopyTo(stream);
                }

                cinema.Img = fileName;
            }

            await _cinemaRepository.AddAsync(cinema, cancellationToken);
            await _cinemaRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var cinema =await _cinemaRepository.GetOneAsync(c => c.Cin_Id == id, cancellationToken: cancellationToken);
            if (cinema == null)
                return RedirectToAction("NotFoundPage", "Home");
            return View(cinema);
        }
   
        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile ImgFile, CancellationToken cancellationToken)
        {
            if (ImgFile != null && ImgFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\CinemaImage");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgFile.CopyTo(stream);
                }

                cinema.Img = fileName;
            }

            _cinemaRepository.Update(cinema);
            await _cinemaRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}, {SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Cin_Id == id, cancellationToken: cancellationToken);
            if (cinema == null)
                return RedirectToAction("NotFoundPage", "Home");

            _cinemaRepository.Delete(cinema);
            await _cinemaRepository.CommitAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

    }
}
