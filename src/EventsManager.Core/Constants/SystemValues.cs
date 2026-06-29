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
            public const string ReservationCodePrefix = "EV-";
            public const string CORSPolicyName = "AllowedOriginCORS";
            public const string JsonContentType = "application/json";
        }
        public static class QueryIncludes
        {
            public const string Event_Venue = nameof(EventEntity.Venue);
            public const string Event_Reservations = nameof(EventEntity.Reservations);
            public const string Reservation_Event = nameof(ReservationEntity.Event);
        }
        public static class PropertyNames
        {
            public const string Name = "Nombre";
            public const string Capacity = "Capacidad";
            public const string City = "Ciudad";
            public const string Email = "Correo Electronico";
            public const string Username = "Nombre de Usuario";
            public const string Password = "Contraseña";
            public const string Role = "Rol";
            public const string Title = "Titulo";
            public const string Description = "Descripcion";
            public const string Price = "Precio";
            public const string StartDate = "Fecha de Inicio";
            public const string EndDate = "Fecha de Finalizacion";
            public const string DateNow = "la fecha actual";
            public const string Status = "Estado";
            public const string Quantity = "Cantidad";
            public const string ReservationCode = "Codigo de Reserva";
            public const string Type = "Tipo";
            public const string Venue = "Lugar";
            public const string Event = "Evento";
            public const string Reservation = "Reservacion";
            public const string MaxCapacity = "Capacidad Maxima";
            public const string EventType = "Tipo de Evento";
        }
        public static class Tags
        {
            public const string Validator_MaxLength = "{MaxLength}";
            public const string Validator_MinLength = "{MinLength}";
            public const string Validator_ComparisonValue = "{ComparisonValue}";
            public const string Validator_TotalLength = "{TotalLength}";
            public const string Validator_PropertyValue = "{PropertyValue}";
            public const string Validator_PropertyName = "{PropertyName}";
            public const string Validator_CouldNoBeConverted = "could not be converted";
        }
        public static class ReservationRules
        {
            public const int SecondsPerHour = 3600;
            public const int HoursBeforeStartForLastDayRules = 24;
            public const int MaxQuantityForLastDay = 5;
            public const decimal ExpensiveEventPriceThreshold = 100m;
            public const int MaxQuantityForExpensiveEvent = 10;
            public const int HoursBeforeStartForCancellationPenalty = 48;
        }
    }
}
