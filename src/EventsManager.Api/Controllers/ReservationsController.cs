using EventsManager.Api.Extensions;
using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.Reservation;
using EventsManager.Application.Features.Reservation.Add;
using EventsManager.Application.Features.Reservation.Cancel;
using EventsManager.Application.Features.Reservation.Confirm;
using EventsManager.Application.Features.Reservation.GetByReservationCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsManager.Api.Controllers
{
    /// <summary>
    /// Gestión de reservas de eventos.
    /// </summary>
    /// <remarks>
    /// GET /api/Reservations: requiere autenticación (lista todas).
    /// GET /api/Reservations/getByReservationCode: público, busca por email+código.
    /// POST /api/Reservations: público, crea reserva (estado PendientePago).
    /// PUT /api/Reservations/cancel: público, cancela reserva confirmada.
    /// PUT /api/Reservations/confirm: requiere autenticación, confirma reserva y genera código.
    /// Respuestas usan Result&lt;T&gt;: { isSuccess, value, errors }.
    /// </remarks>
    public class ReservationsController : BaseApiController
    {
        private readonly ReservationUseCases _reservationUseCases;
        public ReservationsController(ReservationUseCases reservationUseCases)
        {
            _reservationUseCases = reservationUseCases;
        }
        /// <summary>
        /// Obtiene todas las reservas (requiere autenticación).
        /// </summary>
        /// <response code="200">Lista de reservas.</response>
        /// <response code="401">Sin token o token inválido.</response>
        [HttpGet]
        [ProducesResponseType(typeof(Result<List<ReservationDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<List<Unit>>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            return await _reservationUseCases.GetReservations.Execute(Unit.Value)
                .ToActionResult();
        }
        /// <summary>
        /// Busca una reserva por email y código de reserva.
        /// </summary>
        /// <param name="request">
        /// Parámetros de búsqueda:
        /// <list type="bullet">
        /// <item><strong>BuyerEmail</strong> (string, query, requerido, máx 64 caracteres, formato email válido): email del comprador.</item>
        /// <item><strong>ReservationCode</strong> (string, query, requerido, formato EV-XXXXXX): código de 6 caracteres alfanuméricos precedido por "EV-".</item>
        /// </list>
        /// </param>
        /// <response code="200">Reserva con evento incluido en Result&lt;ReservationDTO&gt;.value.</response>
        /// <response code="400">Email o código inválidos (formato, longitud).</response>
        /// <response code="404">Reserva no encontrada.</response>
        [HttpGet("getByReservationCode")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<ReservationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByReservationCode([FromQuery] GetByReservationCodeRequest request)
        {
            return await _reservationUseCases.GetByReservationCode.Execute(request)
                .ToActionResult();
        }
        /// <summary>
        /// Crea una nueva reserva (estado inicial: PendientePago).
        /// </summary>
        /// <param name="request">
        /// Datos de la reserva:
        /// <list type="bullet">
        /// <item><strong>EventId</strong> (int, requerido, &gt; 0): ID del evento existente.</item>
        /// <item><strong>Quantity</strong> (int, requerido, ≥ 1): cantidad de entradas solicitadas.</item>
        /// <item><strong>BuyerName</strong> (string, requerido, 1-32 caracteres): nombre del comprador.</item>
        /// <item><strong>BuyerEmail</strong> (string, requerido, máx 64 caracteres, formato email válido): email del comprador.</item>
        /// </list>
        /// </param>
        /// <response code="200">Reserva creada en Result&lt;ReservationDTO&gt;.value (sin ReservationCode hasta confirmar, Status=PendientePago).</response>
        /// <response code="400">Validaciones de entrada: EventId&gt;0, Quantity≥1, BuyerName 1-32, BuyerEmail email válido 1-64. Reglas de negocio: evento no encontrado, evento inactivo, evento ya iniciado, evento inicia en &lt;1 hora, capacidad insuficiente, límite 5 entradas si evento inicia en &lt;24h, límite 10 entradas si precio&gt;$100.</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<ReservationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(AddReservationRequest request)
        {
            return await _reservationUseCases.AddReservation.Execute(request)
                .ToActionResult();
        }
        /// <summary>
        /// Cancela una reserva confirmada.
        /// </summary>
        /// <param name="request">
        /// Datos para cancelación:
        /// <list type="bullet">
        /// <item><strong>BuyerEmail</strong> (string, requerido, máx 64 caracteres, formato email válido): email del comprador.</item>
        /// <item><strong>ReservationCode</strong> (string, requerido, formato EV-XXXXXX): código de reserva a cancelar.</item>
        /// </list>
        /// </param>
        /// <response code="204">Cancelación exitosa (sin contenido).</response>
        /// <response code="400">Email/código inválidos. Reglas: reserva ya cancelada, reserva no está confirmada (solo confirmadas se cancelan), evento no encontrado.</response>
        /// <response code="404">Reserva no encontrada.</response>
        [HttpPut("cancel")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(CancelReservationRequest request)
        {
            return await _reservationUseCases.CancelReservation.Execute(request)
                .ToActionResult();
        }
        /// <summary>
        /// Confirma una reserva pendiente y genera código de reserva (requiere autenticación).
        /// </summary>
        /// <param name="request">
        /// <list type="bullet">
        /// <item><strong>ReservationId</strong> (int, requerido, &gt; 0): ID de la reserva a confirmar.</item>
        /// <item><strong>ConfirmationCode</strong> (string, opcional): código generado internamente (se ignora si se envía).</item>
        /// </list>
        /// </param>
        /// <response code="200">Código de reserva generado en Result&lt;string&gt;.value (formato EV-XXXXXX).</response>
        /// <response code="400">ReservationId inválido. Reglas: reserva ya cancelada, reserva ya confirmada, no se pudo generar código único tras 5 intentos.</response>
        /// <response code="401">Sin token o token inválido.</response>
        /// <response code="404">Reserva no encontrada.</response>
        [HttpPut("confirm")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Confirm(ConfirmReservationRequest request)
        {
            return await _reservationUseCases.ConfirmReservation.Execute(request)
                .ToActionResult();
        }
    }
}