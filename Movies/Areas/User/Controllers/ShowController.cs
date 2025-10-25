using Microsoft.AspNetCore.Mvc;

namespace Movies.Areas.User.Controllers
{
    public class ShowController : Controller
    {
        [Area("User")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
