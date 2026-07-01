using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Features.Event.Add;
using EventsManager.Application.Features.Event.GetOccupationReport;
using EventsManager.Application.Features.Reservation.Add;
using EventsManager.Application.Features.Reservation.Cancel;
using EventsManager.Application.Features.Reservation.Confirm;
using EventsManager.Application.Features.Reservation.GetByReservationCode;
using EventsManager.Application.Features.User.Login;
using EventsManager.Application.Features.User.Logout;
using EventsManager.Application.Features.Venue.Add;
using EventsManager.Core.Enums;

namespace EventsManager.Application.Tests.Common;

public sealed class AddVenueRequestBuilder
{
    private string _name = "Test Venue";
    private int _capacity = 100;
    private string _city = "Bogotá";

    public AddVenueRequestBuilder WithName(string name) { _name = name; return this; }
    public AddVenueRequestBuilder WithCapacity(int capacity) { _capacity = capacity; return this; }
    public AddVenueRequestBuilder WithCity(string city) { _city = city; return this; }

    public AddVenueRequest Build() => new(_name, _capacity, _city);
}

public sealed class GetVenuesRequestBuilder
{
    public Unit Build() => Unit.Value;
}

public sealed class LoginRequestBuilder
{
    private string _username = "testuser";
    private string _password = "password123";

    public LoginRequestBuilder WithUsername(string username) { _username = username; return this; }
    public LoginRequestBuilder WithPassword(string password) { _password = password; return this; }

    public LoginRequest Build() => new(_username, _password);
}

public sealed class LogoutRequestBuilder
{
    private string _jti = "test-jti-123";
    private DateTimeOffset _expiresDate = new(FakeTimeProvider.ColombiaNow.AddHours(1), TimeSpan.FromHours(-5));

    public LogoutRequestBuilder WithJti(string jti) { _jti = jti; return this; }
    public LogoutRequestBuilder WithExpiresDate(DateTimeOffset expiresDate) { _expiresDate = expiresDate; return this; }

    public LogoutRequest Build() => new(_jti, _expiresDate);
}

public sealed class AddEventRequestBuilder
{
    private string _title = "Concierto";
    private string _description = "Description";
    private int _venueId = 1;
    private int _maxCapacity = 50;
    private DateTime _startDate = FakeTimeProvider.ColombiaNow.AddDays(7).AddHours(13); // 2026-06-22 18:00
    private DateTime _endDate = FakeTimeProvider.ColombiaNow.AddDays(7).AddHours(16);   // 2026-06-22 21:00
    private decimal _price = 50m;
    private EventType _eventType = EventType.Concierto;

    public AddEventRequestBuilder WithTitle(string title) { _title = title; return this; }
    public AddEventRequestBuilder WithDescription(string description) { _description = description; return this; }
    public AddEventRequestBuilder WithVenueId(int venueId) { _venueId = venueId; return this; }
    public AddEventRequestBuilder WithMaxCapacity(int maxCapacity) { _maxCapacity = maxCapacity; return this; }
    public AddEventRequestBuilder WithStartDate(DateTime startDate) { _startDate = startDate; return this; }
    public AddEventRequestBuilder WithEndDate(DateTime endDate) { _endDate = endDate; return this; }
    public AddEventRequestBuilder WithDates(DateTime start, DateTime end) { _startDate = start; _endDate = end; return this; }
    public AddEventRequestBuilder WithPrice(decimal price) { _price = price; return this; }
    public AddEventRequestBuilder WithEventType(EventType eventType) { _eventType = eventType; return this; }

    public AddEventRequest Build() => new(_title, _description, _venueId, _maxCapacity, _startDate, _endDate, _price, _eventType);
}

public sealed class GetEventsRequestBuilder
{
    public Unit Build() => Unit.Value;
}

public sealed class GetOccupationReportRequestBuilder
{
    private int _eventId = 1;

    public GetOccupationReportRequestBuilder WithEventId(int eventId) { _eventId = eventId; return this; }

    public GetOccupationReportRequest Build() => new(_eventId);
}

public sealed class AddReservationRequestBuilder
{
    private int _eventId = 1;
    private int _quantity = 2;
    private string _buyerName = "Juan Pérez";
    private string _buyerEmail = "juan@test.com";

    public AddReservationRequestBuilder WithEventId(int eventId) { _eventId = eventId; return this; }
    public AddReservationRequestBuilder WithQuantity(int quantity) { _quantity = quantity; return this; }
    public AddReservationRequestBuilder WithBuyerName(string buyerName) { _buyerName = buyerName; return this; }
    public AddReservationRequestBuilder WithBuyerEmail(string buyerEmail) { _buyerEmail = buyerEmail; return this; }

    public AddReservationRequest Build() => new(_eventId, _quantity, _buyerName, _buyerEmail);
}

public sealed class GetReservationsRequestBuilder
{
    public Unit Build() => Unit.Value;
}

public sealed class GetByReservationCodeRequestBuilder
{
    private string _buyerEmail = "juan@test.com";
    private string _reservationCode = "EV-ABC123";

    public GetByReservationCodeRequestBuilder WithBuyerEmail(string buyerEmail) { _buyerEmail = buyerEmail; return this; }
    public GetByReservationCodeRequestBuilder WithReservationCode(string reservationCode) { _reservationCode = reservationCode; return this; }

    public GetByReservationCodeRequest Build() => new(_buyerEmail, _reservationCode);
}

public sealed class CancelReservationRequestBuilder
{
    private string _buyerEmail = "juan@test.com";
    private string _reservationCode = "EV-ABC123";

    public CancelReservationRequestBuilder WithBuyerEmail(string buyerEmail) { _buyerEmail = buyerEmail; return this; }
    public CancelReservationRequestBuilder WithReservationCode(string reservationCode) { _reservationCode = reservationCode; return this; }

    public CancelReservationRequest Build() => new(_buyerEmail, _reservationCode);
}

public sealed class ConfirmReservationRequestBuilder
{
    private int _reservationId = 1;
    private string? _confirmationCode = null;

    public ConfirmReservationRequestBuilder WithReservationId(int reservationId) { _reservationId = reservationId; return this; }
    public ConfirmReservationRequestBuilder WithConfirmationCode(string? code) { _confirmationCode = code; return this; }

    public ConfirmReservationRequest Build() => new(_reservationId, _confirmationCode);
}
