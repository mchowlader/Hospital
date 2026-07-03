using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Persistence.Configurations;

public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.HasIndex(urt => urt.Token).IsUnique().HasFilter("\"IsDeleted\" = false");

        builder.HasOne(urt => urt.User)
               .WithMany()
               .HasForeignKey(urt => urt.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
