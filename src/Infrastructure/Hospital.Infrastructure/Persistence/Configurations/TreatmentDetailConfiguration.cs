using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Persistence.Configurations;

public class TreatmentDetailConfiguration : IEntityTypeConfiguration<TreatmentDetail>
{
    public void Configure(EntityTypeBuilder<TreatmentDetail> builder)
    {
        builder.HasOne(td => td.TreatmentHistory)
               .WithMany(th => th.TreatmentDetails)
               .HasForeignKey(td => td.TreatmentHistoryId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(td => td.DepartmentTerm)
               .WithMany()
               .HasForeignKey(td => td.DepartmentTermId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
