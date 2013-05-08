using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelRepublic.Flights.Testing
{
    public class FlightBuilder
    {
        public static Flight CreateFlight(params DateTime[] dates)
        {
            if (dates.Length%2 != 0) throw new ArgumentException("You must pass an even number of dates,", "dates");

            var departureDates = dates.Where((date, index) => index%2 == 0);
            var arrivalDates = dates.Where((date, index) => index%2 == 1);

            var segments = departureDates.Zip(arrivalDates,
                                              (departureDate, arrivalDate) =>
                                              new Segment
                                                  {
                                                      Departure = new TimeAndPlace {Time = departureDate},
                                                      Arrival = new TimeAndPlace {Time = arrivalDate}
                                                  }).ToList();

            return new Flight { Id = Guid.NewGuid(), Segments = segments };
        }
    }
}
