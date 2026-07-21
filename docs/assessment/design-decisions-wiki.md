# Design Decisions Wiki

This file is a concise running index of implementation-level design decisions for quick review.

## Core Decisions

- Architecture style: modular clean-architecture direction with API, Application, Domain, and Infrastructure projects.
- API strategy: versioned contracts and routes under `/api/v1`.
- Money type: `decimal` for all financial values.
- Tax rule: fixed standard rate 20% for taxable items.
- Discount rule: apply order-level discount before tax.
- Rounding rule: round at order level to 2 decimal places.
- Validation strategy: simple validator engine in application layer with structured errors.
- Persistence strategy: EF Core code-first with SQLite and startup seed.
- Scope control: Part Two was deferred until Part One completion and is now implemented as a thin extension.

## Enterprise API Development Phase

- Idempotency: order submission contracts will use `Idempotency-Key`.
- Concurrency: mutable writes will adopt row version or ETag-compatible conflict handling with `409 Conflict` on stale writes.
- Domain events: order lifecycle side effects will be decoupled behind an event dispatcher abstraction.
- Observability: request correlation via `X-Correlation-ID` and structured logs for order lifecycle milestones.
- Resilience: external catalogue access, when introduced, will be behind policy-ready interfaces.
- Auditability: order line items should snapshot price/taxability and preserve append-only state transition history.

## Enterprise API Progress

- Implemented: correlation id middleware (`X-Correlation-ID`) with response propagation.
- Implemented: idempotency seam for order submission using `Idempotency-Key` and persisted response replay.
- Implemented: optimistic concurrency checks using product `Version` metadata and request `productVersion`.
- Implemented: domain event dispatcher and submitted-order event handler seam.
- Implemented: audit-trail entities and immutable persisted line-item snapshots for submitted orders.
- Implemented: application-level submit command handler that owns idempotency, concurrency validation path, event publishing, and submission orchestration.
- Implemented: transaction boundary via unit-of-work wrapper for submit flow.
- Implemented: deterministic idempotency collision handling with save-if-absent semantics.
- Implemented: outbox-ready persistence seam (`OutboxMessages`) for integration event reliability.
- Implemented: migration-based startup flow (`MigrateAsync`) with generated initial migration.
- Implemented: product version increment strategy on updates via DbContext save pipeline.

## Ready For UI

- Backend now supports catalogue fetch, order calculation preview, and idempotent order submission with persisted audit snapshot and status-history baseline.
- Architecture and request-flow diagram recorded in `docs/wiki/backend-architecture-and-data-model.md` for quick reviewer navigation.

## Part Two Progress

- Implemented: three-way split-payment breakdown added to the existing order calculation and submit responses.
- Implemented: split-payment rendering added to the React order summary without changing the request contract.
- Updated: split output now returns whole-number shares (no cents) and assigns remainder whole unit to payer 1.
- Updated: UI auto-recalculates totals on valid quantity/discount changes.
- Updated: idempotency and correlation identifiers remain internal UI submit concerns and are not exposed as editable fields.

## Reviewer Navigation

- Enterprise improvement plan and target-state diagrams are recorded in `docs/wiki/enterprise-improvement-plan.md`.
- Enterprise delivery roadmap and estimated implementation slices are recorded in `docs/wiki/enterprise-delivery-roadmap.md`.

## Implementation Notes (Phase 2 Start)

- Replace template weather endpoint with checkout-focused endpoints.
- Seed a minimal catalogue with taxable and zero-rated items.
- Centralize exception handling and logging before expanding endpoint count.

## Open Items

- Add outbox processing worker when external integrations are introduced.