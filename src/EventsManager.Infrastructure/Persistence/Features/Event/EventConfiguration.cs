using EventsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventsManager.Infrastructure.Persistence.Features.Event
{
    public class EventConfiguration : IEntityTypeConfiguration<EventEntity>
    {
        public void Configure(EntityTypeBuilder<EventEntity> builder)
        {
            builder.ToTable("Events");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.CreateDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.VenueId)
                .IsRequired();

            builder.Property(x => x.MaxCapacity)
                .IsRequired();

            builder.Property(x => x.StartDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(x => x.EndDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(x => x.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Type)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasOne(x => x.Venue)
                .WithMany()
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.VenueId);

            builder.HasIndex(x => x.Type);

            builder.HasIndex(x => x.StartDate);
        }
    }
}
