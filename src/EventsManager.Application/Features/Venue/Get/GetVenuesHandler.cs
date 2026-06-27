using EventsManager.Application.Common.DTOs;
using EventsManager.Application.Common.Interfaces.Persistence;
using EventsManager.Application.Common.ResultPattern;
using EventsManager.Application.Common.UseCases;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Application.Features.Venue.Get
{
    public class GetVenuesHandler : BaseUseCase<Unit, List<VenueDto>>
    {
        private readonly IVenueRepository _venueRepository;
        public GetVenuesHandler(IValidator<Unit> validator, IVenueRepository venueRepository) : base(validator)
        {
            _venueRepository = venueRepository;
        }

        protected override async Task<Result<List<VenueDto>>> OnExecute(Unit request)
        {
            var venues = await _venueRepository.GetAllAsync();
            return venues.Select(v => v.ToDto()).ToList();
        }
    }
}
