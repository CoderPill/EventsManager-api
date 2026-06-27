using EventsManager.Application.Features.Event;
using EventsManager.Application.Features.Reservation.Add;
using EventsManager.Core.Entities;
using EventsManager.Core.Enums;

namespace EventsManager.Application.Features.Reservation
{
    public static class ReservationMapper
    {
        extension(ReservationEntity instance)
        {
            public ReservationDTO ToDto()
            {
                return ReservationDTO.From(instance.Id, instance.EventId, instance.BuyerName, instance.BuyerEmail, instance.Quantity, instance.Status, instance.ReservationCode, instance.CancelDate, instance.HasPenalty, instance.CreationDate);
            }
            public ReservationDTO ToDtoIncludeEvent()
            {
                return ReservationDTO.From(instance.Id, instance.EventId, instance.BuyerName, instance.BuyerEmail, instance.Quantity, instance.Status, instance.ReservationCode, instance.CancelDate, instance.HasPenalty, instance.CreationDate, instance.Event?.ToDto());
            }
        }
        extension(AddReservationRequest instance)
        {
            public ReservationEntity ToEntity()
            {
                return new()
                {
                    EventId = instance.EventId
                    ,
                    Quantity = instance.Quantity
                    ,
                    BuyerName = instance.BuyerName
                    ,
                    BuyerEmail = instance.BuyerEmail
                    ,
                    Status = ReservationStatus.PendingPayment
                };
            }
        }
    }
}
