using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Persistence.Configurations;

public class TreatmentHistoryConfiguration : IEntityTypeConfiguration<TreatmentHistory>
{
    public void Configure(EntityTypeBuilder<TreatmentHistory> builder)
    {
        builder.HasOne(th => th.Patient)
               .WithMany()
               .HasForeignKey(th => th.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(th => th.Department)
               .WithMany()
               .HasForeignKey(th => th.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(th => th.Doctor)
               .WithMany()
               .HasForeignKey(th => th.DoctorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
