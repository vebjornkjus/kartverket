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
        private readonly ApplicationDbContext _context; // Legg til ApplicationDbContext
        private readonly IPasswordHasher<IdentityUser> _passwordHasher; // Legg til PasswordHasher

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
        public async Task<IActionResult> Login(string username, string password, bool rememberMe = false)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, rememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Hash passordet
                var hashedPassword = _passwordHasher.HashPassword(user, password);

                // Opprett ny post i Bruker-tabellen
                var bruker = new Bruker
                {
                    Brukernavn = email,
                    Passord = hashedPassword, // Lagre hashet passord
                    BrukerType = "Standard", // Angi brukerens type
                    IdentityUserId = user.Id // Fremmednøkkel til AspNetUsers
                };

                _context.Bruker.Add(bruker);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }
    }
}
