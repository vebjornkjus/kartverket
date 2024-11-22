using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using KartverketWebApp.Data; // Inkluder namespace for ApplicationDbContext
using KartverketWebApp.Models; // Inkluder namespace for Bruker-modellen
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Sjekk e-post i Bruker-tabellen
            var bruker = _context.Bruker.FirstOrDefault(b => b.Email == model.Username);
            if (bruker == null)
            {
                ModelState.AddModelError(string.Empty, "Feil e-postadresse eller passord.");
                return View(model);
            }

            // Verifiser passord
            var isPasswordValid = false;
            if (bruker.Passord == model.Password) // Sjekk klartekst (kun for testing)
            {
                isPasswordValid = true;
            }
            else
            {
                var hashedValidationResult = _passwordHasher.VerifyHashedPassword(null, bruker.Passord, model.Password);
                if (hashedValidationResult == PasswordVerificationResult.Success)
                {
                    isPasswordValid = true;
                }
            }

            if (!isPasswordValid)
            {
                ModelState.AddModelError(string.Empty, "Feil e-postadresse eller passord.");
                return View(model);
            }

            // Hent fornavn fra Person-tabellen
            var person = _context.Person.FirstOrDefault(p => p.BrukerId == bruker.BrukerId);

            // Legg til e-post og fornavn i claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, bruker.Email), // Bruk e-post som identifikator
                new Claim("Fornavn", person?.Fornavn ?? "Ukjent"), // Legg til fornavnet
                new Claim("BrukerType", bruker.BrukerType) // Valgfritt: Brukerrollen
            };

            var claimsIdentity = new ClaimsIdentity(claims, "AuthCookie");

            // Sett opp autentisering med claims
            await HttpContext.SignInAsync("AuthCookie", new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTime.UtcNow.AddHours(1)
            });

            return RedirectToAction("Index", "Home");
        }


      [HttpGet]
       public IActionResult Register() => View();

      [HttpPost]
      [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Opprett IdentityUser
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Logg inn brukeren etter vellykket registrering
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Hash passordet
                var hashedPassword = _passwordHasher.HashPassword(user, model.Password);

                // Opprett Bruker-objektet
                var bruker = new Bruker
                {
                    Email = model.Email,
                    Passord = hashedPassword,
                    BrukerType = "Standard", // Standard brukerrolle
                    IdentityUserId = user.Id // Knytter IdentityUser med Bruker
                };

                // Legg til Bruker i databasen
                _context.Bruker.Add(bruker);
                await _context.SaveChangesAsync();

                // Opprett Person-objektet
                var person = new Person
                {
                    Fornavn = model.Fornavn,
                    Etternavn = model.Etternavn,
                    BrukerId = bruker.BrukerId // Referanse til Bruker
                };

                // Legg til Person i databasen
                _context.Person.Add(person);
                await _context.SaveChangesAsync();

                // Videresend til en velkomstside eller hjem
                return RedirectToAction("Login", "Account");
            }

            // Håndter feil under oppretting av brukeren
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AuthCookie");
            return RedirectToAction("Index", "Home");
        }
    }
}
