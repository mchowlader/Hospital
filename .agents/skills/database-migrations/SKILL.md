---
name: database-migrations
description: >-
  Adds EF Core migrations or updates the Hospital PostgreSQL database schema.
  Use when the user wants schema changes, new migrations, or database updates.
---

# Database Migrations Guide

The project uses EF Core with a single `AppDbContext` targeting PostgreSQL.

All Entity Framework Core commands must run with the working directory set to the repository root.

## 1. Add a Database Migration
Run the following command in PowerShell/Terminal to scaffold a new migration:

```powershell
dotnet ef migrations add <MigrationName> --project src/Infrastructure/Hospital.Infrastructure --startup-project src/Presentation/Hospital.Api --context AppDbContext
```

* Replace `<MigrationName>` with a descriptive CamelCase name (e.g., `AddStockInEntities`, `AddWarehouseEntity`).
* `--project` points to the Infrastructure project containing the database configurations and context.
* `--startup-project` points to the API project containing `appsettings.json` and connection settings.
* `--context` explicitly sets `AppDbContext`.

## 2. Apply/Update Database Schema
Run the following command to update the local PostgreSQL database with pending migrations:

```powershell
dotnet ef database update --project src/Infrastructure/Hospital.Infrastructure --startup-project src/Presentation/Hospital.Api --context AppDbContext
```

## 3. Common Guidelines & Pitfalls
* **Casing:** Ensure table names and query filters match PostgreSQL casing conventions.
* **Auto-generated Audit logs:** `AppDbContext` intercepts save calls to populate audit fields. Ensure entities inherit from `BaseEntity`.
* **Unique Partial Indexes:** All unique constraints on tables supporting soft delete must include the condition `.HasFilter("\"IsDeleted\" = false")` in their Fluent configuration to avoid collisions.
