namespace EventsManager.Core.Constants
{
    public static class SystemMessages
    {

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
