using EventsManager.Api.Extensions;
using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Features.Reservation;
using EventsManager.Application.Features.Reservation.Add;
using EventsManager.Application.Features.Reservation.Cancel;
using EventsManager.Application.Features.Reservation.Confirm;
using EventsManager.Application.Features.Reservation.GetByReservationCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsManager.Api.Controllers
{
    public class ReservationsController : BaseApiController
    {
        private readonly ReservationUseCases _reservationUseCases;
        public ReservationsController(ReservationUseCases reservationUseCases)
        {
            _reservationUseCases = reservationUseCases;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await _reservationUseCases.GetReservations.Execute(Unit.Value)
                .ToActionResult();
        }
        [HttpGet("getByReservationCode")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByReservationCode([FromQuery] GetByReservationCodeRequest request)
        {
            return await _reservationUseCases.GetByReservationCode.Execute(request)
                .ToActionResult();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Add(AddReservationRequest request)
        {
            return await _reservationUseCases.AddReservation.Execute(request)
                .ToActionResult();
        }

        [HttpPut("cancel")]
        [AllowAnonymous]
        public async Task<IActionResult> Cancel(CancelReservationRequest request)
        {
            return await _reservationUseCases.CancelReservation.Execute(request)
                .ToActionResult(System.Net.HttpStatusCode.NoContent);
        }

        [HttpPut("confirm")]
        public async Task<IActionResult> Confirm(ConfirmReservationRequest request)
        {
            return await _reservationUseCases.ConfirmReservation.Execute(request)
                .ToActionResult();
        }
    }
}
