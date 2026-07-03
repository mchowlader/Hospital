# Agent Instructions for Hospital Management Project

<!-- lean-ctx -->
## lean-ctx
Prefer lean-ctx MCP tools over native equivalents for token savings:
`ctx_read` > Read/cat, `ctx_search` > Grep/rg, `ctx_shell` > bash, `ctx_tree` > ls/find.
Native Edit/Write/Glob stay as-is; use `ctx_edit` only when Edit needs an unavailable Read.
Full rules: LEAN-CTX.md (open on demand — do not auto-load).
<!-- /lean-ctx -->

## Workflow & Language Guidelines
- **Always Start with a Plan in Bengali:** Before making any code changes or starting the implementation of any feature, task, or bug-fix, you MUST create a detailed implementation plan. This plan and the description of proposed changes MUST be written in Bengali (বাংলা).
- **Wait for Explicit Approval:** After presenting the implementation plan, you MUST stop and wait for the user's explicit approval ("Proceed" / "এগিয়ে যাও") before starting any code changes or command execution.
- **Walkthrough in Bengali:** Upon completing the task, the final walkthrough (including `walkthrough.md` and your summary response in the chat) MUST be written in Bengali (বাংলা).
- **Consistent Enforcement:** These rules must be followed automatically for every task in this repository without the user having to repeat them in the chat.

## Technical Architecture & Clean Slice Standards
- **Architecture Pattern:** Clean Slice Architecture (Vertical Slice). No MediatR. No AutoMapper. Endpoints map to Handlers directly.
- **Result Pattern:** Handlers return `Result` / `Result<T>`. Never throw for NotFound, Validation, or Conflict.
- **Soft Delete:** Always set `IsDeleted = true`. Do not use physical delete.
- **Dynamic Permissions:** Dynamic role-based permissions must restrict API access and UI elements.

## Blazor WASM UI Standards
- **File Separation Rule:** Do not include CSS styles or C# `@code` blocks inside Razor files. For every component, use three separate files:
  1. `ComponentName.razor` (HTML/Razor markup)
  2. `ComponentName.razor.cs` (C# logic using partial class)
  3. `ComponentName.razor.css` (Isolated styling)
- **Centralized API Calls:** Never make HTTP requests with hardcoded URLs inside components. Always use typed Service Clients (e.g. `PatientServiceClient.cs` inheriting from `BaseServiceClient.cs`).
- **Color Aesthetics:** Hospital/clinical theme with soft, eye-soothing colors (e.g., slate blue, light teal/green, comfortable gray backgrounds). Avoid bright, harsh primary colors.
- **Mobile Experience:** Fully responsive layout with mobile views optimized to look like a native mobile app shell (e.g., bottom tab bar navigation, fixed headers, card list layouts).
- **Multi-Device Responsive Design:** When designing or refactoring any UI, you MUST explicitly consider and optimize the user experience (UX/UI) for three device categories:
  1. Desktop/Laptop browser view (standard layouts, clean sidebars)
  2. Tablet (Tab) view (responsive grids, fluid sidebars, collapsed sidebars if needed)
  3. Mobile view (native app feel, bottom tab bar navigation, sliding Sidebar Drawer, responsive cards, no bulkiness)
- **Modular CSS Separation:** Do not let global `app.css` grow bulky. Keep it modular by dividing major device-specific or layout-specific styles into separate files (e.g. `variables.css`, `base.css`, `desktop.css`, `tablet.css`, `mobile.css`) and importing them using CSS `@import`.

