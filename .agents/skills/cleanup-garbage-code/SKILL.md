---
name: cleanup-garbage-code
description: >-
  Reviews Hospital code for architecture violations, build warnings, and refactoring cleanup.
  Use when reviewing code, refactoring features, or cleaning up before merge.
---

# Clean Up Garbage Code Checklist

Use this checklist during code reviews, before submitting changes, or when tasked with refactoring.

## 1. Clean Slice Architecture Compliance
Ensure there are no architectural violations:
* **API Endpoints (`Endpoint.cs`):**
  * Verify there is **no business logic, DB queries, or direct mappings** in endpoints. They must only receive inputs, call Handlers, and map results to HTTP codes.
  * Verify routes are registered with **Named Methods (Delegate Methods)**. No inline lambdas.
  * Ensure the endpoint class implements `IEndpoint` and is scanned dynamically.
* **MediatR and AutoMapper Avoidance:**
  * Ensure handlers are normal classes registered directly in Dependency Injection. No MediatR commands, queries, or behaviors.
  * Ensure mappings are done manually or via helper conversion methods. No AutoMapper profiles.
* **Db Access Rules:**
  * Application Handlers in the API feature slices can query `AppDbContext` directly. However, ensure they do not throw business exceptions for flow control (use the `Result` pattern instead).

## 2. Database & Soft-Delete Compliance
* **Physical Deletes:** Check for any instances of physical DB deletions (e.g. `Remove()`, `RemoveRange()`). Replace them with soft deletes (`IsDeleted = true`).
* **Configurations:** Ensure all database entity mappings (indexes, constraints) live in configurations classes under `Persistence/Configurations/`, not inline inside `OnModelCreating`.
* **Unique Partial Indexes:** Ensure unique index builders are configured with `.HasFilter("\"IsDeleted\" = false")`.

## 3. Frontend & Blazor Compliance
* **Strict Separation of Concerns:**
  * Ensure **no `@code` blocks** containing business logic exist in `.razor` components. All UI state, handlers, and logic must reside in a `.razor.cs` code-behind file.
  * Ensure **no inline styles (`style="..."`) or `<style>` blocks** exist in `.razor` files. All layout/styling must reside in isolated `.razor.css` files.
* **Component Splitting:**
  * Check for large, bloated `.razor` pages. Refactor them by splitting modals, sidebars, and detail forms into separate components under the same feature directory.
* **Client Service API Integration:**
  * Verify Typed HTTP Client is used with cancellation tokens properly passed to API requests.

## 4. Verification
After executing cleanup or refactoring:
* Run `dotnet build` from the repository root directory. The build must succeed without errors.
* Verify that frontend and backend work correctly in combination.
