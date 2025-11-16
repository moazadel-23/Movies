using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movies.Controllers;
using Movies.Models;
using Movies.Repositories.IRepository;
using Movies.ViewModel;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace Movies.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class CartController : Controller
    {
        public UserManager<ApplicationUser> _userManager { get; }
        public IRepository<Cart> _cartRepository { get; }

        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
        }
        public async Task<IActionResult> Index(string code)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var MovieInDB = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, include: [e => e.Movie, e => e.User]);

            return View(MovieInDB);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int TicketCount, int movieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var productInDb = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.Mov_Id == movieId);

            if (productInDb is not null)
            {
                productInDb.TicketCount += TicketCount;
                await _cartRepository.CommitAsync(cancellationToken);

                TempData["success-notification"] = "Update Product Count to cart successfully";

                return RedirectToAction("Index", "Show");
            }

            await _cartRepository.AddAsync(new()
            {
                Mov_Id = movieId,
                TicketCount = TicketCount,
                ApplicationUserId = user.Id,
                //Price = (await _cartRepository.GetOneAsync(e => e.Mov_Id == movieId)!).Price
            }, cancellationToken: cancellationToken);
            await _cartRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Add Product to cart successfully";

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> InCremantMovie(int movieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var movie = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.Mov_Id == movieId);
            if (movie is not null)
                return NotFound();
            movie!.TicketCount += 1;
            _cartRepository.Update(movie!);
            await _cartRepository.CommitAsync(cancellationToken);
            return RedirectToAction("Index", "cart");
        }

  
        public async Task<IActionResult> DeCremantMovie(int movieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var movie = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.Mov_Id == movieId);
            if (movie is not null)
                return NotFound();

            if(movieId < 1)
            {
                _cartRepository.Delete(movie!);
            }
            else
                movie!.TicketCount -= 1;
            _cartRepository.Update(movie!);
            await _cartRepository.CommitAsync(cancellationToken);
            return RedirectToAction("Index", "cart");
        }

        public async Task<IActionResult> Delete(int movieId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var movie = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.Mov_Id == movieId);
            if (movie is null)
                return NotFound();

            _cartRepository.Delete(movie!);
            await _cartRepository.CommitAsync(cancellationToken);
            return RedirectToAction("Index", "cart");
        }

        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, include: [e => e.Movie]);

            if (cart is null) return NotFound();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/cancel",
            };

            foreach (var item in cart)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Movie.Name,
                            Description = item.Movie.Description,
                        },
                        UnitAmount = (long)item.Movie.Price * 100,
                    },
                    Quantity = item.TicketCount,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }
    }
}
