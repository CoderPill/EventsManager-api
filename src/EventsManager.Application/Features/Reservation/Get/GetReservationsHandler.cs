using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.Reservation.Get
{
    public class GetReservationsHandler:BaseUseCase<Unit, List<ReservationDTO>>
    {
        private readonly IReservationRepository _reservationRepository;
        public GetReservationsHandler(IValidator<Unit> validator, IReservationRepository reservationRepository) : base(validator)
        {
            _reservationRepository = reservationRepository;
        }
        protected override async Task<Result<List<ReservationDTO>>> OnExecute(Unit request)
        {
            var reservations = await _reservationRepository.GetAllAsync();
            return reservations.Select(r => r.ToDto()).ToList();
        }
    }
}
