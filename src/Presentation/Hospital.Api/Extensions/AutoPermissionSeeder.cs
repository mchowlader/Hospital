using Hospital.Api.Common;
using Hospital.Domain.Common;
using Hospital.Domain.Entities;
using Hospital.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Api.Extensions;

public static class AutoPermissionSeeder
{
    public static async Task SeedPermissionsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 1. Seed Roles
        var defaultRoles = new[] { "SuperAdmin", "Admin", "Doctor", "Staff", "Patient" };
        var existingRoles = await dbContext.Roles.ToListAsync();
        var rolesToCreate = defaultRoles
            .Where(r => !existingRoles.Any(er => er.Name.Equals(r, StringComparison.OrdinalIgnoreCase)))
            .Select(r => new Role { Name = r, Description = $"Default system {r} role" })
            .ToList();

        if (rolesToCreate.Any())
        {
            await dbContext.Roles.AddRangeAsync(rolesToCreate);
            await dbContext.SaveChangesAsync();
            existingRoles = await dbContext.Roles.ToListAsync();
        }

        var superAdminRole = existingRoles.First(r => r.Name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase));
        var adminRole = existingRoles.FirstOrDefault(r => r.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase));
        var doctorRole = existingRoles.FirstOrDefault(r => r.Name.Equals("Doctor", StringComparison.OrdinalIgnoreCase));
        var staffRole = existingRoles.FirstOrDefault(r => r.Name.Equals("Staff", StringComparison.OrdinalIgnoreCase));
        var patientRole = existingRoles.FirstOrDefault(r => r.Name.Equals("Patient", StringComparison.OrdinalIgnoreCase));

        // 2. Find all classes inheriting from BaseEntity in Domain assembly to seed permissions
        var domainAssembly = typeof(BaseEntity).Assembly;
        var entityTypes = domainAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(BaseEntity).IsAssignableFrom(t));

        var permissionsToSeed = new List<string>();
        foreach (var type in entityTypes)
        {
            var entityName = type.Name;
            // Skip intermediate relational entities if not directly controlled by permissions
            if (entityName.Equals("UserRole") || entityName.Equals("RolePermission") || 
                entityName.Equals("UserPermission") || entityName.Equals("UserRefreshToken"))
            {
                continue;
            }

            permissionsToSeed.Add($"{entityName}.View");
            permissionsToSeed.Add($"{entityName}.Create");
            permissionsToSeed.Add($"{entityName}.Edit");
            permissionsToSeed.Add($"{entityName}.Delete");
        }

        // Get existing permissions from DB
        var existingKeys = await dbContext.Permissions
            .AsNoTracking()
            .Select(p => p.Key)
            .ToListAsync();

        var newPermissions = permissionsToSeed
            .Except(existingKeys, StringComparer.OrdinalIgnoreCase)
            .Select(key => new Permission
            {
                Key = key,
                Description = $"Auto-seeded permission for {key.Split('.')[0]} {key.Split('.')[1]}"
            })
            .ToList();

        if (newPermissions.Any())
        {
            await dbContext.Permissions.AddRangeAsync(newPermissions);
            await dbContext.SaveChangesAsync();
        }

        var allPermissions = await dbContext.Permissions.ToListAsync();

        // 3. Map all permissions to SuperAdmin
        if (superAdminRole != null)
        {
            var existingSuperAdminPermissions = await dbContext.RolePermissions
                .Where(rp => rp.RoleId == superAdminRole.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var superAdminPermissionsToCreate = allPermissions
                .Where(p => !existingSuperAdminPermissions.Contains(p.Id))
                .Select(p => new RolePermission { RoleId = superAdminRole.Id, PermissionId = p.Id })
                .ToList();

            if (superAdminPermissionsToCreate.Any())
            {
                await dbContext.RolePermissions.AddRangeAsync(superAdminPermissionsToCreate);
                await dbContext.SaveChangesAsync();
            }
        }

        // 4. Map default permissions to Doctor role
        if (doctorRole != null)
        {
            var doctorPermissionKeys = new[]
            {
                "Patient.View", "Patient.Edit",
                "Appointment.View", "Appointment.Edit",
                "Doctor.View",
                "TreatmentHistory.View", "TreatmentHistory.Create", "TreatmentHistory.Edit",
                "TreatmentDetail.View", "TreatmentDetail.Create", "TreatmentDetail.Edit",
                "Department.View", "DepartmentTerm.View"
            };

            var existingDoctorPermissions = await dbContext.RolePermissions
                .Where(rp => rp.RoleId == doctorRole.Id)
                .Select(rp => rp.Permission.Key)
                .ToListAsync();

            var doctorPermissionsToCreate = allPermissions
                .Where(p => doctorPermissionKeys.Contains(p.Key, StringComparer.OrdinalIgnoreCase) && 
                            !existingDoctorPermissions.Contains(p.Key, StringComparer.OrdinalIgnoreCase))
                .Select(p => new RolePermission { RoleId = doctorRole.Id, PermissionId = p.Id })
                .ToList();

            if (doctorPermissionsToCreate.Any())
            {
                await dbContext.RolePermissions.AddRangeAsync(doctorPermissionsToCreate);
                await dbContext.SaveChangesAsync();
            }
        }

        // 5. Map default permissions to Staff role
        if (staffRole != null)
        {
            var staffPermissionKeys = new[]
            {
                "Patient.View", "Patient.Create", "Patient.Edit",
                "Appointment.View", "Appointment.Create", "Appointment.Edit",
                "Doctor.View", "Department.View", "DepartmentTerm.View",
                "Billing.View", "Billing.Create", "Billing.Edit"
            };

            var existingStaffPermissions = await dbContext.RolePermissions
                .Where(rp => rp.RoleId == staffRole.Id)
                .Select(rp => rp.Permission.Key)
                .ToListAsync();

            var staffPermissionsToCreate = allPermissions
                .Where(p => staffPermissionKeys.Contains(p.Key, StringComparer.OrdinalIgnoreCase) && 
                            !existingStaffPermissions.Contains(p.Key, StringComparer.OrdinalIgnoreCase))
                .Select(p => new RolePermission { RoleId = staffRole.Id, PermissionId = p.Id })
                .ToList();

            if (staffPermissionsToCreate.Any())
            {
                await dbContext.RolePermissions.AddRangeAsync(staffPermissionsToCreate);
                await dbContext.SaveChangesAsync();
            }
        }

        // 6. Map default permissions to Patient role
        if (patientRole != null)
        {
            var patientPermissionKeys = new[]
            {
                "Patient.View",
                "Appointment.View", "Appointment.Create",
                "Doctor.View", "Department.View", "DepartmentTerm.View",
                "TreatmentHistory.View", "Billing.View"
            };

            var existingPatientPermissions = await dbContext.RolePermissions
                .Where(rp => rp.RoleId == patientRole.Id)
                .Select(rp => rp.Permission.Key)
                .ToListAsync();

            var patientPermissionsToCreate = allPermissions
                .Where(p => patientPermissionKeys.Contains(p.Key, StringComparer.OrdinalIgnoreCase) && 
                            !existingPatientPermissions.Contains(p.Key, StringComparer.OrdinalIgnoreCase))
                .Select(p => new RolePermission { RoleId = patientRole.Id, PermissionId = p.Id })
                .ToList();

            if (patientPermissionsToCreate.Any())
            {
                await dbContext.RolePermissions.AddRangeAsync(patientPermissionsToCreate);
                await dbContext.SaveChangesAsync();
            }
        }

        // 7. Seed default Super Admin User if no users exist
        var defaultAdmin = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == "developer");
        if (defaultAdmin == null)
        {
            defaultAdmin = new User
            {
                Name = "System Engineer",
                Username = "developer",
                Email = "developer@dentalapp.com",
                Mobile = "01700000000",
                PasswordHash = PasswordHasher.Hash("Developer@682672")
            };

            await dbContext.Users.AddAsync(defaultAdmin);
            await dbContext.SaveChangesAsync();
        }

        // Ensure default user has SuperAdmin role
        var existingSuperAdminRole = await dbContext.UserRoles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(ur => ur.UserId == defaultAdmin.Id && ur.RoleId == superAdminRole!.Id);

        if (existingSuperAdminRole == null)
        {
            await dbContext.UserRoles.AddAsync(new UserRole
            {
                UserId = defaultAdmin.Id,
                RoleId = superAdminRole!.Id
            });
            await dbContext.SaveChangesAsync();
        }
        else if (existingSuperAdminRole.IsDeleted)
        {
            existingSuperAdminRole.IsDeleted = false;
            existingSuperAdminRole.IsActive = true;
            await dbContext.SaveChangesAsync();
        }

        // Seed default landing page sections if empty or has old seed data
        var oldSeedExists = await dbContext.LandingPageSections.AnyAsync(s => s.Title == "আমি আপনার দাঁতের ডাক্তার");
        if (oldSeedExists)
        {
            var oldSections = await dbContext.LandingPageSections.ToListAsync();
            dbContext.LandingPageSections.RemoveRange(oldSections);
            await dbContext.SaveChangesAsync();
        }

        var hasSections = await dbContext.LandingPageSections.AnyAsync();
        if (!hasSections)
        {
            await dbContext.LandingPageSections.AddRangeAsync(new List<LandingPageSection>
            {
                new()
                {
                    Title = "Farzana's Painless Dental Care",
                    Content = "Providing state-of-the-art, stress-free dental treatments with utmost care and precision. Experience modern dentistry designed to keep your smile healthy and beautiful.",
                    ImageUrl = "images/dental_banner.png",
                    DisplayOrder = 1,
                    SectionType = "Hero",
                    IsVisible = true
                },
                new()
                {
                    Title = "Meet Our Senior Consultant",
                    Content = "Dr. Abdullah Al Mamun\nBDS (DU), MDS\nPGT (Conservative Dentistry & Endodontics)\nSpecialist in Dental Implants & Root Canal Therapy\nDhaka Central International Medical College & Hospital\nReg No: 11783\nContact: +8801737-591865",
                    ImageUrl = "images/doctor_mamun.jpg",
                    DisplayOrder = 2,
                    SectionType = "DoctorProfile",
                    IsVisible = true
                },
                new()
                {
                    Title = "Our Premium Dental Treatments",
                    Content = "Root Canal Therapy\nSave your natural teeth with advanced, painless root canal treatments.\n\nDental Implants\nRestore missing teeth permanently with state-of-the-art dental implants.\n\nCrowns & Bridges\nStrengthen and restore damaged teeth with high-quality porcelain crowns.\n\nScaling & Polishing\nMaintain fresh breath and clean teeth with professional cleaning.",
                    ImageUrl = "",
                    DisplayOrder = 3,
                    SectionType = "Treatments",
                    IsVisible = true
                },
                new()
                {
                    Title = "Advanced Dental Care Technology",
                    Content = "Watch how modern painless dentistry helps you maintain perfect oral hygiene without any fear or discomfort.",
                    ImageUrl = "https://www.youtube.com/embed/hB2h3V6q-fQ",
                    DisplayOrder = 4,
                    SectionType = "Video",
                    IsVisible = true
                },
                new()
                {
                    Title = "Excellence in Clinical Hygiene",
                    Content = "At Farzana's Dental Care, patient safety and clinical hygiene are our top priorities. We use state-of-the-art sterilisation methods, autoclave systems, and gentle diagnostic tools to ensure a completely safe and pain-free treatment experience.",
                    ImageUrl = "",
                    DisplayOrder = 5,
                    SectionType = "About",
                    IsVisible = true
                },
                new()
                {
                    Title = "Visiting Hours & Location",
                    Content = "10:00 AM - 02:00 PM\n04:00 PM - 10:00 PM",
                    ImageUrl = "",
                    DisplayOrder = 6,
                    SectionType = "InfoCard",
                    IsVisible = true
                }
            });
            await dbContext.SaveChangesAsync();
        }
    }
}

