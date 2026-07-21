# Enterprise Delivery Roadmap

This page turns the enterprise target-state plan into a phased delivery roadmap with estimated slices. The intent is to show how the current proof of concept could evolve incrementally without rewriting the working domain core.

## Roadmap Principles

- Keep the existing checkout domain logic as the foundation and harden boundaries around it.
- Deliver risk-reducing platform capabilities before broad feature expansion.
- Prefer vertical slices that include architecture, operations, and rollback strategy rather than isolated code-only milestones.
- Preserve backward compatibility for versioned APIs and events where possible.

## Phase Summary

| Phase | Focus | Estimated Slice | Main Outcome |
| --- | --- | --- | --- |
| 1 | Production baseline | 1 to 2 weeks | Secure runtime, CI, health checks, rate limiting, config hygiene |
| 2 | Application boundary hardening | 1 to 2 weeks | Mapping module, module contracts, clearer application seams |
| 3 | Event-driven foundation | 2 to 3 weeks | Durable outbox publisher, broker integration, versioned events |
| 4 | Observability and audit expansion | 1 to 2 weeks | Traces, metrics, dashboards, richer audit/event history |
| 5 | Notifications and operational workflows | 1 to 2 weeks | Notification service, delivery tracking, operational alerts |
| 6 | Product and order management expansion | 2 to 4 weeks | Richer product lifecycle, order workflows, approvals, search |

## Phase 1: Production Baseline

### Scope

- Add authentication and authorization to all protected API routes.
- Add rate limiting and abuse protection at the API edge.
- Move secrets and configuration into environment-specific managed stores.
- Add readiness and liveness checks.
- Add CI checks for build, lint, tests, and static analysis.
- Add deployment-safe migration handling.

### Deliverables

- API gateway or ingress policy for throttling and auth enforcement.
- OIDC and OAuth 2.1 integration.
- Role and policy definitions for product and order operations.
- Health-check endpoints and pipeline automation.
- Operational runbook for deployment and rollback.

### Why First

- This reduces operational and security risk before the service surface grows.
- It establishes a minimum enterprise hosting baseline without changing the core business logic.

## Phase 2: Application Boundary Hardening

### Scope

- Add dedicated DTO-to-command, DTO-to-domain, and domain-to-response mapping modules.
- Introduce clearer module contracts between order, product, pricing, and notification concerns.
- Reduce direct endpoint assembly logic.
- Add contract and integration tests for calculation, submit, replay, and concurrency behavior.

### Deliverables

- Mapping layer package or module.
- Cleaner endpoint handlers with focused orchestration only.
- Explicit command and query boundaries where useful.
- Stronger test coverage on domain and transport seams.

### Why Here

- This keeps the codebase maintainable before multiple services and event consumers are introduced.

## Phase 3: Event-Driven Foundation

### Scope

- Replace in-process domain event handling with durable outbox publishing.
- Publish versioned integration events to a broker.
- Add idempotent consumer policies and retry handling.
- Introduce dead-letter handling and replay tooling.

### Deliverables

- Broker integration, for example Kafka or Azure Service Bus.
- Outbox dispatcher worker.
- Versioned event contracts such as `OrderSubmitted`, `OrderStatusChanged`, and `ProductUpdated`.
- Consumer-hosting pattern and operational retry policy.

### Why Here

- This unlocks notifications, reporting, and downstream integrations without tight coupling.

## Phase 4: Observability And Audit Expansion

### Scope

- Add OpenTelemetry tracing across HTTP, database, and messaging.
- Add dashboards and alerts for order throughput, latency, replay volume, and broker lag.
- Expand the current order history into a formal immutable audit trail and event log.

### Deliverables

- Centralized logs, traces, and metrics.
- Alert rules and dashboards.
- Append-only audit/event store or equivalent immutable log pattern.
- Query capability for audit reconstruction and investigations.

### Why Here

- Once asynchronous flows exist, operational visibility and forensic traceability become mandatory.

## Phase 5: Notifications And Operational Workflows

### Scope

- Add a dedicated notification service consuming order lifecycle events.
- Support email, SMS, webhooks, and operator alerts.
- Track delivery history, failures, retries, and escalation.

### Deliverables

- Notification templates and channel adapters.
- Delivery log and retry rules.
- Internal operator notification paths.
- Failure reconciliation dashboard.

### Why Here

- Notifications are valuable, but they depend on the eventing and observability foundations from earlier phases.

## Phase 6: Product And Order Management Expansion

### Scope

- Add richer product lifecycle management, categories, pricing rules, and product change history.
- Add richer order states, search, reporting, approval flows, and exception handling workflows.
- Expand split payments beyond a fixed three-way model if required.

### Deliverables

- Product management APIs and UI.
- Operational order management workflows.
- Reporting and search views.
- Configurable split strategy and richer business rules.

### Why Last

- This is feature expansion, not foundational risk reduction.

## Cross-Cutting Workstreams

### Security

- Token validation, policy enforcement, secret rotation, privileged-action review, and data-protection controls.

### Resilience

- Timeout, retry, circuit-breaker, bulkhead, fallback, and backpressure strategies.

### Quality

- Unit, integration, contract, and end-to-end automation.

### Governance

- API versioning policy, event versioning policy, migration standards, and operational runbooks.

## Suggested Increment Sequence

```mermaid
%%{init: {'theme': 'base', 'themeVariables': { 'primaryColor': '#eff6ff', 'primaryTextColor': '#0f172a', 'primaryBorderColor': '#1d4ed8', 'lineColor': '#475569', 'secondaryColor': '#fef3c7', 'tertiaryColor': '#dcfce7', 'background': '#ffffff'}}}%%
flowchart LR
    P0[Current POC Baseline]
    P1[Phase 1\nSecurity and Runtime Baseline]
    P2[Phase 2\nMapping and Module Hardening]
    P3[Phase 3\nOutbox and Broker Integration]
    P4[Phase 4\nObservability and Audit Expansion]
    P5[Phase 5\nNotifications and Operations]
    P6[Phase 6\nFeature Expansion]

    P0 --> P1 --> P2 --> P3 --> P4 --> P5 --> P6
```

## Recommended Team Focus By Slice

- Slice 1: platform engineer plus backend engineer.
- Slice 2: backend engineer plus architect or senior reviewer.
- Slice 3: backend engineer plus integration or platform engineer.
- Slice 4: platform or SRE engineer plus backend engineer.
- Slice 5: integration engineer plus product-facing backend engineer.
- Slice 6: full product squad across backend, UI, and platform.

## Exit Criteria For Enterprise Readiness

- All protected endpoints require authenticated and authorized access.
- Submit writes are idempotent end to end, including downstream consumers.
- Core workflows are traceable across API, storage, and messaging.
- Audit history is immutable and queryable.
- Notifications are asynchronous, durable, and observable.
- Product and order operations have clear module ownership and mapping boundaries.

## Recommendation

Treat this roadmap as a sequence of hardening layers around the current proof-of-concept core. The fastest path to enterprise quality is evolutionary: secure the runtime, formalize boundaries, introduce durable messaging, expand observability, and only then broaden product scope.