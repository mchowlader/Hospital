using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Persistence.Configurations;

public class LandingPageSectionConfiguration : IEntityTypeConfiguration<LandingPageSection>
{
    public void Configure(EntityTypeBuilder<LandingPageSection> builder)
    {
        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Content)
            .IsRequired();

        builder.Property(s => s.SectionType)
            .IsRequired()
            .HasMaxLength(50);
    }
}
