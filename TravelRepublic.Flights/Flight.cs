using System;
using System.Linq;

using System.Collections.Generic;

namespace TravelRepublic.Flights
{
    public class Flight
    {
        public Guid Id { get; set; }
        public IList<Segment> Segments { get; set; }

        public override string ToString()
        {
            return String.Join(", ", Segments.Select(s => s.ToString()));
        }
    }
}
