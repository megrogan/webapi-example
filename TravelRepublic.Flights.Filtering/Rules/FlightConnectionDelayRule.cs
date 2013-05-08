using System;

namespace TravelRepublic.Flights.Filtering.Rules
{
    /// <summary>
    /// Rule to match a flight that has a delay between flight segments totalling more than the given delay
    /// </summary>
    public sealed class FlightConnectionDelayRule : IFlightMatchRule
    {
        private readonly TimeSpan delay;

        public FlightConnectionDelayRule(TimeSpan delay)
        {
            this.delay = delay;
        }

        public bool IsMatch(Flight flight)
        {
            Segment prevSegment = null;
            var cumulativeDelay = new TimeSpan();

            foreach (var segment in flight.Segments)
            {
                if (prevSegment != null)
                {
                    cumulativeDelay += segment.Departure.Time - prevSegment.Arrival.Time;

                    if (cumulativeDelay > this.delay)
                    {
                        return true;
                    }
                }

                prevSegment = segment;
            }

            return false;
        }
    }
}
