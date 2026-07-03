---
name: clean-slice-database
description: >-
  Defines or refactors Hospital database entities, BaseEntity, EF Core DbContext, and
  IEntityTypeConfiguration. Use when working in Domain or Infrastructure persistence code.
---

# Clean Slice Database & Entity Guidelines

Use this skill when defining or modifying database entities, entity mapping configurations, or EF Core logic.

## Entity & Database Conventions
* **Primary Keys:** The primary key (ID) for all database entities must be of type `long` (Int64 / PostgreSQL `bigint`). Do NOT use `int` or `Guid` for IDs.
* **Audit Logging & Base Entity:**
  * All entities must inherit from a common `BaseEntity` class containing:
    * `long Id`
    * `DateTime CreatedAt`
    * `string? CreatedBy`
    * `DateTime? UpdatedAt`
    * `string? UpdatedBy`
    * `bool IsActive` (defaults to `true`)
    * `bool IsDeleted` (defaults to `false`)
* **EF Core Automation:**
  * EF Core must automatically populate audit properties (`CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`) on insert/update operations by injecting a service that reads the current logged-in user context.
  * Intercept deletes to perform soft deletes (`IsDeleted = true`).
  * Configure **Global Query Filters** for all entities inheriting from `BaseEntity` to filter out soft-deleted data (`IsDeleted == false`) by default.

## Entity Configurations (Fluent API Separation)
* Do **NOT** write Entity Framework Core Fluent API mapping configurations directly inside `AppDbContext.cs`'s `OnModelCreating`.
* Each entity must have its own configuration class implementing `IEntityTypeConfiguration<TEntity>` (placed inside the `Persistence/Configurations/` directory).
* Scan and register configurations using:
  `modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly)`
* **Unique Indexes:** All unique indexes for entities implementing soft delete must be partial indexes configured with `.HasFilter("\"IsDeleted\" = false")`. This allows creating a new active record with the same name if the existing record was soft-deleted.

## Performance Optimization
* **Read-only DB Queries:** Always use `.AsNoTracking()` in EF Core.
* **Database Queries:** Perform projections using `.Select()` to only pull required fields. No `SELECT *` equivalent.

## Reference Examples
* [ProductConfiguration.cs](examples/ProductConfiguration.cs) — Fluent API mapping with partial unique indexes.
