using EventsManager.Api.Extensions;
using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Features.Event;
using EventsManager.Application.Features.Event.Add;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsManager.Api.Controllers
{
    public class EventController : BaseApiController
    {
        private readonly EventUseCases _eventUseCases;
        public EventController(EventUseCases eventUseCases)
        {
            _eventUseCases = eventUseCases;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            return await _eventUseCases.GetEvents.Execute(Unit.Value)
                .ToActionResult();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(AddEventRequest request)
        {
            return await _eventUseCases.AddEvent.Execute(request)
                .ToActionResult();
        }
    }
}
