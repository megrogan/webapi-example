using System;

namespace TravelRepublic.Flights
{
    public class Segment
    {
        public TimeAndPlace Departure { get; set; }
        public TimeAndPlace Arrival { get; set; }

        public override string ToString()
        {
            return Departure + " -> " + Arrival;
        }
    }
}
