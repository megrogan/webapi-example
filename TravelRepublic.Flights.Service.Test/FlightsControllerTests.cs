using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TravelRepublic.Flights.Repository;
using System.Net;
using System.Net.Http;
using TravelRepublic.Flights.Validation;

namespace TravelRepublic.Flights.Service.Test
{
    /// <summary>
    /// http://www.peterprovost.org/blog/2012/06/16/unit-testing-asp-dot-net-web-api
    /// </summary>
    [TestClass]
    public class FlightsControllerTests
    {
        [TestMethod]
        public void Get_ExistingFlight_ReturnsFlight()
        {
            // Arrange
            var flight = new Flight {Id = Guid.NewGuid()};
            var repo = new Mock<IFlightRepository>();
            var validator = new Mock<IFlightValidator>();
            repo.Setup(r => r.Get(flight.Id)).Returns(flight);
            var controller = new FlightsController(repo.Object, validator.Object);

            // Act
            var result = controller.Get(flight.Id);

            // Assert
            Assert.ReferenceEquals(result, flight);
        }

        [TestMethod]
        public void Get_NonExistantFlight_ThrowsHttpResponseException()
        {
            // Arrange
            var repo = new Mock<IFlightRepository>();
            repo.Setup(r => r.Get(It.IsAny<Guid>())).Returns(null as Flight);
            var validator = new Mock<IFlightValidator>();
            var controller = new FlightsController(repo.Object, validator.Object);

            // Act + Assert
            try
            {
                controller.Get(Guid.NewGuid());
                Assert.IsTrue(false, "No exception thrown");
            }
            catch (HttpResponseException ex1)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, ex1.Response.StatusCode, "Unexpected status code");
            }
            catch (Exception ex2)
            {
                Assert.IsTrue(false, "Unexpected exception type: " + ex2.GetType());
            }
        }

        [TestMethod]
        public void Post_ValidFlight_ReturnsCreatedStatusCode()
        {
            // Arrange
            var flight = new Flight { Id = Guid.NewGuid() };
            var repo = new Mock<IFlightRepository>();
            repo.Setup(r => r.Add(It.IsAny<Flight>())).Returns(true);
            var validator = new Mock<IFlightValidator>();
            validator.Setup(v => v.IsValid(It.IsAny<Flight>())).Returns(true);
            var controller = new FlightsController(repo.Object, validator.Object);

            SetupControllerForTests(controller);

            // Act
            var result = controller.Post(flight);

            //Assert            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

        }

        [IgnoreAttribute]
        [TestMethod]
        public void Post_InvalidFlight_ThrowsHttpResponseException()
        {
            throw new NotImplementedException();
        }

        [IgnoreAttribute]
        [TestMethod]
        public void Post_ExistingFlight_ThrowsHttpResponseException()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Post_ValidFlight_ReturnsCorrectLocationInResponseMessage()
        {
            // Arrange
            var repo = new Mock<IFlightRepository>();
            repo.Setup(r => r.Add(It.IsAny<Flight>())).Returns(true);
            var validator = new Mock<IFlightValidator>();
            validator.Setup(v => v.IsValid(It.IsAny<Flight>())).Returns(true);
            var controller = new FlightsController(repo.Object, validator.Object);
            SetupControllerForTests(controller);

            // Act
            var flight = new Flight { Id = Guid.NewGuid() };
            var result = controller.Post(flight);

            //Assert
            Assert.AreEqual("http://localhost/api/flights/" + flight.Id, result.Headers.Location.ToString());
        }

        [TestMethod]
        public void Post_ValidFlight_CallsAddOnRepository()
        {
            // Arrange
            var addedFlight = default(Flight);
            var repo = new Mock<IFlightRepository>();
            repo.Setup(r => r.Add(It.IsAny<Flight>())).Callback<Flight>(f => addedFlight = f).Returns(true);
            var validator = new Mock<IFlightValidator>();
            validator.Setup(v => v.IsValid(It.IsAny<Flight>())).Returns(true);
            var controller = new FlightsController(repo.Object, validator.Object);
            SetupControllerForTests(controller);

            // Act
            var flight = new Flight { Id = Guid.NewGuid() };
            var result = controller.Post(flight);

            // Assert
            Assert.ReferenceEquals(flight, addedFlight);
        }

        [IgnoreAttribute]
        [TestMethod]
        public void Put_ValidExistantFlight_UpdatesRepository()
        {
            throw new NotImplementedException();
        }

        [IgnoreAttribute]
        [TestMethod]
        public void Put_InvalidFlight_ThrowsHttpResponseException()
        {
            throw new NotImplementedException();
        }

        [IgnoreAttribute]
        [TestMethod]
        public void Put_ValidNonExistantFlight_ThrowsHttpResponseException()
        {
            throw new NotImplementedException();
        }

        [IgnoreAttribute]
        [TestMethod]
        public void Put_ValidExistantFlight_SetsId()
        {
            throw new NotImplementedException();
        }

        [IgnoreAttribute]
        [TestMethod]
        public void Delete_Flight_CallsRepositoryRemove()
        {
            throw new NotImplementedException();
        }

        [IgnoreAttribute]
        [TestMethod]
        public void Delete_Flight_ReturnsResponseMessageWithNoContentStatusCode()
        {
            throw new NotImplementedException();
        }

        [IgnoreAttribute]
        [TestMethod]
        public void Delete_NonExistantFlight_ThrowsHttpResponseException()
        {
            throw new NotImplementedException();
        }

        private static void SetupControllerForTests(ApiController controller)
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/flights");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "flights" } });

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            controller.Request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;
        }
    }
}
