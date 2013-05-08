namespace TravelRepublic.Flights.Validation
{
    public class DummyFlightValidator : IFlightValidator
    {
        public bool IsValid(Flights.Flight flight)
        {
            return true;
        }
    }
}
