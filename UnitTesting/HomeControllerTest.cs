using Xunit;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Data;
using KartverketWebApp.Data;
using KartverketWebApp.Services;
using KartverketWebApp.Models;
using KartverketWebApp.Controllers;
using KartverketWebApp.API_Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Tests
{
    public class HomeControllerTests : IDisposable
    {
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly Mock<IStednavn> _stednavnServiceMock;
        private readonly Mock<ISokeService> _sokeServiceMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IOptions<ApiSettings>> _apiSettingsMock;
        private readonly Mock<IDbConnection> _dbConnectionMock;
        private readonly ApplicationDbContext _context;

        public HomeControllerTests()
        {
            // Setup mocks
            _loggerMock = new Mock<ILogger<HomeController>>();
            _stednavnServiceMock = new Mock<IStednavn>();
            _sokeServiceMock = new Mock<ISokeService>();
            _apiSettingsMock = new Mock<IOptions<ApiSettings>>();
            _dbConnectionMock = new Mock<IDbConnection>();

            // Setup HttpClient with mocked handler
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"omrade\":{\"coordinates\":[[[]]]}}"),
                });

            _httpClient = new HttpClient(handlerMock.Object);

            // Setup InMemory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            
            // Setup ApiSettings
            _apiSettingsMock.Setup(x => x.Value).Returns(new ApiSettings 
            { 
                KommuneInfoApiBaseUrl = "http://test.api" 
            });
        }

        [Fact]
        public async Task Index_WithValidData_ReturnsRedirectToTakkRapport()
        {
            // Arrange
            var controller = new HomeController(
                _loggerMock.Object,
                _stednavnServiceMock.Object,
                _sokeServiceMock.Object,
                _httpClient,
                _apiSettingsMock.Object,
                _context,
                _dbConnectionMock.Object
            );

            // Setup test user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Setup test data with all required fields
            var bruker = new Bruker 
            { 
                BrukerId = 1, 
                Email = "test@example.com",
                BrukerType = "Innsender",
                Passord = "TestPassword123"
            };
            var person = new Person 
            { 
                PersonId = 1, 
                BrukerId = 1,
                Fornavn = "Test",  // Lagt til påkrevd felt
                Etternavn = "Testesen" // Lagt til påkrevd felt
            };
            _context.Bruker.Add(bruker);
            _context.Person.Add(person);
            await _context.SaveChangesAsync();

            var koordinater = new List<KoordinatModel>
            {
                new KoordinatModel { Nord = 60.0, Ost = 10.0 }
            };

            // Setup mock response for stednavnService
            _stednavnServiceMock.Setup(x => x.GetStednavnAsync(
                It.IsAny<double>(), 
                It.IsAny<double>(), 
                It.IsAny<int>()))
                .ReturnsAsync(new StednavnResponse 
                { 
                    Fylkesnavn = "TestFylke",
                    Kommunenavn = "TestKommune",
                    Fylkesnummer = "1",
                    Kommunenummer = "101"
                });

            // Act
            var result = await controller.Index(
                koordsys: 1,
                tittel: "Test Title",
                beskrivelse: "Test Description",
                mapType: "Turkart",
                rapportType: "Test Type",
                koordinater: koordinater,
                file: null) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TakkRapport", result.ActionName);

            // Verify database entries
            var savedKart = await _context.Kart.FirstOrDefaultAsync();
            Assert.NotNull(savedKart);
            Assert.Equal("Test Title", savedKart.Tittel);
            Assert.Equal("Test Description", savedKart.Beskrivelse);

            var savedRapport = await _context.Rapport.FirstOrDefaultAsync();
            Assert.NotNull(savedRapport);
            Assert.Equal("Uåpnet", savedRapport.RapportStatus);
        }

        [Fact]
        public async Task Saksbehandler_WithValidUser_ReturnsCorrectView()
        {
            // Arrange
            var controller = new HomeController(
                _loggerMock.Object,
                _stednavnServiceMock.Object,
                _sokeServiceMock.Object,
                _httpClient,
                _apiSettingsMock.Object,
                _context,
                _dbConnectionMock.Object
            );

            // Setup test user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Setup test data with all required fields
            var bruker = new Bruker 
            { 
                BrukerId = 1, 
                Email = "test@example.com",
                BrukerType = "Saksbehandler",
                Passord = "TestPassword123"
            };
            var person = new Person 
            { 
                PersonId = 1, 
                BrukerId = 1, 
                Fornavn = "Test", 
                Etternavn = "Person" 
            };
            var ansatt = new Ansatt 
            { 
                AnsattId = 1, 
                PersonId = 1, 
                Kommunenummer = 301 
            };
            
            _context.Bruker.Add(bruker);
            _context.Person.Add(person);
            _context.Ansatt.Add(ansatt);
            await _context.SaveChangesAsync();

            // Act
            var result = await controller.Saksbehandler() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("~/Views/Home/Saksbehandler/Saksbehandler.cshtml", result.ViewName);
            var model = Assert.IsType<CombinedViewModel>(result.Model);
            Assert.NotNull(model.ActiveRapporter);
            Assert.NotNull(model.ResolvedRapporter);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}