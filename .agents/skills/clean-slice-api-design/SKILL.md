---
name: clean-slice-api-design
description: >-
  Designs or refactors Hospital Minimal API endpoints, MediatR-free handlers, and API responses.
  Use when creating, editing, or refactoring C# endpoints in Presentation/Hospital.Api.
---

# Clean Slice API Design & Handler Guidelines

Use this skill when developing or modifying API endpoints and business handlers.

## Architecture Guidelines
* **No MediatR:** Native .NET Dependency Injection with custom Handler classes.
* **API Endpoints (`Endpoint.cs`):**
  * Thin gateways only. No business logic, DB queries, or data mapping.
  * Receive HTTP requests, delegate to the corresponding `Handler.cs`, return HTTP response.
* **Named Methods Routing:**
  * Do **NOT** use inline lambdas for mapping routes.
  * Use **Named Methods (Delegate Methods)**. Register routes by referencing private static methods inside the endpoint class.
* **Automatic Endpoint Registration:**
  * Define an `IEndpoint` interface. All endpoint classes must implement this interface.
  * Use the assembly-scanning extension method `MapAllEndpoints()` in the API project to automatically find and register all endpoints implementing `IEndpoint` at startup.

## Error Handling & Responses
* **Result Pattern:**
  * **No business exceptions** for control flow (NotFound, ValidationErrors, Conflicts).
  * Use the functional **Result Pattern** (`Result` and `Result<T>`) to return failure states from Handlers.
  * Map result states to HTTP Status Codes using RFC-7807 Problem Details.

## Entity API Endpoint Standards (Minimum 6 APIs)
For each entity slice, ensure there are at least 6 standard endpoints implemented:
1. `GET /api/{entity}` (search & server-side pagination)
2. `GET /api/{entity}/{id}` (get details by Id)
3. `GET /api/{entity}/export` (CSV export)
4. `POST /api/{entity}` (Create)
5. `PUT /api/{entity}/{id}` (Update)
6. `DELETE /api/{entity}/{id}` (Soft delete)

## Reference Examples
* [ProductEndpoints.cs](examples/ProductEndpoints.cs) — routing and endpoint mapping using named delegate methods.
* [CreateProductHandler.cs](examples/CreateProductHandler.cs) — handler logic with FluentValidation and the Result pattern.
