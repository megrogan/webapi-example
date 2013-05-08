
namespace TravelRepublic.Flights.Filtering
{
    /// <summary>
    /// Interface for a rule to match a flight
    /// </summary>
    public interface IFlightMatchRule
    {
        bool IsMatch(Flight flight);
    }
}
