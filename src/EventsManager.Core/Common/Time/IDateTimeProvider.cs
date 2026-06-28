namespace EventsManager.Core.Common.Time;

public interface IDateTimeProvider
{
    /// <summary>
    /// Hora actual en zona horaria de Colombia (UTC-5, sin horario de verano).
    /// Úsalo para toda la lógica de negocio: validaciones, reglas temporales, timestamps de dominio.
    /// </summary>
    DateTime GetNowColombia();

    /// <summary>
    /// Convierte un DateTimeOffset (típicamente UTC) a hora Colombia.
    /// Útil cuando recibís timestamps externos (APIs, DB, logs) y necesitás normalizarlos.
    /// </summary>
    DateTime GetNowColombia(DateTimeOffset utcDateTime);

    /// <summary>
    /// UTC real del sistema. Solo para librerías que lo exijan (JWT, EF Core, logging, HTTP clients).
    /// NO usar para lógica de negocio.
    /// </summary>
    DateTime GetUtcNow();

    /// <summary>
    /// Delay asíncrono controlable (para tests poder adelantar tiempo).
    /// </summary>
    Task Delay(TimeSpan delay, CancellationToken cancellationToken = default);
}