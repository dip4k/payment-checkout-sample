# Application Architecture And Flow

This wiki page captures the current Part One and Part Two architecture and the end-to-end request flow between the React UI and the .NET backend.

## API Project Dependency Graph

```mermaid
flowchart LR
        Api[CheckoutSystem.Api]
        App[CheckoutSystem.Application]
        Domain[CheckoutSystem.Domain]
        Infra[CheckoutSystem.Infrastructure]

        Api --> App
        Api --> Infra
        Api --> Domain
        App --> Domain
        Infra --> App
        Infra --> Domain
```

### Dependency Intent

- `CheckoutSystem.Domain`: core business concepts that should stay framework-agnostic and stable.
- `CheckoutSystem.Application`: use-case orchestration, validation, and business workflows that depend on domain types, not storage or transport details.
- `CheckoutSystem.Infrastructure`: technical implementations (EF Core, SQLite, repository implementations, event dispatch handlers) that fulfill application abstractions.
- `CheckoutSystem.Api`: transport layer and composition root (endpoint mapping, contracts, middleware, DI wiring) that references the other projects to expose HTTP behavior.
- `CheckoutSystem.Api -> CheckoutSystem.Domain` is acceptable in this proof-of-concept because endpoint contract mapping uses `DiscountType`; for stricter layering, map to an application enum/DTO and remove this reference.

## Layered Block Diagram (What Goes Where)

```mermaid
flowchart TB
        Client[Client Apps\nReact UI / Swagger / External Consumers]

        subgraph ApiLayer[API Layer - CheckoutSystem.Api]
                Middleware[Cross-cutting Middleware\nCorrelationId + Exception Handling]
                Endpoints[Versioned HTTP Endpoints\n/api/v1/*]
                Contracts[Request/Response Contracts\nContracts/V1]
                Composition[Composition Root\nProgram.cs DI + pipeline]
        end

        subgraph AppLayer[Application Layer - CheckoutSystem.Application]
                UseCases[Use Cases\nCheckoutService + SubmitOrderCommandHandler]
                Ports[Ports/Abstractions\nPersistence + Events + Validation]
                AppModels[Application Models\nRequest/Result models]
                Policies[Business Policies\nValidation + orchestration]
        end

        subgraph DomainLayer[Domain Layer - CheckoutSystem.Domain]
                Entities[Domain Types\nProduct, DiscountType]
                DomainRules[Pure Business Rules\nMoney and discount primitives]
        end

        subgraph InfraLayer[Infrastructure Layer - CheckoutSystem.Infrastructure]
                Persistence[Persistence Adapters\nDbContext + repositories + unit of work]
                Eventing[Event Adapters\nDomainEventDispatcher + handlers]
                DataStore[(SQLite)]
        end

        Client --> Endpoints
        Middleware --> Endpoints
        Endpoints --> UseCases
        Endpoints --> Contracts
        UseCases --> Ports
        UseCases --> AppModels
        UseCases --> Entities
        Ports --> Persistence
        Ports --> Eventing
        Persistence --> DataStore
        Eventing --> DataStore
```

### Layer Placement Rules And Why

- API layer:
    Handles HTTP-only concerns (routing, versioning, headers, status codes, OpenAPI, middleware) so transport changes do not ripple into business logic.
- Application layer:
    Owns use-case flow (calculate, submit), validation, idempotency workflow orchestration, and transaction boundaries via abstractions so the business process is testable and independent of infrastructure details.
- Domain layer:
    Holds core concepts and invariant-friendly types (`Product`, `DiscountType`) so critical business meaning remains independent from frameworks and I/O.
- Infrastructure layer:
    Implements technical concerns (EF Core, SQLite, repository classes, event handlers, migrations) so persistence/event technology can evolve without rewriting use-case logic.
- Dependency rule:
    Dependencies should point inward toward stable business policy (`Api/Infrastructure -> Application -> Domain`), with outward details implemented through interfaces from the application layer.

## Architecture Diagram

```mermaid
flowchart LR
    Operator[Operator]
    UI[React UI\nui/src]
    Client[API Client\ncheckoutApi.ts]
    Api[ASP.NET Core Minimal API\n/api/v1]
    Middleware[Middleware\nCorrelation + Exception Handling]
    Application[Application Layer\nCheckoutService + SubmitOrderCommandHandler]
    Domain[Domain Rules\nProducts + Discounts + Tax + Rounding]
    Infra[Infrastructure Layer\nRepositories + Unit of Work + Event Dispatcher]
    Db[(SQLite)]
    Outbox[(OutboxMessages)]

    Operator --> UI
    UI --> Client
    Client --> Api
    Api --> Middleware
    Middleware --> Application
    Application --> Domain
    Application --> Infra
    Infra --> Db
    Infra --> Outbox
    Db --> Infra
    Infra --> Application
    Application --> Middleware
    Middleware --> Api
    Api --> Client
    Client --> UI
```

## Application Flow

```mermaid
sequenceDiagram
    participant Operator
    participant UI as React UI
    participant Client as API Client
    participant API as Checkout API
    participant Service as Application Services
    participant Data as SQLite

    Operator->>UI: Open checkout console
    UI->>Client: getCatalogue()
    Client->>API: GET /api/v1/catalogue
    API->>Service: Get catalogue items
    Service->>Data: Read seeded products
    Data-->>Service: Catalogue rows
    Service-->>API: Catalogue models
    API-->>Client: CatalogueItemResponse[]
    Client-->>UI: Render product cards and versions

    Operator->>UI: Set quantities and optional discount
    UI->>Client: calculateOrder(request, correlationId)
    Client->>API: POST /api/v1/orders/calculate
    API->>Service: Validate request and calculate totals
    Service->>Data: Read products by id
    Data-->>Service: Matching products
    Service-->>API: Subtotal, discount, tax, total, split shares
    API-->>Client: CalculateOrderResponse
    Client-->>UI: Show totals and split breakdown

    Operator->>UI: Submit order
    UI->>Client: submitOrder(request, idempotencyKey, correlationId)
    Client->>API: POST /api/v1/orders/submit
    API->>Service: SubmitOrderCommandHandler
    Service->>Data: Check idempotency + product versions
    Service->>Data: Persist order, line snapshots, status history
    Service->>Data: Persist outbox message seam
    Data-->>Service: Stored order result
    Service-->>API: Final totals, split shares, or replayed result
    API-->>Client: CalculateOrderResponse
    Client-->>UI: Confirm successful submission and split view
```

## Notes

- The UI targets `http://localhost:5152/api/v1` by default and can be overridden with `VITE_API_BASE_URL`.
- Request payloads use string enum values for discount type, matching both the UI client and Swagger examples.
- Submit requests must include `Idempotency-Key`; the backend replays the prior result when the same key and payload are retried.
- Optimistic concurrency is enforced through `productVersion` on each selected line item.
- Split-payment output currently returns exactly 3 whole-number shares (no cents), with any remainder whole unit assigned to payer 1.