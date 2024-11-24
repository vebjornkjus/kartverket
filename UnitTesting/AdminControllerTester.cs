using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Xunit;
using Moq;
using KartverketWebApp.Data;
using KartverketWebApp.Models;
using KartverketWebApp.Controllers;

namespace KartverketWebApp.Tests
{
    public class AdminControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IDbConnection> _mockDbConnection;
        private readonly AdminController _controller;
        private readonly IServiceProvider _serviceProvider;

        public AdminControllerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _mockDbConnection = new Mock<IDbConnection>();

            // Setup service collection
            var services = new ServiceCollection();
            services.AddScoped(_ => _mockDbConnection.Object);
            services.AddControllersWithViews();

            // Build service provider
            _serviceProvider = services.BuildServiceProvider();

            // Setup HttpContext
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };

            // Setup controller
            _controller = new AdminController(_context, _mockDbConnection.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            // Mock UrlHelper
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns("Admin/BrukerOversikt");
            _controller.Url = mockUrlHelper.Object;
        }

        [Fact]
        public async Task BrukerOversikt_ReturnsViewResult_WithModel()
        {
            // Arrange
            var testBrukere = new List<(Bruker bruker, Person person)>
            {
                (
                    new Bruker { 
                        BrukerId = 1, 
                        Email = "test1@example.com", 
                        BrukerType = "Bruker",
                        Passord = "TestPassord123" 
                    },
                    new Person { BrukerId = 1, Fornavn = "Ola", Etternavn = "Nordmann" }
                ),
                (
                    new Bruker { 
                        BrukerId = 2, 
                        Email = "test2@example.com", 
                        BrukerType = "Admin",
                        Passord = "TestPassord456" 
                    },
                    new Person { BrukerId = 2, Fornavn = "Kari", Etternavn = "Nordmann" }
                )
            };

            foreach (var (bruker, person) in testBrukere)
            {
                await _context.Bruker.AddAsync(bruker);
                await _context.Person.AddAsync(person);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.BrukerOversikt();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<IEnumerable<BrukerOversiktViewModel>>(viewResult.Model);
            Assert.Equal(testBrukere.Count, model.Count());
            
            // Verify model content
            var modelList = model.ToList();
            for (int i = 0; i < testBrukere.Count; i++)
            {
                Assert.Equal(testBrukere[i].bruker.Email, modelList[i].Email);
                Assert.Equal(testBrukere[i].bruker.BrukerType, modelList[i].BrukerType);
                Assert.Equal(testBrukere[i].person.Fornavn, modelList[i].Fornavn);
                Assert.Equal(testBrukere[i].person.Etternavn, modelList[i].Etternavn);
            }
        }

        [Fact]
        public async Task SlettBruker_ExistingUser_RedirectsToBrukerOversikt()
        {
            // Arrange
            var bruker = new Bruker { 
                BrukerId = 1, 
                Email = "test@example.com", 
                BrukerType = "Bruker",
                Passord = "TestPassord123"
            };
            var person = new Person { BrukerId = 1, Fornavn = "Test", Etternavn = "Testesen" };

            await _context.Bruker.AddAsync(bruker);
            await _context.Person.AddAsync(person);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.SlettBruker(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("BrukerOversikt", redirectResult.ActionName);

            // Verify user was deleted
            Assert.Null(await _context.Bruker.FindAsync(1));
            Assert.Null(await _context.Person.FirstOrDefaultAsync(p => p.BrukerId == 1));
        }

        [Fact]
        public async Task RedigerBruker_Get_ExistingUser_ReturnsViewResult_WithModel()
        {
            // Arrange
            var bruker = new Bruker { 
                BrukerId = 1, 
                Email = "test@example.com", 
                BrukerType = "Admin",
                Passord = "TestPassord123"
            };
            var person = new Person { BrukerId = 1, Fornavn = "Test", Etternavn = "Testesen" };

            await _context.Bruker.AddAsync(bruker);
            await _context.Person.AddAsync(person);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.RedigerBruker(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<BrukerOversiktViewModel>(viewResult.Model);
            
            Assert.Equal(bruker.BrukerId, model.BrukerId);
            Assert.Equal(bruker.Email, model.Email);
            Assert.Equal(bruker.BrukerType, model.BrukerType);
            Assert.Equal(person.Fornavn, model.Fornavn);
            Assert.Equal(person.Etternavn, model.Etternavn);
        }

        [Fact]
        public async Task RedigerBruker_Post_ValidModel_UpdatesUserAndRedirects()
        {
            // Arrange
            var originalBruker = new Bruker 
            { 
                BrukerId = 1, 
                Email = "original@example.com",
                BrukerType = "Bruker",
                Passord = "TestPassord123"
            };
            var originalPerson = new Person 
            { 
                BrukerId = 1, 
                Fornavn = "Original", 
                Etternavn = "Test" 
            };

            await _context.Bruker.AddAsync(originalBruker);
            await _context.Person.AddAsync(originalPerson);
            await _context.SaveChangesAsync();

            var updatedModel = new BrukerOversiktViewModel 
            { 
                BrukerId = 1, 
                Email = "updated@example.com", 
                BrukerType = "Admin", 
                Fornavn = "Updated", 
                Etternavn = "Testesen" 
            };

            // Act
            var result = await _controller.RedigerBruker(updatedModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("BrukerOversikt", redirectResult.ActionName);

            // Verify database updates
            var updatedBruker = await _context.Bruker.FindAsync(1);
            var updatedPerson = await _context.Person.FirstOrDefaultAsync(p => p.BrukerId == 1);

            Assert.NotNull(updatedBruker);
            Assert.NotNull(updatedPerson);
            Assert.Equal(updatedModel.Email, updatedBruker.Email);
            Assert.Equal(updatedModel.BrukerType, updatedBruker.BrukerType);
            Assert.Equal(updatedModel.Fornavn, updatedPerson.Fornavn);
            Assert.Equal(updatedModel.Etternavn, updatedPerson.Etternavn);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}