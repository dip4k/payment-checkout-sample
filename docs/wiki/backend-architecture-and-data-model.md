# Backend Architecture And Data Model

This page documents backend project dependencies, layer responsibilities, request flow, and the SQLite schema for the checkout proof of concept.

## 1. API Project Dependency Graph

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

### Why This Dependency Shape Exists

- Domain: stable business concepts and primitives that remain framework-agnostic.
- Application: use-case orchestration and policies that depend on domain types and abstractions.
- Infrastructure: technical adapters that implement application abstractions for persistence and event handling.
- API: transport and composition root concerns that wire up application and infrastructure for HTTP.
- Current POC exception: API references Domain to map discount enum values; a stricter clean layering variant would move this mapping to application contracts and remove Api -> Domain.

## 2. Layered Block Diagram (What Goes In Which Layer)

```mermaid
flowchart TB
    Client[Client Apps\nReact UI / Swagger / External Consumers]

    subgraph ApiLayer[API Layer - CheckoutSystem.Api]
        Middleware[Cross-cutting Middleware\nCorrelationId + Exception Handling]
        Endpoints[Versioned HTTP Endpoints\n/api/v1/*]
        Contracts[Request and Response Contracts\nContracts/V1]
        Composition[Composition Root\nProgram.cs pipeline and DI]
    end

    subgraph AppLayer[Application Layer - CheckoutSystem.Application]
        UseCases[Use Cases\nCheckoutService + SubmitOrderCommandHandler]
        Ports[Abstractions and Ports\nPersistence + Events + Validation]
        AppModels[Application Models\nrequest/result payloads]
        Policies[Business Workflow Policies\nidempotency + orchestration]
    end

    subgraph DomainLayer[Domain Layer - CheckoutSystem.Domain]
        Entities[Domain Types\nProduct, DiscountType]
        DomainRules[Core Business Meaning\nmoney and discount semantics]
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

### Placement Rules

- API layer owns HTTP concerns only (routing, headers, status codes, middleware, OpenAPI).
- Application layer owns use-case sequencing, validation, and business workflow orchestration.
- Domain layer owns core business concepts and invariant-friendly types.
- Infrastructure layer owns technical implementation details and external I/O.
- Dependency direction stays inward: Api and Infrastructure depend on Application, and Application depends on Domain.

## 3. End-To-End Request Flow

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
    API->>Service: Fetch catalogue
    Service->>Data: Query products
    Data-->>Service: Product rows
    Service-->>API: Catalogue models
    API-->>Client: CatalogueItemResponse[]
    Client-->>UI: Render product cards

    Operator->>UI: Set quantities and optional discount
    UI->>Client: calculateOrder(request, correlationId)
    Client->>API: POST /api/v1/orders/calculate
    API->>Service: Validate and calculate totals
    Service->>Data: Read products by id
    Data-->>Service: Matching products
    Service-->>API: Totals and split shares
    API-->>Client: CalculateOrderResponse
    Client-->>UI: Show totals

    Operator->>UI: Submit order
    UI->>Client: submitOrder(request, idempotencyKey, correlationId)
    Client->>API: POST /api/v1/orders/submit
    API->>Service: Execute submit use case
    Service->>Data: Validate idempotency and versions
    Service->>Data: Persist order and audit data
    Service->>Data: Persist outbox seam
    Data-->>Service: Submission result
    Service-->>API: Final/replayed totals
    API-->>Client: CalculateOrderResponse
    Client-->>UI: Confirm success
```

## 4. Database Table Relationship Diagram

```mermaid
erDiagram
    ORDERS ||--o{ ORDER_LINE_SNAPSHOTS : contains
    ORDERS ||--o{ ORDER_STATUS_HISTORY : tracks

    PRODUCTS {
        guid Id PK
        string Name
        decimal UnitPrice
        bool IsTaxable
        long Version
    }

    IDEMPOTENCY_RECORDS {
        guid Id PK
        string Key
        string OperationName
        string RequestHash
        decimal Subtotal
        decimal DiscountApplied
        decimal Tax
        decimal Total
        datetimeoffset CreatedAtUtc
    }

    ORDERS {
        guid Id PK
        string IdempotencyKey UK
        string CorrelationId
        decimal Subtotal
        decimal DiscountApplied
        decimal Tax
        decimal Total
        datetimeoffset CreatedAtUtc
    }

    ORDER_LINE_SNAPSHOTS {
        guid Id PK
        guid OrderId FK
        guid ProductId
        string ProductName
        decimal UnitPrice
        bool IsTaxable
        int Quantity
        long ProductVersion
    }

    ORDER_STATUS_HISTORY {
        guid Id PK
        guid OrderId FK
        string Status
        datetimeoffset ChangedAtUtc
        string Note
    }

    OUTBOX_MESSAGES {
        guid Id PK
        string EventType
        string AggregateId
        string Payload
        datetimeoffset CreatedAtUtc
        datetimeoffset ProcessedAtUtc
    }
```

### Relationship Notes

