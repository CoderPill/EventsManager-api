using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using FluentValidation;

namespace EventsManager.Application.Features.Venue.Add
{
    public class AddVenueHandler : BaseUseCase<AddVenueRequest, VenueDto>
    {
        private readonly IVenueRepository _venueRepository;
        public AddVenueHandler(IValidator<AddVenueRequest> validator, IVenueRepository venueRepository)
            : base(validator)
        {
            _venueRepository = venueRepository;
        }
        protected override async Task<Result<VenueDto>> OnExecute(AddVenueRequest request)
        {
            var tempEntity = request.ToVenueEntity();
            await _venueRepository.AddAsync(tempEntity);
            await _venueRepository.SaveChangesAsync();
            return tempEntity.ToDto();
        }
    }
}
