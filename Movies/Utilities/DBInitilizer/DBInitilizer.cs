using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies.Models;

namespace Movies.Utilities.DBInitilizer
{
    public class DBInitilizer : IDBInitilizer
    {
       private readonly ApplicationDbContext _context;
        private readonly ILogger<DBInitilizer> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DBInitilizer(ApplicationDbContext context, ILogger<DBInitilizer> logger, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            Console.WriteLine($"_context: {_context != null}");
            Console.WriteLine($"_roleManager: {_roleManager != null}");
            Console.WriteLine($"_userManager: {_userManager != null}");
            Console.WriteLine($"_logger: {_logger != null}");
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }

                if (!_roleManager.Roles.Any())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.SUPER_ADMIN_ROLE)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.ADMIN_ROLE)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.EMPLOYEE_ROLE)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.CUSTOMER_ROLE)).GetAwaiter().GetResult();

                    _userManager.CreateAsync(new ApplicationUser
                    {
                        Email = "moaza5887@gmail.com",
                        UserName = "SuperAdmin",
                        FirstName = "Moaz",
                        LastName = "Adel",
                        EmailConfirmed = true,
                        PhoneNumber = "01234567890"
                    }, "Moaz123@").GetAwaiter().GetResult();

                    var user = _userManager.FindByNameAsync("SuperAdmin").GetAwaiter().GetResult();
                    if (user != null)
                    {
                        _userManager.AddToRoleAsync(user, SD.SUPER_ADMIN_ROLE).GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred: {ex.Message}");
            }
        }

    }
}
