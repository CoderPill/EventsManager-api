using EventsManager.Api.Extensions;
using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Features.Venue;
using EventsManager.Application.Features.Venue.Add;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsManager.Api.Controllers
{
    /// <summary>
    /// Gestión de lugares (venues) para eventos.
    /// </summary>
    /// <remarks>
    /// GET público para listar. POST requiere autenticación (organizadores/admin).
    /// Respuestas usan Result&lt;T&gt;: { isSuccess, value, errors }.
    /// </remarks>
    public class VenuesController : BaseApiController
    {
        private readonly VenueUseCases _venueUseCases;
        public VenuesController(VenueUseCases venueUseCases)
        {
            _venueUseCases = venueUseCases;
        }
        /// <summary>
        /// Obtiene todos los lugares registrados.
        /// </summary>
        /// <remarks>
        /// Endpoint público. No requiere autenticación.
        /// Útil para mostrar lista de lugares al crear eventos.
        /// </remarks>
        /// <response code="200">Lista de lugares en Result&lt;List&lt;VenueDto&gt;&gt;.value.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Result<List<VenueDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            return await _venueUseCases.GetVenues.Execute(Unit.Value)
                .ToActionResult();
        }
        /// <summary>
        /// Crea un nuevo lugar.
        /// </summary>
        /// <remarks>
        /// Datos del lugar:
        /// <list type="bullet">
        /// <item><strong>Name</strong> (string, requerido, máx 32 caracteres): nombre del lugar.</item>
        /// <item><strong>Capacity</strong> (int, requerido, &gt; 0): capacidad máxima de asistentes.</item>
        /// <item><strong>City</strong> (string, requerido, máx 32 caracteres): ciudad donde se ubica.</item>
        /// </list>
        /// </remarks>
        /// <param name="request">Datos del lugar: Name, Capacity, City.</param>
        /// <response code="200">Lugar creado en Result&lt;VenueDto&gt;.value (incluye Id y CreationDate).</response>
        /// <response code="400">Errores de validación: Name requerido/máx 32, Capacity &gt; 0, City requerido/máx 32.</response>
        /// <response code="401">Token JWT inválido o expirado.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Result<VenueDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create(AddVenueRequest request)
        {
            return await _venueUseCases.AddVenue.Execute(request)
                .ToActionResult();
        }
    }
}