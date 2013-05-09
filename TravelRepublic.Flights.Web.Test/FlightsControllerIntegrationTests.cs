using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http;
using System.Collections.Generic;

namespace TravelRepublic.Flights.Web.Test
{
    [TestClass]
    public class FlightsControllerIntegrationTests
    {
        private HttpClient client;
        private WebApiSetup webApiSetup;

        [TestInitialize()]
        public void SetupHttpClient()
        {
            var config = new HttpConfiguration();
            webApiSetup = new WebApiSetup(config, ".");

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // Some material on tracing Web Api
            // http://www.asp.net/web-api/overview/testing-and-debugging/tracing-in-aspnet-web-api
            // http://himanshudesai.wordpress.com/2013/02/19/web-api-introduction-to-systemdiagnosticstracewriter/
            config.EnableSystemDiagnosticsTracing();

            var server = new HttpServer(config);
            client = new HttpClient(server) {BaseAddress = new Uri("http://localhost")};
        }

        [TestCleanup()]
        public void Cleanup()
        {
            webApiSetup.Dispose();
        }

        [TestMethod]
        public void Post_ValidFlight_ReturnsCreatedStatusCode()
        {
            // Arrange
            var flight = new Flight { Id = Guid.NewGuid() };

            // Act
            var response = client.PostAsJsonAsync("api/flights", flight).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNotNull(response.Content);
        }

        [TestMethod]
        public void Get_PostedFlight_ReturnsSameFlight()
        {
            // Arrange
            var flight = new Flight { Id = Guid.NewGuid() };
            client.PostAsJsonAsync("api/flights", flight).Wait();

            // Act
            var response = client.GetAsync("api/flights/" + flight.Id).Result;

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsNotNull(response.Content);
            var result = response.Content.ReadAsAsync<Flight>().Result;
            Assert.AreEqual(flight.Id, result.Id);
        }

        [TestMethod]
        public void GetAll_PostedFlight_ReturnsFlightList()
        {
            // Arrange
            var flight = new Flight { Id = Guid.NewGuid() };
            client.PostAsJsonAsync("api/flights", flight).Wait();

            // Act
            var response = client.GetAsync("api/flights").Result;

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsNotNull(response.Content);
            var result = response.Content.ReadAsAsync<IEnumerable<Flight>>().Result;
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(flight.Id, result.First().Id);            
        }
    }
}
