using System;
using System.Linq;

namespace TravelRepublic.Flights.Filtering.Rules
{
    /// <summary>
    /// Rule to match a flight that has departed since the given date
    /// </summary>
    public sealed class FlightDepartedRule : IFlightMatchRule
    {
        private readonly DateTime departedBy;

        public FlightDepartedRule(DateTime departedBy)
        {
            this.departedBy = departedBy;
        }

        public bool IsMatch(Flight flight)
        {
            return flight.Segments.First().Departure.Time < this.departedBy;
        }
    }
}
