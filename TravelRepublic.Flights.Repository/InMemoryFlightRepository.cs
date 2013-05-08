using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TravelRepublic.Flights.Repository
{
    public class InMemoryFlightRepository : IFlightRepository
    {
        private readonly ConcurrentDictionary<Guid, Flight> flightsMap = new ConcurrentDictionary<Guid, Flight>();

        public IEnumerable<Flight> GetAll()
        {
            return flightsMap.Values;
        }

        public IQueryable<Flight> Query()
        {
            return GetAll().AsQueryable();
        }

        public Flight Get(Guid id)
        {
            return !flightsMap.ContainsKey(id) ? null : flightsMap[id];
        }

        public bool Add(Flight item)
        {
            return flightsMap.TryAdd(item.Id, item);
        }

        public void Remove(Guid id)
        {
            Flight flight;
            flightsMap.TryRemove(id, out flight);
        }

        public bool Update(Flight item)
        {
            if (flightsMap[item.Id] == null)
                return false;

            flightsMap[item.Id] = item;

            return true;
        }
    }
}
