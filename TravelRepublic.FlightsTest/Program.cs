using System;
using TravelRepublic.Flights.Filtering;
using TravelRepublic.Flights.Filtering.Rules;
using System.Collections.Generic;
using TravelRepublic.Flights;
using TravelRepublic.Flights.Testing;

namespace TravelRepublic.FlightsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get test flights
            var flights = GetFlights();

            // Filter the flights
            var matchingFlights = new FlightFilter()
                .AddRule(new ArrivesBeforeDepartsRule())
                .AddRule(new FlightDepartedRule(DateTime.UtcNow))
                .AddRule(new FlightConnectionDelayRule(TimeSpan.FromHours(2)))
                .Filter(flights);

            // Write the filtered flights to the console
            foreach (var flight in matchingFlights)
            {
                Console.Out.WriteLine(flight.ToString());
            }
        }

        private static IList<Flight> GetFlights()
        {
            DateTime threeDaysFromNow = DateTime.UtcNow.AddDays(3);

            return new List<Flight>
			{
                //A normal flight with two hour duration
			    FlightBuilder.CreateFlight(threeDaysFromNow, threeDaysFromNow.AddHours(2)),

                //A normal multi segment flight
			    FlightBuilder.CreateFlight(threeDaysFromNow, threeDaysFromNow.AddHours(2), threeDaysFromNow.AddHours(3), threeDaysFromNow.AddHours(5)),
                           
                //A flight departing in the past
                FlightBuilder.CreateFlight(threeDaysFromNow.AddDays(-6), threeDaysFromNow),

                //A flight that departs before it arrives
                FlightBuilder.CreateFlight(threeDaysFromNow, threeDaysFromNow.AddHours(-6)),

                //A flight with more than two hours ground time
                FlightBuilder.CreateFlight(threeDaysFromNow, threeDaysFromNow.AddHours(2), threeDaysFromNow.AddHours(5), threeDaysFromNow.AddHours(6)),

                //Another flight with more than two hours ground time
                FlightBuilder.CreateFlight(threeDaysFromNow, threeDaysFromNow.AddHours(2), threeDaysFromNow.AddHours(3), threeDaysFromNow.AddHours(4), threeDaysFromNow.AddHours(6), threeDaysFromNow.AddHours(7))
			};
        }
    }
}
