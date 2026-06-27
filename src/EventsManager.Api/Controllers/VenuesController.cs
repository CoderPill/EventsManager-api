using EventsManager.Api.Extensions;
using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Features.Venue;
using EventsManager.Application.Features.Venue.Add;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsManager.Api.Controllers
{
    public class VenuesController : BaseApiController
    {
        private readonly VenueUseCases _venueUseCases;
        public VenuesController(VenueUseCases venueUseCases)
        {
            _venueUseCases = venueUseCases;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            return await _venueUseCases.GetVenues.Execute(Unit.Value)
                .ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddVenueRequest request)
        {
            return await _venueUseCases.AddVenue.Execute(request)
                .ToActionResult();
        }
    }
}
