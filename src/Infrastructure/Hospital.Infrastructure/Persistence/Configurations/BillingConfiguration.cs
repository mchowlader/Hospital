using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Persistence.Configurations;

public class BillingConfiguration : IEntityTypeConfiguration<Billing>
{
    public void Configure(EntityTypeBuilder<Billing> builder)
    {
        builder.HasOne(b => b.Patient)
               .WithMany()
               .HasForeignKey(b => b.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.TreatmentHistory)
               .WithOne()
               .HasForeignKey<Billing>(b => b.TreatmentHistoryId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
