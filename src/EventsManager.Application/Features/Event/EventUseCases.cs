using EventsManager.Application.Features.Event.Add;
using EventsManager.Application.Features.Event.Get;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.Event
{
    public record EventUseCases(GetEventsHandler GetEvents,AddEventHandler AddEvent);
}
