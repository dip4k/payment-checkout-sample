# ADR 0003: Enterprise API Considerations for Order and Product Domain

## Status

Accepted

## Context

The assessment implementation is time-boxed, but order/product domains in production involve distributed system risks that must be explicitly designed for. Senior-level design quality requires these concerns to be addressed in architecture now, even when full implementation is deferred.

## Decision

Phase 3 introduces an explicit API Development (Enterprise Considerations) slice with these design commitments:

- Idempotency:
  - Order submission endpoints should require `Idempotency-Key`.
  - The API contract and persistence seam must prevent duplicate order creation from retries.
- Optimistic concurrency:
  - Mutable resources should include row version or ETag-compatible concurrency metadata.
  - Write operations should return `409 Conflict` when stale data is submitted.
- Domain events:
  - Core order actions should publish domain events (for example, `OrderPlaced`) through an internal dispatcher abstraction.
  - Side effects (notifications, inventory updates, integrations) should be handled by event handlers instead of direct service coupling.
- Observability:
  - Middleware should accept or generate `X-Correlation-ID` and propagate it through logs and downstream calls.
  - Structured logging with named properties should be used for order lifecycle events.
- Resilience seams:
  - Any external product/catalogue dependency should be behind an interface and policy-ready for retry and circuit-breaker behavior.
- Auditability and immutability:
  - Order line items must snapshot price and taxability at checkout time.
  - Status transitions should support append-only audit trail patterns.

## Rationale

- These concerns are common failure points in enterprise commerce workflows.
- Capturing the seam now improves extensibility without forcing full feature build in the assessment window.
- This demonstrates architectural judgment while preserving focus on Part One deliverables.

## Consequences

- Additional interfaces and contracts may exist before full runtime behavior is implemented.
- Some concerns remain intentionally partial in this proof of concept and must be called out in submission notes.
- The architecture remains compatible with future event-driven and distributed-service extensions.