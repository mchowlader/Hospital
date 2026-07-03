---
name: add-new-feature
description: >-
  Creates or implements a new Hospital feature with full Domain, EF config, API slice,
  Blazor UI, and migration. Use when the user asks to add a new entity, endpoints, or UI pages.
---

# Add a New Feature Checklist

Follow this checklist in order when implementing a new database entity with full API endpoints and UI interface. Each layer depends on the one before it.

## 1. Domain & Shared Core (Core Projects)
* **Entity (`src/Core/Hospital.Domain/Entities/`):**
  * Create the entity file `{Entity}.cs`.
  * Inherit from `BaseEntity` (inherits `long Id`, audit properties, and soft-delete properties).
  * Expose collection navigation properties if there are sub-tables or variants.
  * Add the entity to `DbSet<T>` in `src/Infrastructure/Hospital.Infrastructure/Persistence/AppDbContext.cs`.
* **DTOs (`src/Core/Hospital.Shared/DTOs/`):**
  * Create `{Entity}Dtos.cs` inside the appropriate subfolder (e.g. `Definitions` or `Inventory`).
  * Add DTOs (classes or records) like `Create{Entity}Request`, `Update{Entity}Request`, `{Entity}Dto`, `{Entity}AdminDto`.
* **Validation (`src/Core/Hospital.Shared/Validators/`):**
  * Create `{Entity}Validators.cs` containing validation classes inheriting from `AbstractValidator<T>` (e.g., `Create{Entity}RequestValidator`).

## 2. Database Mapping (Infrastructure Project)
* **Configuration (`src/Infrastructure/Hospital.Infrastructure/Persistence/Configurations/`):**
  * Create `{Entity}Configuration.cs` implementing `IEntityTypeConfiguration<{Entity}>`.
  * Define all column limits, required properties, and foreign keys.
  * **Unique Indexes:** Configure partial indexes with `.HasFilter("\"IsDeleted\" = false")` for any unique fields so soft-deleted records do not block new records with the same value.

## 3. Presentation API (API Project)
Create a dedicated slice folder inside `src/Presentation/Hospital.Api/Features/{Folder}/{Entity}s/` containing:
* **`{Entity}Endpoints.cs`:**
  * Implement `IEndpoint` interface. Expose a public `MapEndpoint(IEndpointRouteBuilder app)` method.
  * Use **Named Methods (Delegate Methods)** for route handlers (no inline lambdas).
  * Auto-register routes by calling `app.MapAllEndpoints()` at startup (no manual `Program.cs` additions).
  * Apply `.RequirePermission("{Entity}.{Action}")` to protect routes.
* **Handlers (`{Action}{Entity}Handler.cs`):**
  * Create separate handler classes for each API operation (e.g. `Create{Entity}Handler.cs`, `Delete{Entity}Handler.cs`, `Get{Entity}sHandler.cs`).
  * Inject `AppDbContext` and relevant validators directly (no MediatR).
  * Return `Result` or `Result<T>` instead of throwing business exceptions.
  * Map these results to standard HTTP codes and RFC-7807 Problem Details in the endpoint using `.ToProblemDetails()`.
  * Ensure `Delete` handlers perform a soft delete (`IsDeleted = true`), not physical database removal.

## 4. Blazor WASM Frontend (Client Project)
Create a folder at `src/Presentation/Hospital.Client/Pages/{Folder}/{Entity}s/` containing:
* **UI Structure (Separate Razor Files):**
  * Split UI components into separate, manageable files rather than one large page (e.g., `{Entity}s.razor` for grid, `{Entity}CreateModal.razor`, `{Entity}EditModal.razor`, `{Entity}ViewModal.razor`).
* **Strict Code-Behind & Styles Isolation:**
  * `{Component}.razor`: Contains HTML markup, CSS classes, bindings, and subcomponent tags. **No `@code` blocks.**
  * `{Component}.razor.cs`: Partial class holding properties, parameters, event handlers, lifecycle overrides, and backend API service calls.
  * `{Component}.razor.css`: Holds component-isolated CSS styles. **No inline styles (`style="..."`) or `<style>` blocks.**
* **Strong UI/UX Priority (Aesthetics & Usability):**
  * You **MUST** prioritize a premium, modern, and user-friendly UI/UX in every feature and page. This includes clear typographic hierarchy, comfortable spacing, cohesive and soft clinical color palettes, intuitive controls (icons, tooltips, loading states), micro-animations, and tactile active/hover states.
* **Multi-Device Responsive Design:**
  * When designing or refactoring any UI, you **MUST** explicitly optimize the user experience (UX/UI) for three device categories:
    1. **Desktop/Laptop browser view:** Standard layouts, fixed sidebars, and spacious forms.
    2. **Tablet (Tab) view:** Collapsed/fluid sidebars, responsive grids, and flexible cards.
    3. **Mobile view:** Bottom tab bar navigation, sliding Sidebar Drawer with user banner, stacked forms (1 column), and card list layouts. Never display wide table grids without horizontal scroll.
* **Modular CSS Separation:**
  * Do **NOT** let global `app.css` grow bulky. Keep it modular by dividing major device-specific or layout-specific styles into separate files (e.g. `variables.css`, `base.css`, `desktop.css`, `tablet.css`, `mobile.css`) and importing them using CSS `@import`.
* **UI Components & Colors:**
  * Use the centralized `SharedDataTable<TItem>.razor` for tables/lists.
  * Keep button colors consistent using the HSL/Slate palette CSS variables.
* **HTTP Client:** Use a Typed HTTP Client via `IHttpClientFactory` with base URLs declared as constants. Always pass the component's `CancellationToken` to API calls.


## 5. DB Migration
* Generate and apply migration using EF Core tools. See the `database-migrations` skill for command details.
