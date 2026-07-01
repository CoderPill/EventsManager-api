using EventsManager.Core.Entities;
using EventsManager.Core.Enums;

namespace EventsManager.Application.Tests.Common;

public sealed class VenueEntityBuilder
{
    private int _id = 1;
    private string _name = "Test Venue";
    private int _capacity = 100;
    private string _city = "Bogotá";
    private DateTime _creationDate = FakeTimeProvider.ColombiaNow;

    public VenueEntityBuilder WithId(int id) { _id = id; return this; }
    public VenueEntityBuilder WithName(string name) { _name = name; return this; }
    public VenueEntityBuilder WithCapacity(int capacity) { _capacity = capacity; return this; }
    public VenueEntityBuilder WithCity(string city) { _city = city; return this; }
    public VenueEntityBuilder WithCreationDate(DateTime date) { _creationDate = date; return this; }

    public VenueEntity Build() => new()
    {
        Id = _id,
        Name = _name,
        Capacity = _capacity,
        City = _city,
        CreationDate = _creationDate
    };
}

public sealed class EventEntityBuilder
{
    private int _id = 1;
    private int _venueId = 1;
    private string _title = "Test Event";
    private string _description = "Description";
    private DateTime _startDate = FakeTimeProvider.ColombiaNow.AddDays(7).AddHours(13); // 2026-06-22 18:00
    private DateTime _endDate = FakeTimeProvider.ColombiaNow.AddDays(7).AddHours(16);   // 2026-06-22 21:00
    private int _maxCapacity = 50;
    private decimal _price = 50m;
    private EventType _type = EventType.Concierto;
    private bool _isActive = true;
    private DateTime _creationDate = FakeTimeProvider.ColombiaNow;

    public EventEntityBuilder WithId(int id) { _id = id; return this; }
    public EventEntityBuilder WithVenueId(int venueId) { _venueId = venueId; return this; }
    public EventEntityBuilder WithTitle(string title) { _title = title; return this; }
    public EventEntityBuilder WithDescription(string description) { _description = description; return this; }
    public EventEntityBuilder WithStartDate(DateTime startDate) { _startDate = startDate; return this; }
    public EventEntityBuilder WithEndDate(DateTime endDate) { _endDate = endDate; return this; }
    public EventEntityBuilder WithDates(DateTime start, DateTime end) { _startDate = start; _endDate = end; return this; }
    public EventEntityBuilder WithMaxCapacity(int maxCapacity) { _maxCapacity = maxCapacity; return this; }
    public EventEntityBuilder WithPrice(decimal price) { _price = price; return this; }
    public EventEntityBuilder WithType(EventType type) { _type = type; return this; }
    public EventEntityBuilder WithIsActive(bool isActive) { _isActive = isActive; return this; }
    public EventEntityBuilder Inactive() { _isActive = false; return this; }
    public EventEntityBuilder WithCreationDate(DateTime date) { _creationDate = date; return this; }

    public EventEntity Build() => new()
    {
        Id = _id,
        VenueId = _venueId,
        Title = _title,
        Description = _description,
        StartDate = _startDate,
        EndDate = _endDate,
        MaxCapacity = _maxCapacity,
        Price = _price,
        Type = _type,
        IsActive = _isActive,
        CreationDate = _creationDate
    };
}

public sealed class ReservationEntityBuilder
{
    private int _id = 1;
    private int _eventId = 1;
    private string _buyerName = "Juan Pérez";
    private string _buyerEmail = "juan@test.com";
    private int _quantity = 2;
    private ReservationStatus _status = ReservationStatus.PendientePago;
    private string? _reservationCode = null;
    private DateTime? _cancelDate = null;
    private bool _hasPenalty = false;
    private DateTime _creationDate = FakeTimeProvider.ColombiaNow;

    public ReservationEntityBuilder WithId(int id) { _id = id; return this; }
    public ReservationEntityBuilder WithEventId(int eventId) { _eventId = eventId; return this; }
    public ReservationEntityBuilder WithBuyerName(string buyerName) { _buyerName = buyerName; return this; }
    public ReservationEntityBuilder WithBuyerEmail(string buyerEmail) { _buyerEmail = buyerEmail; return this; }
    public ReservationEntityBuilder WithQuantity(int quantity) { _quantity = quantity; return this; }
    public ReservationEntityBuilder WithStatus(ReservationStatus status) { _status = status; return this; }
    public ReservationEntityBuilder WithReservationCode(string? code) { _reservationCode = code; return this; }
    public ReservationEntityBuilder WithCancelDate(DateTime? date) { _cancelDate = date; return this; }
    public ReservationEntityBuilder WithHasPenalty(bool hasPenalty) { _hasPenalty = hasPenalty; return this; }
    public ReservationEntityBuilder Confirmed() { _status = ReservationStatus.Confirmada; return this; }
    public ReservationEntityBuilder Cancelled() { _status = ReservationStatus.Cancelada; return this; }

    public ReservationEntity Build() => new()
    {
        Id = _id,
        EventId = _eventId,
        BuyerName = _buyerName,
        BuyerEmail = _buyerEmail,
        Quantity = _quantity,
        Status = _status,
        ReservationCode = _reservationCode,
        CancelDate = _cancelDate,
        HasPenalty = _hasPenalty,
        CreationDate = _creationDate
    };
}

public sealed class UserEntityBuilder
{
    private int _id = 1;
    private string _username = "testuser";
    private string _passwordHash = "hashed";
    private UserRole _role = UserRole.Admin;
    private bool _isActive = true;
    private DateTime _creationDate = FakeTimeProvider.ColombiaNow;

    public UserEntityBuilder WithId(int id) { _id = id; return this; }
    public UserEntityBuilder WithUsername(string username) { _username = username; return this; }
    public UserEntityBuilder WithPasswordHash(string passwordHash) { _passwordHash = passwordHash; return this; }
    public UserEntityBuilder WithRole(UserRole role) { _role = role; return this; }
    public UserEntityBuilder WithIsActive(bool isActive) { _isActive = isActive; return this; }
    public UserEntityBuilder Inactive() { _isActive = false; return this; }

    public UserEntity Build() => new()
    {
        Id = _id,
        Username = _username,
        PasswordHash = _passwordHash,
        Role = _role,
        IsActive = _isActive,
        CreationDate = _creationDate
    };
}
