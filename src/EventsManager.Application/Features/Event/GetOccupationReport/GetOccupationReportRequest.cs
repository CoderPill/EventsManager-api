namespace EventsManager.Application.Features.Event.GetOccupationReport
{
    public record GetOccupationReportRequest(int EventId)
    {
        public static GetOccupationReportRequest From(int eventId)
        {
            return new(eventId);
        }
    }
}
