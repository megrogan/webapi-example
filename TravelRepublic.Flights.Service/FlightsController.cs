using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.OData;
using TravelRepublic.Flights.Repository;
using TravelRepublic.Flights.Validation;

namespace TravelRepublic.Flights.Service
{
    public class FlightsController : ApiController
    {
        private readonly IFlightRepository repository;
        private readonly IFlightValidator validator;

        public FlightsController(IFlightRepository repository, IFlightValidator validator)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            this.repository = repository;
            this.validator = validator;
        }

        public IQueryable<Flight> Get()
        {
            return repository.Query();
        }

        public Flight Get(Guid id)
        {
            var item = repository.Get(id);

            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return item;
        }

        public HttpResponseMessage Post(Flight item)
        {
            if (!validator.IsValid(item))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (!repository.Add(item))
            {
                // A flight with this id already exists
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            var response = Request.CreateResponse<Flight>(HttpStatusCode.Created, item);

            var uri = Url.Link("DefaultApi", new { id = item.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        public void Put(Guid id, Flight flight)
        {
            if (!validator.IsValid(flight))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            flight.Id = id;

            if (!repository.Update(flight))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [AcceptVerbs("Patch")]
        public HttpResponseMessage Patch(Guid id, Delta<Flight> delta)
        {
            var flight = repository.Get(id);

            if (flight == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            delta.Patch(flight);

            if (!validator.IsValid(flight))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // TODO: Think about version conflicts
            if (!repository.Update(flight))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.NoContent); 
        }

        public void Delete(Guid id)
        {
            var item = repository.Get(id);

            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            repository.Remove(id);
        }
    }
}
