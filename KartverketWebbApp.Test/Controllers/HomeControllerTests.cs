using Microsoft.AspNetCore.Mvc; // Add this using directive
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using KartverketWebApp.API_Models;
using Moq;
using Microsoft.Extensions.Options;
using Xunit;
using System.Data;
using KartverketWebApp.Services;
using KartverketWebApp.Models;
using KartverketWebApp.Data;
using KartverketWebApp.Controllers; // Add this using directive


namespace KartverketWebApp.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly Mock<IStednavn> _stednavnServiceMock;
        private readonly Mock<ISokeService> _sokeServiceMock;
        private readonly Mock<HttpClient> _httpClientMock;
        private readonly Mock<IOptions<KartverketWebApp.API_Models.ApiSettings>> _apiSettingsMock; // Update the type here
        private readonly Mock<IDbConnection> _dbConnectionMock;
        private readonly HomeController _controller;
        private readonly ApplicationDbContext _dbContext;

        public HomeControllerTests()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _stednavnServiceMock = new Mock<IStednavn>();
            _sokeServiceMock = new Mock<ISokeService>();
            _httpClientMock = new Mock<HttpClient>();
            _apiSettingsMock = new Mock<IOptions<KartverketWebApp.API_Models.ApiSettings>>(); // Update the type here
            _dbConnectionMock = new Mock<IDbConnection>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new ApplicationDbContext(options);

            _controller = new HomeController(
                _loggerMock.Object,
                _stednavnServiceMock.Object,
                _sokeServiceMock.Object,
                _httpClientMock.Object,
                _apiSettingsMock.Object, // Pass this mock
                _dbContext,
                _dbConnectionMock.Object
            );
        }

        [Fact]
        public void TakkRapport_ReturnsViewResult()
        {
            // Act
            var result = _controller.TakkRapport();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Home/Innsender/TakkRapport.cshtml", viewResult.ViewName);
        }

        [Fact]
        public void Innlogging_ReturnsViewResult()
        {
            // Act
            var result = _controller.Innlogging();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Admin_ReturnsViewResult()
        {
            // Act
            var result = _controller.Admin();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Soke_ReturnsViewResult()
        {
            // Act
            var result = _controller.soke();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WhenKoordinaterIsNull()
        {
            // Act
            var result = await _controller.Index(0, "title", "description", "mapType", "reportType", null, (IFormFile?)null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WhenKoordinaterIsEmpty()
        {
            // Act
            var result = await _controller.Index(0, "title", "description", "mapType", "reportType", new List<KoordinatModel>(), (IFormFile?)null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
        }
    }
}
