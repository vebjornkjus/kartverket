using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using KartverketWebApp.Data; // Inkluder namespace for ApplicationDbContext
using KartverketWebApp.Models; // Inkluder namespace for Bruker-modellen

namespace KartverketWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<IdentityUser> _passwordHasher;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IPasswordHasher<IdentityUser> passwordHasher) // Initialiser passwordHasher
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _passwordHasher = passwordHasher; // Initialiser passwordHasher
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account locked out due to multiple failed attempts.");
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Login is not allowed for this account.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your username and password.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                var bruker = new Bruker
                {
                    Email = model.Email,
                    BrukerType = "Standard",
                    IdentityUserId = user.Id
                };

                _context?.Bruker.Add(bruker);
                await _context?.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
