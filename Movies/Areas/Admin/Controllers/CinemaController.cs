using Microsoft.AspNetCore.Mvc;
using Movies.Models;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CinemaController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var Directors = _context.Directors.ToList();
            return View(Directors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Cinema cinema, IFormFile ImgFile)
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

            _context.Directors.Add(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cinema = _context.Directors.FirstOrDefault(c => c.Cin_Id == id);
            if (cinema == null)
                return RedirectToAction("NotFoundPage", "Home");
            return View(cinema);
        }
   
        [HttpPost]
        public IActionResult Edit(Cinema cinema, IFormFile ImgFile)
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

            _context.Directors.Update(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            var cinema = _context.Directors.FirstOrDefault(c => c.Cin_Id == id);
            if (cinema == null) return RedirectToAction("NotFoundPage", "Home");

            _context.Directors.Remove(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
