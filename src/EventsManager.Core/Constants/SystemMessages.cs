namespace EventsManager.Core.Constants
{
    public static class SystemMessages
    {
        public static class Validations
        {
            private const string TheProperty = "La propiedad {0} ";
            public const string Error_Required = TheProperty+"es requerida";
            public const string Error_MaxLength = TheProperty+"no puede tener mas de {1} caracteres";
            public const string Error_MinLength = TheProperty+"no puede tener menos de {1} caracteres";
            public const string Error_GreaterThan = TheProperty+"debe ser mayor a {1}";
            public const string Error_GreaterThanOrEqual = TheProperty+ "debe ser igual o mayor  a {1}";
            public const string Error_LessThanOrEqual = TheProperty+ "debe ser igual o menor a {1}";
            public const string Error_LessThan = TheProperty+"debe ser menor a {1}";
            public const string Error_InvalidValue = TheProperty+"es invalida";
            public const string Error_NotFound = "no se encontro {0}";
            public const string Error_InvalidFormat = "El formato de {0} no es valido";


            public const string Rule_ReservationAlreadyCancelled = "La reserva ya se encuentra cancelada";
            public const string Rule_ReservationNotConfirmed = "Solo se pueden cancelar reservas confirmadas";
            public const string Rule_ReservationAlreadyConfirmed = "La reserva ya se encuentra confirmada";
            public const string Rule_EventWeekendStartTimeLimit = "Los Eventos en fin de semana no pueden iniciar después de las 22:00";
            public const string Rule_EventCapacityLimit = "La Capacidad Maxima del Evento debe ser menor o igual a la Capacidad del Lugar seleccionado";
            public const string Rule_EventScheduleOverlap = "Ya existe un Evento activo en este Lugar con horarios superpuestos";
            public const string Rule_ReservationTooCloseToStart = "No se permiten reservas para eventos que inicien en menos de 1 hora";
            public const string Rule_InsufficientCapacity = "Solo hay {0} entradas disponibles";
            public const string Rule_MaxQuantityForLastDay = "Para eventos que inician en menos de 24 horas, el máximo es 5 entradas por transacción";
            public const string Rule_MaxQuantityForExpensiveEvent = "Para eventos con precio mayor a $100, el máximo es 10 entradas por transacción";
            public const string Rule_CouldNotGenerateReservationCode = "No se pudo generar un codigo unico para la reserva. Intente nuevamente.";
        }

        public static class Infrastructure
        {
            public const string Error_InitConfiguration = "Error al inicializar, revise la configuracion relacionada a {0}";
            public const string Error_Internal = "Error interno";
            public const string Error_BadRequest = "Solicitud invalida";
            public const string Error_Unauthorized = "No autorizado";
            public const string Error_NotFound = "Recurso no encontrado";
            public const string Error_Conflict = "Conflicto en la operacion";
        }

        public static class User
        {
            public const string Error_Credentials = "Credenciales Invalidas";
            public const string Error_RevokedToken = "Token revocado";
            public const string Error_InvalidToken = "Token invalido";
        }

    }
}
