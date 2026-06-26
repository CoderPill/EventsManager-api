using EventsManager.Core.Entities;

namespace EventsManager.Core.Constants
{
    public static class SystemValues
    {
        public static class Infrastructure
        {
            public const string EnvironmentPrefix = "EVENTSMANAGER_";
            public const string DbContextFactoryConnectionStringEnvKey = "ConnectionStringsSettings__DefaultConnection";
            public const string AlphaNumericAlphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            public const string CORSPolicyName = "AllowedOriginCORS";
            public const string JsonContentType = "application/json";
        }
        public static class QueryIncludes
        {
            public const string Event_Venue = nameof(EventEntity.Venue);
            public const string Event_Reservations = nameof(EventEntity.Reservations);


            public const string Reservation_Event = nameof(ReservationEntity.Event);
        }
    }
}
