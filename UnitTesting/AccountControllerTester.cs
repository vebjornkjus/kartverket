using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Xunit;
using Moq;
using KartverketWebApp.Data;
using KartverketWebApp.Models;
using KartverketWebApp.Controllers;

namespace KartverketWebApp.Tests
{
    public class AccountControllerTests
    {
        // Mock-objekter og dependencies som brukes på tvers av testene
        private readonly Mock<IPasswordHasher<IdentityUser>> _mockPasswordHasher;  // Simulerer passordhashing uten faktisk kryptering
        private readonly Mock<IAuthenticationService> _mockAuthService;            // Simulerer autentiseringstjenester (innlogging/utlogging)
        private readonly AccountController _controller;                            // Controller-instansen som testes
        private readonly ApplicationDbContext _context;                           // In-memory database for testing

        public AccountControllerTests()
        {
            // Oppsett av in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Initialiserer mock-objekter
            _mockPasswordHasher = new Mock<IPasswordHasher<IdentityUser>>();
            _mockAuthService = new Mock<IAuthenticationService>();

            // Konfigurerer dependency injection for testene
            var services = new ServiceCollection();
            services.AddScoped(_ => _mockAuthService.Object);  // Registrerer mock authentication service
            services.AddControllersWithViews();                // Legger til MVC-tjenester

            var serviceProvider = services.BuildServiceProvider();

            // Setter opp HTTP-kontekst for testing
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };

            // Initialiserer controller med nødvendige dependencies og kontekst
            _controller = new AccountController(_context, _mockPasswordHasher.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            // Konfigurerer URL-helper for controller
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                        .Returns("Home/Index");
            _controller.Url = mockUrlHelper.Object;
        }

        [Fact]
        public void Login_Get_ReturnsView()
        {
            // Test at GET-request til login returnerer korrekt view
            var result = _controller.Login();
            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [InlineData("invalid-email", "Password123")]    // Tester ugyldig e-postformat
        [InlineData("", "Password123")]                 // Tester tom e-post
        [InlineData("test@example.com", "")]           // Tester tomt passord
        public async Task Login_Post_InvalidModel_ReturnsViewWithErrors(string username, string password)
        {
            // Arrange: Setter opp ugyldig login-modell
            var model = new LoginViewModel { Username = username, Password = password };
            _controller.ModelState.AddModelError("", "Test error");

            // Act: Forsøker innlogging med ugyldig modell
            var result = await _controller.Login(model);

            // Assert: Verifiserer at feil håndteres korrekt
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Login_Post_ValidCredentials_RedirectsToHome()
        {
            // Arrange: Setter opp gyldig bruker og innloggingsdata
            var model = new LoginViewModel
            {
                Username = "test@example.com",
                Password = "Password123",
                RememberMe = true
            };

            var bruker = new Bruker
            {
                Email = "test@example.com",
                Passord = "hashedPassword",
                BrukerType = "Standard"
            };

            var person = new Person
            {
                Fornavn = "Test",
                Etternavn = "Testesen",
                BrukerId = bruker.BrukerId
            };

            // Legger til testdata i databasen
            await _context.Bruker.AddAsync(bruker);
            await _context.Person.AddAsync(person);
            await _context.SaveChangesAsync();

            // Konfigurerer mock-oppførsel for passordverifisering
            _mockPasswordHasher
                .Setup(x => x.VerifyHashedPassword(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(PasswordVerificationResult.Success);

            // Konfigurerer mock-oppførsel for innlogging
            _mockAuthService
                .Setup(x => x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            // Act: Utfører innlogging
            var result = await _controller.Login(model);

            // Assert: Verifiserer redirect til hjemmeside
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Register_Post_ValidModel_CreatesUserAndRedirects()
        {
            // Arrange: Setter opp gyldig registreringsmodell
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password123",
                Fornavn = "Test",
                Etternavn = "Testesen"
            };

            // Konfigurerer mock-oppførsel for passordhashing
            _mockPasswordHasher
                .Setup(x => x.HashPassword(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns("hashedPassword");

            // Konfigurerer mock-oppførsel for automatisk innlogging etter registrering
            _mockAuthService
                .Setup(x => x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            // Act: Utfører registrering
            var result = await _controller.Register(model);

            // Assert: Verifiserer brukeropprettelse og redirect
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);

            // Verifiserer at bruker og person ble opprettet i databasen
            Assert.Single(await _context.Bruker.ToListAsync());
            Assert.Single(await _context.Person.ToListAsync());
        }

        [Theory]
        [InlineData("short", false)]                // Tester for kort passord
        [InlineData("onlysmall123", false)]        // Tester passord uten stor bokstav
        [InlineData("ONLYBIG123", false)]          // Tester passord uten liten bokstav
        [InlineData("OnlyLetters", false)]         // Tester passord uten tall
        [InlineData("ValidPass123", true)]         // Tester gyldig passord
        public void ErPassordGyldig_ValidatesCorrectly(string password, bool expectedResult)
        {
            // Henter private metode via reflection for testing
            var methodInfo = typeof(AccountController).GetMethod("ErPassordGyldig",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Utfører passordvalidering
            var result = (bool)methodInfo.Invoke(_controller, new object[] { password });

            // Verifiserer resultat
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("test@example.com", true)]     // Tester gyldig e-postformat
        [InlineData("invalid-email", false)]       // Tester ugyldig e-postformat
        [InlineData("", false)]                    // Tester tom e-post
        [InlineData("test@example", false)]        // Tester ufullstendig e-postformat
        public void ErEpostGyldig_ValidatesCorrectly(string email, bool expectedResult)
        {
            // Henter private metode via reflection for testing
            var methodInfo = typeof(AccountController).GetMethod("ErEpostGyldig",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Utfører e-postvalidering
            var result = (bool)methodInfo.Invoke(_controller, new object[] { email });

            // Verifiserer resultat
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task Logout_SignsOutAndRedirectsToHome()
        {
            // Konfigurerer mock-oppførsel for utlogging
            _mockAuthService
                .Setup(x => x.SignOutAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            // Utfører utlogging
            var result = await _controller.Logout();

            // Verifiserer at utlogging ble utført
            _mockAuthService.Verify(
                x => x.SignOutAsync(
                    It.IsAny<HttpContext>(),
                    "AuthCookie",
                    It.IsAny<AuthenticationProperties>()
                ),
                Times.Once
            );

            // Verifiserer redirect til hjemmeside
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        public void Dispose()
        {
            // Rydder opp testmiljøet og sletter in-memory database
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}