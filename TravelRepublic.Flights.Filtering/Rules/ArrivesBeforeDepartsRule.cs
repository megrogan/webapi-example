using System;
using System.Linq;

namespace TravelRepublic.Flights.Filtering.Rules
{
    /// <summary>
    /// Rule to match a flight that arrives before it departs
    /// </summary>
    public sealed class ArrivesBeforeDepartsRule : IFlightMatchRule
    {
        public bool IsMatch(Flight flight)
        {
            return flight.Segments.Any(s => s.Arrival.Time < s.Departure.Time);
        }
    }
}
