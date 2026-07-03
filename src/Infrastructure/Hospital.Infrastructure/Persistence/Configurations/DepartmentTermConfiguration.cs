using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Persistence.Configurations;

public class DepartmentTermConfiguration : IEntityTypeConfiguration<DepartmentTerm>
{
    public void Configure(EntityTypeBuilder<DepartmentTerm> builder)
    {
        builder.HasIndex(dt => new { dt.DepartmentId, dt.Name }).IsUnique().HasFilter("\"IsDeleted\" = false");

        builder.HasOne(dt => dt.Department)
               .WithMany(dept => dept.DepartmentTerms)
               .HasForeignKey(dt => dt.DepartmentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
