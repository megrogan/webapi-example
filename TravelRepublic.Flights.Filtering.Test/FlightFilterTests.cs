using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using TravelRepublic.Flights.Testing;

namespace TravelRepublic.Flights.Filtering.Test
{
    [TestClass]
    public class FlightFilterTests
    {
        [TestMethod]
        public void AddRule_Call_ReturnsSelf()
        {
            var rule = new Mock<IFlightMatchRule>();

            var filter = new FlightFilter();
            Assert.ReferenceEquals(filter, filter.AddRule(rule.Object));
        }

        [TestMethod]
        public void Filter_NoRules_ReturnsAllFlights()
        {
            var flights = new List<Flight>()
            {
                FlightBuilder.CreateFlight(),
                FlightBuilder.CreateFlight()
            };

            var filter = new FlightFilter();
            Assert.IsTrue(FlightsMatch(flights, filter.Filter(flights)));
        }

        [TestMethod]
        public void Filter_NonMatchingRule_ReturnsAllFlights()
        {
            var flights = new List<Flight>()
            {
                FlightBuilder.CreateFlight(),
                FlightBuilder.CreateFlight()
            };

            var rule = new Mock<IFlightMatchRule>();
            rule.Setup(r => r.IsMatch(It.IsAny<Flight>())).Returns(false);

            var filter = new FlightFilter();
            filter.AddRule(rule.Object);
            Assert.IsTrue(FlightsMatch(flights, filter.Filter(flights)));
        }

        [TestMethod]
        public void Filter_MatchingRule_ReturnsNonMatchingFlights()
        {
            var flight1 = FlightBuilder.CreateFlight();
            var flight2 = FlightBuilder.CreateFlight();
            var flights = new List<Flight>() { flight1, flight2 };

            var rule = new Mock<IFlightMatchRule>();
            rule.Setup(r => r.IsMatch(flight1)).Returns(false);
            rule.Setup(r => r.IsMatch(flight2)).Returns(true);

            var filter = new FlightFilter();
            filter.AddRule(rule.Object);
            Assert.IsTrue(FlightsMatch(new List<Flight>() { flight1 }, filter.Filter(flights)));
        }

        [TestMethod]
        public void Filter_OneMatchingOneNonMatchingRule_ReturnsExpectedFlights()
        {
            var flight1 = FlightBuilder.CreateFlight();
            var flight2 = FlightBuilder.CreateFlight();
            var flights = new List<Flight>() { flight1, flight2 };

            var rule1 = new Mock<IFlightMatchRule>();
            rule1.Setup(r => r.IsMatch(flight1)).Returns(false);
            rule1.Setup(r => r.IsMatch(flight2)).Returns(true);

            var rule2 = new Mock<IFlightMatchRule>();
            rule2.Setup(r => r.IsMatch(It.IsAny<Flight>())).Returns(false);

            var filter = new FlightFilter();
            filter.AddRule(rule1.Object);
            filter.AddRule(rule2.Object);
            Assert.IsTrue(FlightsMatch(new List<Flight>() { flight1 }, filter.Filter(flights)));
        }

        private bool FlightsMatch(IList<Flight> flights1, IList<Flight> flights2)
        {
            var set1 = new HashSet<Flight>(flights1);
            var set2 = new HashSet<Flight>(flights2);
            return set1.SetEquals(set2);
        }
    }
}
