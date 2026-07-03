---
name: clean-slice-ui-design
description: >-
  Designs or refactors Hospital Blazor WASM components, code-behind classes, isolated CSS,
  and API client services. Use when working in Presentation/Hospital.Client.
---

# Clean Slice UI Design & Blazor WASM Guidelines

Use this skill when developing, refactoring, or designing UI components in the Blazor WASM frontend project.

## Reference Example & Directory Benchmark
* **Reference Example:** Always follow the **Products** feature structure under `src/Presentation/Hospital.Client/Pages/Definitions/Products/` as the standard benchmark.
* **UI File Splitting (Modular Design):**
  * Do **NOT** create a single giant razor file for all views.
  * Split UI features into smaller, logical sub-component razor files (e.g., separate files for `CreateModal`, `EditModal`, `ViewModal`, `BulkImportModal`, etc.).
  * For each of these sub-components/modals, you **MUST** maintain separate files for:
    1. The markup (`.razor`)
    2. The code-behind partial class (`.razor.cs`)
    3. The component-specific isolated CSS (`.razor.css`)

## Strict Separation & Styling Rules
* **Code-Behind Pattern:**
  * `MyComponent.razor`: Contains only HTML markup, CSS classes, and basic bindings. **Absolutely no `@code` blocks.**
  * `MyComponent.razor.cs`: Partial class containing UI logic, lifecycle methods, properties, injected services, and event handlers.
  * `MyComponent.razor.css`: Holds component-isolated CSS styles. **Avoid inline styles entirely.**
* **Strong UI/UX Priority (Aesthetics & Usability):**
  * You **MUST** prioritize a premium, modern, and user-friendly UI/UX in every component and page. This includes clear typographic hierarchy, comfortable spacing, cohesive and soft clinical color palettes, intuitive controls (icons, tooltips, loading states), micro-animations, and tactile active/hover states.
* **Multi-Device Responsive Design:**

  * When designing or refactoring any UI, you **MUST** explicitly optimize the user experience (UX/UI) for three device categories:
    1. **Desktop/Laptop browser view:** Standard grid layouts, fixed sidebars, and spacious forms.
    2. **Tablet (Tab) view:** Collapsed/fluid sidebars, responsive grids, and flexible cards.
    3. **Mobile view:** Bottom tab bar navigation, sliding Sidebar Drawer with user banner, stacked forms (1 column), and card list layouts. Never display wide table grids without horizontal scroll.
* **Modular CSS Separation:**
  * Do **NOT** let global `app.css` grow bulky. Keep it modular by dividing major device-specific or layout-specific styles into separate files (e.g. `variables.css`, `base.css`, `desktop.css`, `tablet.css`, `mobile.css`) and importing them using CSS `@import`.
* **Buttons & Actions Consistency:**
  * **Add/Create/Primary:** Soft premium brand blue (`--color-primary`)
  * **Download/Active Status/Success:** Soft eye-pleasing green (`--color-success`)
  * **Edit/Warning:** Warm soft amber/orange (`--color-warning`)
  * **Delete/Inactive Status/Danger:** Soft pastel red (`--color-danger`)
  * **View/Info:** Soft cyan/indigo (`--color-info`)
* **Reusable Components:**
  * Use the centralized `SharedDataTable<TItem>.razor` for lists and data table presentations.
  * Avoid repeating HTML/CSS structures for tables, modal overlays, inputs, loaders, and card views across different pages.

## API Integration & Client Services
* Use **Typed HTTP Clients** registered via `IHttpClientFactory`.
* **No hardcoded URLs in methods.** Endpoint base URLs must be declared as constants or variables at the top of the client class.
* Always propagate `CancellationToken` from Blazor components to API calls.

## Reference Examples
* [ProductViewModal.razor](examples/ProductViewModal.razor) — markup-only modal structure utilizing standard classes.
* [ProductViewModal.razor.cs](examples/ProductViewModal.razor.cs) — code-behind partial class with properties, parameters, and event handlers.
* [ProductViewModal.razor.css](examples/ProductViewModal.razor.css) — component-specific isolated styling rules.
