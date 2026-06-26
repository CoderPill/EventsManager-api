using EventsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Infrastructure.Persistence.Features.Venue
{
    internal class VenueConfiguration : IEntityTypeConfiguration<VenueEntity>
    {
        public void Configure(EntityTypeBuilder<VenueEntity> builder)
        {
            builder.ToTable("Venues");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.CreateDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

            builder.Property(x => x.Capacity)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
