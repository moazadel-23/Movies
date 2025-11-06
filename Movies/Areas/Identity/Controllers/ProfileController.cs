using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using Movies.ViewModel;

namespace Movies.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // ======= عرض الصفحة =======
        public async Task<IActionResult> UpdateProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var userVM = new ApplicationUserVM()
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Address = user.Address,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ImgPath = user.ImgPath // 👈 نعرض الصورة الحالية
            };

            return View(userVM);
        }

        // ======= تحديث البيانات الشخصية =======
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var names = applicationUserVM.FullName?.Split(" ");

            if (names != null && names.Length >= 1)
                user.FirstName = names[0];
            if (names != null && names.Length >= 2)
                user.LastName = names[1];

            user.PhoneNumber = applicationUserVM.PhoneNumber;
            user.Address = applicationUserVM.Address;

            await _userManager.UpdateAsync(user);

            TempData["success-notification"] = "Profile updated successfully!";
            return RedirectToAction(nameof(UpdateProfile));
        }

        // ======= تحديث كلمة المرور =======
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            if (string.IsNullOrEmpty(applicationUserVM.CurrentPassword) || string.IsNullOrEmpty(applicationUserVM.NewPassword))
            {
                TempData["error-notification"] = "Both Current and New Password are required.";
                return RedirectToAction(nameof(UpdateProfile));
            }

            var result = await _userManager.ChangePasswordAsync(user, applicationUserVM.CurrentPassword, applicationUserVM.NewPassword);

            if (!result.Succeeded)
            {
                TempData["error-notification"] =
                    string.Join(", ", result.Errors.Select(e => e.Description));

                return RedirectToAction(nameof(UpdateProfile));
            }

            TempData["success-notification"] = "Password updated successfully!";
            return RedirectToAction(nameof(UpdateProfile));
        }

        // ======= تحديث الصورة =======
        [HttpPost]
        public async Task<IActionResult> UpdateProfileImage(string ImgPath)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

           user.ImgPath = ImgPath; // نحفظ الاسم فقط مثلاً "user1.png"
            await _userManager.UpdateAsync(user);

            TempData["success-notification"] = "Profile image updated successfully!";
            return RedirectToAction(nameof(UpdateProfile));
        }
    }
}
