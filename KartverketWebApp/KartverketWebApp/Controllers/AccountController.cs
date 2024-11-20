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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Finn brukeren i databasen
                var bruker = _context.Bruker.FirstOrDefault(b => b.Email == model.Username);
                if (bruker == null)
                {
                    ModelState.AddModelError(string.Empty, "Feil e-postadresse eller passord.");
                    return View(model);
                }

                // Start validering av passord
                var isPasswordValid = false;

                // Prøv å validere som hashet passord
                if (!string.IsNullOrEmpty(bruker.Passord))
                {
                    try
                    {
                        var hashedValidationResult = _passwordHasher.VerifyHashedPassword(null, bruker.Passord, model.Password);
                        if (hashedValidationResult == PasswordVerificationResult.Success)
                        {
                            isPasswordValid = true;
                        }
                    }
                    catch
                    {
                        // Ikke en hashet passord - fortsett til uhashet sjekk
                    }
                }

                // Hvis hashet validering feiler, prøv uhashet validering
                if (!isPasswordValid && bruker.Passord == model.Password)
                {
                    isPasswordValid = true;
                }

                if (!isPasswordValid)
                {
                    ModelState.AddModelError(string.Empty, "Feil e-postadresse eller passord.");
                    return View(model);
                }

                // Opprett claims for brukeren
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, bruker.Email),
                    new Claim("BrukerType", bruker.BrukerType),
                    new Claim("BrukerId", bruker.BrukerId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, "AuthCookie");

                // Sett cookie for autentisering
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                };

                await HttpContext.SignInAsync("AuthCookie", new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Feil under innlogging: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Det oppstod en feil under innloggingen. Prøv igjen senere.");
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

            // Opprett IdentityUser
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Hash passordet
                var hashedPassword = _passwordHasher.HashPassword(user, model.Password);

                // Opprett Bruker-objektet
                var bruker = new Bruker
                {
                    Email = model.Email,
                    Passord = hashedPassword,
                    BrukerType = "Standard",
                    IdentityUserId = user.Id // Knytter til IdentityUser
                };

                // Legg til Bruker i databasen
                _context.Bruker.Add(bruker);
                await _context.SaveChangesAsync(); // Spar databasen for å generere BrukerId

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

                return RedirectToAction("Index", "Home");
            }

            // Håndter feil
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
