using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.Reservation.GetByReservationCode
{
    public record GetByReservationCodeRequest(string BuyerEmail,string ReservationCode)
    {
        public static GetByReservationCodeRequest From (string buyerEmail,string reservationCode)
        {
            return new(buyerEmail,reservationCode);
        }
    }
}
