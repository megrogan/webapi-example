using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelRepublic.Flights.Filtering
{
    /// <summary>
    /// Filter flights which match the registered rules
    /// </summary>
    public class FlightFilter : IFlightFilter
    {
        // A set of flight matching rules
        protected readonly ISet<IFlightMatchRule> rules = new HashSet<IFlightMatchRule>();

        /// <summary>
        /// Add a rule used to filter a flight
        /// </summary>
        /// <param name="rule"></param>
        public FlightFilter AddRule(IFlightMatchRule rule)
        {
            rules.Add(rule);
            return this;
        }

        public virtual IList<Flight> Filter(IList<Flight> flights)
        {
            IEnumerable<Flight> remaining = flights;

            // Apply each flight match rule in turn
            foreach (var rule in rules)
            {
                // Note: Need to take a copy of the loop variable otherwise in C# <= v4.0 the closure
                // below will use the last rule each time rather than the expected current rule
                // http://stackoverflow.com/questions/14907987/access-to-foreach-variable-in-closure
                var r1 = rule;
                // Keep flights which *don't* match the filtering rule
                remaining = remaining.Where(f => !r1.IsMatch(f));
            }

            // Return a list of the filtered flights
            return remaining.ToList<Flight>();
        }
    }
}
