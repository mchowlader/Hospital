using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Persistence.Configurations;

public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.HasIndex(up => new { up.UserId, up.PermissionId }).IsUnique().HasFilter("\"IsDeleted\" = false");

        builder.HasOne(up => up.User)
               .WithMany(u => u.UserPermissions)
               .HasForeignKey(up => up.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.Permission)
               .WithMany()
               .HasForeignKey(up => up.PermissionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
