using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly Mock<IPasswordHasher<IdentityUser>> _mockPasswordHasher;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly AccountController _controller;
        private readonly ApplicationDbContext _context;

        public AccountControllerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _mockPasswordHasher = new Mock<IPasswordHasher<IdentityUser>>();
            _mockAuthService = new Mock<IAuthenticationService>();

            // Setup service collection
            var services = new ServiceCollection();
            services.AddScoped(_ => _mockAuthService.Object);
            services.AddControllersWithViews();

            // Build service provider
            var serviceProvider = services.BuildServiceProvider();

            // Setup HttpContext
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };

            // Setup controller context
            _controller = new AccountController(_context, _mockPasswordHasher.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            // Mock UrlHelper
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns("Home/Index");
            _controller.Url = mockUrlHelper.Object;
        }

        [Fact]
        public void Login_Get_ReturnsView()
        {
            // Act
            var result = _controller.Login();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [InlineData("invalid-email", "Password123")] // Ugyldig e-post
        [InlineData("", "Password123")] // Tom e-post
        [InlineData("test@example.com", "")] // Tomt passord
        public async Task Login_Post_InvalidModel_ReturnsViewWithErrors(string username, string password)
        {
            // Arrange
            var model = new LoginViewModel { Username = username, Password = password };
            _controller.ModelState.AddModelError("", "Test error");

            // Act
            var result = await _controller.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Login_Post_ValidCredentials_RedirectsToHome()
        {
            // Arrange
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

            // Add test data to in-memory database
            await _context.Bruker.AddAsync(bruker);
            await _context.Person.AddAsync(person);
            await _context.SaveChangesAsync();

            _mockPasswordHasher
                .Setup(x => x.VerifyHashedPassword(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(PasswordVerificationResult.Success);

            _mockAuthService
                .Setup(x => x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Login(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Register_Post_ValidModel_CreatesUserAndRedirects()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password123",
                Fornavn = "Test",
                Etternavn = "Testesen"
            };

            _mockPasswordHasher
                .Setup(x => x.HashPassword(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns("hashedPassword");

            _mockAuthService
                .Setup(x => x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Register(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);

            // Verify database changes
            Assert.Single(await _context.Bruker.ToListAsync());
            Assert.Single(await _context.Person.ToListAsync());
        }

        [Theory]
        [InlineData("short", false)] // For kort
        [InlineData("onlysmall123", false)] // Mangler stor bokstav
        [InlineData("ONLYBIG123", false)] // Mangler liten bokstav
        [InlineData("OnlyLetters", false)] // Mangler tall
        [InlineData("ValidPass123", true)] // Gyldig passord
        public void ErPassordGyldig_ValidatesCorrectly(string password, bool expectedResult)
        {
            // Arrange - using private method through reflection
            var methodInfo = typeof(AccountController).GetMethod("ErPassordGyldig",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = (bool)methodInfo.Invoke(_controller, new object[] { password });

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("test@example.com", true)]
        [InlineData("invalid-email", false)]
        [InlineData("", false)]
        [InlineData("test@example", false)]
        public void ErEpostGyldig_ValidatesCorrectly(string email, bool expectedResult)
        {
            // Arrange - using private method through reflection
            var methodInfo = typeof(AccountController).GetMethod("ErEpostGyldig",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = (bool)methodInfo.Invoke(_controller, new object[] { email });

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task Logout_SignsOutAndRedirectsToHome()
        {
            // Arrange
            _mockAuthService
                .Setup(x => x.SignOutAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            _mockAuthService.Verify(
                x => x.SignOutAsync(
                    It.IsAny<HttpContext>(),
                    "AuthCookie",
                    It.IsAny<AuthenticationProperties>()
                ),
                Times.Once
            );

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}