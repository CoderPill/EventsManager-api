using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.Reservation.Cancel
{
    public record CancelReservationRequest(string ReservationCode)
    {
        public static CancelReservationRequest From(string reservationCode)
        {
            return new(reservationCode);
        }
    }
}
