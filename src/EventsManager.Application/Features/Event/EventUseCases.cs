using EventsManager.Application.Features.Event.Add;
using EventsManager.Application.Features.Event.Get;
using EventsManager.Application.Features.Event.GetOccupationReport;

namespace EventsManager.Application.Features.Event
{
    public record EventUseCases(GetEventsHandler GetEvents, AddEventHandler AddEvent, GetOccupationReportHandler GetOccupationReport);
}
