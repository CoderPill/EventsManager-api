using EventsManager.Api.Extensions;
using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.Event;
using EventsManager.Application.Features.Event.Add;
using EventsManager.Application.Features.Event.GetOccupationReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsManager.Api.Controllers
{
    /// <summary>
    /// Gestión de eventos.
    /// </summary>
    /// <remarks>
    /// GET /api/Event: público, lista eventos con su Venue incluido.
    /// GET /api/Event/occupationReport: requiere auth, reporte de ocupación.
    /// POST /api/Event: requiere auth, crea evento.
    /// Respuestas usan Result&lt;T&gt;: { isSuccess, value, errors }.
    /// </remarks>
    public class EventsController : BaseApiController
    {
        private readonly EventUseCases _eventUseCases;
        public EventsController(EventUseCases eventUseCases)
        {
            _eventUseCases = eventUseCases;
        }
        /// <summary>
        /// Obtiene todos los eventos con su lugar (venue) incluido.
        /// </summary>
        /// <remarks>
        /// Endpoint público. No requiere autenticación.
        /// Cada evento incluye su Venue anidado (Name, Capacity, City, CreationDate).
        /// </remarks>
        /// <response code="200">Lista de eventos en Result&lt;List&lt;EventDTO&gt;&gt;. Cada EventDTO incluye VenueDto.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<List<EventDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            return await _eventUseCases.GetEvents.Execute(Unit.Value)
                .ToActionResult();
        }
        /// <summary>
        /// Obtiene reporte de ocupación de un evento (entradas vendidas, disponibles, ingresos).
        /// </summary>
        /// <param name="request">EventId (query parameter, int, requerido, &gt; 0).</param>
        /// <response code="200">Reporte en Result&lt;EventOccupationReportDto&gt;.value.</response>
        /// <response code="400">EventId inválido.</response>
        /// <response code="401">Sin token o token inválido.</response>
        /// <response code="404">Evento no encontrado.</response>
        [HttpGet("occupationReport")]
        [ProducesResponseType(typeof(Result<EventOccupationReportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOccupationReport([FromQuery] GetOccupationReportRequest request)
        {
            return await _eventUseCases.GetOccupationReport.Execute(request)
                .ToActionResult();
        }
        /// <summary>
        /// Crea un nuevo evento.
        /// </summary>
        /// <param name="request">
        /// Datos del evento:
        /// <list type="bullet">
        /// <item><strong>Title</strong> (string, requerido, 5-100 caracteres): título del evento.</item>
        /// <item><strong>Description</strong> (string, requerido, 10-500 caracteres): descripción detallada.</item>
        /// <item><strong>VenueId</strong> (int, requerido, &gt; 0): ID del lugar existente.</item>
        /// <item><strong>MaxCapacity</strong> (int, requerido, &gt; 0): capacidad máxima (debe ser ≤ capacidad del Venue).</item>
        /// <item><strong>StartDate</strong> (DateTime, requerido): fecha/hora inicio en futuro; si es fin de semana (sábado/domingo) debe ser ≤ 22:00.</item>
        /// <item><strong>EndDate</strong> (DateTime, requerido): fecha/hora fin, obligatoriamente &gt; StartDate.</item>
        /// <item><strong>Price</strong> (decimal, requerido, &gt; 0): precio por entrada.</item>
        /// <item><strong>EventType</strong> (enum, requerido): Conferencia=1, Taller=2, Concierto=3.</item>
        /// </list>
        /// </param>
        /// <response code="200">Evento creado en Result&lt;EventDTO&gt;.value (incluye Id, Status=Activo, CreationDate, Venue).</response>
        /// <response code="400">Validaciones fallidas: Title 5-100, Description 10-500, VenueId&gt;0, MaxCapacity&gt;0, StartDate futuro + regla fin de semana, EndDate&gt;StartDate, Price&gt;0, EventType enum válido. Reglas de negocio: Venue no encontrado, capacidad excede venue, solape de horarios en mismo venue.</response>
        /// <response code="401">Sin token o token inválido.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Result<EventDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create(AddEventRequest request)
        {
            return await _eventUseCases.AddEvent.Execute(request)
                .ToActionResult();
        }
    }
}