- Orders to OrderLineSnapshots is one-to-many with cascade delete.
- Orders to OrderStatusHistory is one-to-many with cascade delete.
- OrderLineSnapshots.ProductId is intentionally stored as a snapshot value and does not enforce a foreign key to Products.
- OutboxMessages is independent from Orders to support eventual integration publishing boundaries.
- IdempotencyRecords is independent from Orders and stores replay payload totals for idempotent submission behavior.

## 5. Database Schema Details

### Products

Purpose: source-of-truth catalogue rows used for pricing and taxability.

| Column | Type (SQLite/EF) | Null | Constraints |
| --- | --- | --- | --- |
| Id | TEXT (Guid) | No | PK |
| Name | TEXT | No | MaxLength(200) |
| UnitPrice | TEXT (decimal 18,2) | No | Precision(18,2) |
| IsTaxable | INTEGER (bool) | No | - |
| Version | INTEGER (long) | No | Concurrency token, default 1 |

Indexes:

- PK_Products on Id.

### IdempotencyRecords

Purpose: stores request-hash keyed response totals for safe replay on retries.

| Column | Type (SQLite/EF) | Null | Constraints |
| --- | --- | --- | --- |
| Id | TEXT (Guid) | No | PK |
| Key | TEXT | No | MaxLength(200) |
| OperationName | TEXT | No | MaxLength(100) |
| RequestHash | TEXT | No | MaxLength(128) |
| Subtotal | TEXT (decimal 18,2) | No | Precision(18,2) |
| DiscountApplied | TEXT (decimal 18,2) | No | Precision(18,2) |
| Tax | TEXT (decimal 18,2) | No | Precision(18,2) |
| Total | TEXT (decimal 18,2) | No | Precision(18,2) |
| CreatedAtUtc | TEXT (DateTimeOffset) | No | - |

Indexes:

- Unique IX_IdempotencyRecords_Key_OperationName on (Key, OperationName).

### Orders

Purpose: immutable financial snapshot per successful submission.

| Column | Type (SQLite/EF) | Null | Constraints |
| --- | --- | --- | --- |
| Id | TEXT (Guid) | No | PK |
| IdempotencyKey | TEXT | No | MaxLength(200), Unique |
| CorrelationId | TEXT | No | MaxLength(100) |
| Subtotal | TEXT (decimal 18,2) | No | Precision(18,2) |
| DiscountApplied | TEXT (decimal 18,2) | No | Precision(18,2) |
| Tax | TEXT (decimal 18,2) | No | Precision(18,2) |
| Total | TEXT (decimal 18,2) | No | Precision(18,2) |
| CreatedAtUtc | TEXT (DateTimeOffset) | No | - |

Indexes:

- Unique IX_Orders_IdempotencyKey on IdempotencyKey.

Relationships:

- One order has many line snapshots.
- One order has many status history records.

### OrderLineSnapshots

Purpose: immutable ordered-line detail captured at submit time.

| Column | Type (SQLite/EF) | Null | Constraints |
| --- | --- | --- | --- |
| Id | TEXT (Guid) | No | PK |
| OrderId | TEXT (Guid) | No | FK -> Orders.Id |
| ProductId | TEXT (Guid) | No | Snapshot identifier |
| ProductName | TEXT | No | MaxLength(200) |
| UnitPrice | TEXT (decimal 18,2) | No | Precision(18,2) |
| IsTaxable | INTEGER (bool) | No | - |
| Quantity | INTEGER | No | - |
| ProductVersion | INTEGER (long) | No | - |

Indexes:

- IX_OrderLineSnapshots_OrderId on OrderId.

Relationships:

- Many snapshots belong to one order via FK with cascade delete.

### OrderStatusHistory

Purpose: append-only status transitions for order lifecycle auditability.

| Column | Type (SQLite/EF) | Null | Constraints |
| --- | --- | --- | --- |
| Id | TEXT (Guid) | No | PK |
| OrderId | TEXT (Guid) | No | FK -> Orders.Id |
| Status | TEXT | No | MaxLength(100) |
| ChangedAtUtc | TEXT (DateTimeOffset) | No | - |
| Note | TEXT | No | MaxLength(500) |

Indexes:

- IX_OrderStatusHistory_OrderId on OrderId.

Relationships:

- Many history records belong to one order via FK with cascade delete.

### OutboxMessages

Purpose: integration-event staging for asynchronous publish processing.

| Column | Type (SQLite/EF) | Null | Constraints |
| --- | --- | --- | --- |
| Id | TEXT (Guid) | No | PK |
| EventType | TEXT | No | MaxLength(200) |
| AggregateId | TEXT | No | MaxLength(100) |
| Payload | TEXT | No | JSON payload |
| CreatedAtUtc | TEXT (DateTimeOffset) | No | - |
| ProcessedAtUtc | TEXT (DateTimeOffset) | Yes | Nullable processing marker |

Indexes:

- IX_OutboxMessages_ProcessedAtUtc on ProcessedAtUtc.

## 6. Operational Notes

- API base path remains /api/v1.
- UI default target remains http://localhost:5152/api/v1, overridable via VITE_API_BASE_URL.
- Submit requests require Idempotency-Key.
- Optimistic concurrency on products is enforced with Version.
- Split payment output currently returns exactly three whole-number shares (no cents), with remainder whole unit assigned to payer 1.