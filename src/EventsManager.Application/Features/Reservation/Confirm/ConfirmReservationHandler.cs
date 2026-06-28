using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.Interfaces.Tools;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using EventsManager.Core.Common.Time;
using EventsManager.Core.Constants;
using EventsManager.Core.Enums;
using FluentValidation;

namespace EventsManager.Application.Features.Reservation.Confirm
{
    public class ConfirmReservationHandler : BaseUseCase<ConfirmReservationRequest, string>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IAlphaNumericCodeGenerator _codeGenerator;
        private readonly IEmailService _emailService;
        private readonly IDateTimeProvider _timeProvider;

        public ConfirmReservationHandler(IValidator<ConfirmReservationRequest> validator
                                        , IReservationRepository reservationRepository
                                        , IEventRepository eventRepository
                                        , IAlphaNumericCodeGenerator codeGenerator
                                        , IEmailService emailService
                                        , IDateTimeProvider timeProvider)
            : base(validator)
        {
            _reservationRepository = reservationRepository;
            _eventRepository = eventRepository;
            _codeGenerator = codeGenerator;
            _emailService = emailService;
            _timeProvider = timeProvider;
        }

        protected override async Task<Result<string>> OnExecute(ConfirmReservationRequest request)
        {
            return await GenerateConfirmationCode(request)
                        .BindAsync(ConfirmReservation);
        }

        private async Task<Result<ConfirmReservationRequest>> GenerateConfirmationCode(ConfirmReservationRequest request)
        {
            int limitTry = 5;
            bool generatedFlag = false;
            string tempCode = string.Empty;
            for (int i = 0; i < limitTry; i++)
            {
                tempCode = $"{SystemValues.Infrastructure.ReservationCodePrefix}{_codeGenerator.Generate(6)}";
                bool exists = await _reservationRepository.ExistsByCodeAsync(tempCode);

                if (!exists)
                {
                    generatedFlag = true;
                    break;
                }
            }

            if (!generatedFlag)
                return Result.Failure<ConfirmReservationRequest>(SystemMessages.Validations.Rule_CouldNotGenerateReservationCode);

            return ConfirmReservationRequest.From(request.ReservationId, tempCode);
        }

        private async Task<Result<string>> ConfirmReservation(ConfirmReservationRequest request)
        {
            var tempReservation = await _reservationRepository.GetByIdAsync(request.ReservationId);
            if (tempReservation is null)
                return Result.Failure<string>(string.Format(SystemMessages.Validations.Error_NotFound, SystemValues.PropertyNames.Reservation));

            if (tempReservation.Status == ReservationStatus.Cancelled)
                return Result.Failure<string>(SystemMessages.Validations.Rule_ReservationAlreadyCancelled);

            if (tempReservation.Status == ReservationStatus.Confirmed)
                return Result.Failure<string>(SystemMessages.Validations.Rule_ReservationAlreadyConfirmed);

            var now = _timeProvider.GetNowColombia();

            tempReservation.Status = Core.Enums.ReservationStatus.Confirmed;
            tempReservation.ReservationCode = request.ConfirmationCode;
            _reservationRepository.Update(tempReservation);
            await _reservationRepository.SaveChangesAsync();

            var tempEvent = await _eventRepository.GetByIdAsync(tempReservation.EventId);

            var emailMessage = new EmailMessage(
                To: tempReservation.BuyerEmail,
                Subject: $"Confirmación de reserva - {tempEvent!.Title}",
                Body: $@"
                <h1>¡Reserva confirmada!</h1>
                <p>Hola {tempReservation.BuyerName},</p>
                <p>Tu reserva para <strong>{tempEvent.Title}</strong> ha sido confirmada.</p>
                <p>Usa el código de reserva para consultar la información o cancelar la reserva en el sitio web.</p>
                <p><strong>Código de reserva:</strong> {tempReservation.ReservationCode}</p>
                <p><strong>Cantidad:</strong> {tempReservation.Quantity} entradas</p>
                <p><strong>Fecha:</strong> {tempEvent.StartDate:dd/MM/yyyy HH:mm}</p>
                <p><strong>Confirmado:</strong> {now:dd/MM/yyyy HH:mm}</p>
            ");

            await _emailService.SendAsync(emailMessage);
            return request.ConfirmationCode!;
        }

    }
}
