using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using EventsManager.Core.Constants;
using EventsManager.Core.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.Reservation.Cancel
{
    public class CancelReservationHandler : BaseUseCase<CancelReservationRequest, Unit>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IEventRepository _eventRepository;
        public CancelReservationHandler(IValidator<CancelReservationRequest> validator, IReservationRepository reservationRepository,IEventRepository eventRepository)
            :base(validator)
        {
            _reservationRepository = reservationRepository;
            _eventRepository = eventRepository;
        }
        protected override async Task<Result<Unit>> OnExecute(CancelReservationRequest request)
        {
            var reservation = await _reservationRepository.GetByCodeAsync(request.ReservationCode);
            if (reservation is null)
                return Result.Failure(string.Format(SystemMessages.Validations.Error_NotFound,SystemValues.PropertyNames.Reservation));


            if (reservation.Status == ReservationStatus.Cancelled)
                return Result.Failure(SystemMessages.Validations.Rule_ReservationAlreadyCancelled);


            if (reservation.Status != ReservationStatus.Confirmed)
                return Result.Failure(SystemMessages.Validations.Rule_ReservationNotConfirmed);


            var eventEntity = await _eventRepository.GetByIdAsync(reservation.EventId);
            if (eventEntity is null)
                return Result.Failure(string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Event));

            var now = DateTime.Now;
            var TowDays = 172800;
            var secondsUntilStart = (eventEntity.StartDate - now).TotalSeconds;

            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancelDate = now;
            reservation.HasPenalty = secondsUntilStart < TowDays;

            _reservationRepository.Update(reservation);
            await _reservationRepository.SaveChangesAsync();
            return  Result.Success();
        }
    }
}
