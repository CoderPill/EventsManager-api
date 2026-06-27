using EventsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventsManager.Infrastructure.Persistence.Features.Reservation
{
    public class ReservationConfiguration : IEntityTypeConfiguration<ReservationEntity>
    {
        public void Configure(EntityTypeBuilder<ReservationEntity> builder)
        {
            builder.ToTable("Reservations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.CreationDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(x => x.EventId)
                .IsRequired();

            builder.Property(x => x.BuyerName)
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(x => x.BuyerEmail)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.ReservationCode)
                .HasMaxLength(16);

            builder.Property(x => x.CancelDate)
                .HasColumnType("datetime2");

            builder.Property(x => x.HasPenalty)
                .IsRequired();

            builder.HasOne(x => x.Event)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.EventId);

            builder.HasIndex(x => x.BuyerEmail);

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => x.ReservationCode)
                .IsUnique()
                .HasFilter("[ReservationCode] IS NOT NULL");
        }
    }
}
