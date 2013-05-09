using System;
using System.Linq;

using System.Collections.Generic;

namespace TravelRepublic.Flights
{
    public class Flight
    {
        public Guid Id { get; set; }
        public TimeAndPlace Departure { get; set; }
        public TimeAndPlace Arrival { get; set; }

        public override string ToString()
        {
            return Departure + " -> " + Arrival;
        }
    }
}
