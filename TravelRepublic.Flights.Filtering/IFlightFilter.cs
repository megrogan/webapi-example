using System.Collections.Generic;

namespace TravelRepublic.Flights.Filtering
{
    /// <summary>
    /// Interface to filter a list of flights
    /// </summary>
    public interface IFlightFilter
    {
        IList<Flight> Filter(IList<Flight> flights);
    }
}
