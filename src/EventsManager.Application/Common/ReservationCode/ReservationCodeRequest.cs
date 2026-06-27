namespace EventsManager.Application.Common.ReservationCode
{
    public abstract record ReservationCodeRequest(string BuyerEmail, string ReservationCode);
}
