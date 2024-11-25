using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KartverketWebApp.Controllers;
using KartverketWebApp.Data;
using KartverketWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace KartverketWebApp.UnitTests
{
    public class MinSideControllerTests
    {
        private ApplicationDbContext GetInMemoryDatabase()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewWithModel_WhenUserExists()
        {
            // Arrange
            var context = GetInMemoryDatabase();
            var loggerMock = new Mock<ILogger<MinSideController>>();

            // Opprett testdata
            var bruker = new Bruker
            {
                BrukerId = 1,
                Email = "test@kartverket.no",
                BrukerType = "Bruker",
                Passord = "Test123!"
            };

            var person = new Person
            {
                PersonId = 1,
                Fornavn = "Test",
                Etternavn = "Bruker",
                BrukerId = bruker.BrukerId // Knytter person til bruker
            };

            var ansatt = new Ansatt
            {
                AnsattId = 1,
                PersonId = person.PersonId, // Knytter ansatt til person
                Kommunenummer = 1234,
                AnsettelsesDato = DateTime.Now
            };

            var kart = new Kart
            {
                KartEndringId = 1,
                Tittel = "Testkart",
                Beskrivelse = "Dette er et testkart",
                Koordsys = 4326,
                MapType = "Topografisk",
                RapportType = "Endring",
                FilePath = null // Ingen fil kreves for test
            };

            var rapport = new Rapport
            {
                RapportId = 1,
                PersonId = person.PersonId, // Knytter rapport til person
                RapportStatus = "Uåpnet",
                Opprettet = DateTime.Now,
                TildelAnsattId = ansatt.AnsattId, // Tildel ansatt
                KartEndringId = kart.KartEndringId, // Knytter til kart
                BehandletDato = null
            };

            context.Bruker.Add(bruker);
            context.Person.Add(person);
            context.Ansatt.Add(ansatt);
            context.Kart.Add(kart);
            context.Rapport.Add(rapport);
            await context.SaveChangesAsync();

            // Mock HttpContext
            var httpContextMock = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            httpContextMock.Setup(c => c.User.Identity.Name).Returns("test@kartverket.no");

            var controller = new MinSideController(context, null, loggerMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<MinSideViewModel>(viewResult.Model);

            Assert.NotNull(model.Rapporter); // Sikre at listen ikke er null
            Assert.NotEmpty(model.Rapporter); // Sikre at listen inneholder data
            Assert.Single(model.Rapporter); // Sikre at det kun finnes én rapport

            // Ekstra validering
            Assert.Equal("Test", model.Fornavn);
            Assert.Equal("Bruker", model.Etternavn);
            Assert.Equal("test@kartverket.no", model.Email);
        }
    }
}