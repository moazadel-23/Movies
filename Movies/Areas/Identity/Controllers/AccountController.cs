using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Movies.Models;
using Movies.Repositories.IRepository;
using Movies.Utilities;
using Movies.ViewModel;
using System.Threading;
using System.Threading.Tasks;


namespace Movies.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOtpRepository;

        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, IRepository<ApplicationUserOTP> ApplicationUserOtpRepository)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _applicationUserOtpRepository = ApplicationUserOtpRepository;
        }
   

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);

            var user = new ApplicationUser()
            {
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                Email = registerVM.Email,
                UserName = registerVM.UserName,
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Code);
                }

                return View(registerVM);
            }

            // Send Confirmation Mail
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", token, userId = user.Id }, Request.Scheme);

            await _emailSender.SendEmailAsync(registerVM.Email, "Ecommerce 519 - Confirm Your Email!"
                , $"<h1>Confirm Your Email By Clicking <a href='{link}'>Here</a></h1>");

            await _userManager.AddToRoleAsync(user, SD.CUSTOMER_ROLE);

            return RedirectToAction("Login");
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user =await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["error-notification"] = "Invalid User Cred.";
            }

            var result =await _userManager.ConfirmEmailAsync(user!, token);

            if (!result.Succeeded)
            {
                TempData["error-notification"] = "Invalid in Token.";
            }
            else
            {
                TempData["success-notification"] = "Confirm Email Successfully.";
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if(!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = await _userManager.FindByNameAsync(loginVM.EmailOrUser) ??
                await _userManager.FindByEmailAsync(loginVM.EmailOrUser);
            if(user is null)
            {
                ModelState.AddModelError(string.Empty, "User or Email Invalid");
                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe ,lockoutOnFailure: true);

            if(!result.Succeeded)          
            {
                if(result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "wait 5 min");
                }
                if(!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "please enter your email");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid User Name / Email OR Password");
                }
                return View(loginVM);
            }
            return RedirectToAction("Index", "Show", new {area = "User" });
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ResentConfirmEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResentConfirmEmail(ResentConfirmEmailVM resentConfirmEmailVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resentConfirmEmailVM);
            }

            var user = await _userManager.FindByNameAsync(resentConfirmEmailVM.UserNameOrEmail) ??
                await _userManager.FindByEmailAsync(resentConfirmEmailVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid UserName or Email");
                return View(resentConfirmEmailVM);
            }
            if (user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Email is already confirmed");
                return View(resentConfirmEmailVM);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email!, "Ecommerce 519 - Confirm Your Email!"
                , $"<h1>Confirm Your Email By Clicking <a href='{link}'>Here</a></h1>");

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(forgetPasswordVM);

            var user = await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name / Email");
                return View(forgetPasswordVM);
            }

            var userOTPs = await _applicationUserOtpRepository.GetAsync(e => e.ApplicationUserId == user.Id);

            var totalOTPs = userOTPs.Count(e => (DateTime.UtcNow - e.CreateAt).TotalHours < 24);

            if (totalOTPs > 3)
            {
                ModelState.AddModelError(string.Empty, "Too Many Attemps");
                return View(forgetPasswordVM);
            }

            var otp = new Random().Next(1000, 9999).ToString();

            await _applicationUserOtpRepository.AddAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationUserId = user.Id,
                CreateAt = DateTime.UtcNow,
                IsValid = true,
                OTP = otp,
                ValidTo = DateTime.UtcNow.AddDays(1),
            });
            await _applicationUserOtpRepository.CommitAsync();

            await _emailSender.SendEmailAsync(user.Email!, "Ecommerce 519 - Reset Your Password"
                , $"<h1>Use This OTP: {otp} To Reset Your Account. Don't share it.</h1>");

            return RedirectToAction("ValidateOTP", new { userId = user.Id });
        }

        public IActionResult ValidateOTP(string userId)
        {
            return View(new ValidateOTPVM
            {
                ApplicationUserId = userId
            });
        }
        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            var result = await _applicationUserOtpRepository.GetOneAsync(e => e.ApplicationUserId == validateOTPVM.ApplicationUserId && e.OTP == validateOTPVM.OTP && e.IsValid);

            if (result is null)
            {
                return RedirectToAction(nameof(ValidateOTP), new { userId = validateOTPVM.ApplicationUserId });
            }

            return RedirectToAction("NewPassword", new { userId = validateOTPVM.ApplicationUserId });
        }
        public IActionResult NewPassword(string userId)
        {
            return View(new NewPasswordVM
            {
                ApplicationUserId = userId
            });
        }

        [HttpPost]
        public async Task<IActionResult> NewPassword(NewPasswordVM newPasswordVM)
        {
            var user = await _userManager.FindByIdAsync(newPasswordVM.ApplicationUserId);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name / Email");
                return View(newPasswordVM);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, newPasswordVM.Password);


            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Code);
                }

                return View(newPasswordVM);
            }

            return RedirectToAction("Login");
        }

    }
}
