using System;
using System.Collections.Generic;
using System.Linq;

namespace TravelRepublic.Flights.Repository
{
    public interface IFlightRepository
    {
        IEnumerable<Flight> GetAll();
        IQueryable<Flight> Query();
        Flight Get(Guid id);
        bool Add(Flight item);
        void Remove(Guid id);
        bool Update(Flight item);
    }
}
