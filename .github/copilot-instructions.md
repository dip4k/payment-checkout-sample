# Payment Solution Workspace Instructions

This workspace is for an interview assessment and should be treated as a time-boxed proof of concept.

## Scope

- Use `api/` for the .NET backend and `ui/` for the React frontend.
- Prefer a modular clean-architecture layout that is easy to extend without overengineering the initial slice.
- Implement only the smallest end-to-end baseline needed for the assessment before adding refinements.
- Record assumptions before encoding behavior where the brief is ambiguous.

## Backend Expectations

- Use C# and ASP.NET Core APIs with versioned routes under `/api/v1`.
- Keep request and response contracts versioned and separate from domain models.
- Use SQLite for persistence in this proof of concept.
- Use `decimal` for all money calculations.
- Keep tax, discount, and total calculation logic in application/domain services, not controllers.
- Add XML documentation comments to all public C# classes, records, interfaces, enums, and methods.
- Centralize exception handling and logging.
- Include enterprise API considerations as explicit interfaces and ADR decisions even when full implementation is deferred by time box:
	- Idempotency key handling for order submission.
	- Optimistic concurrency strategy for mutable catalogue/order writes.
	- Domain event publication boundaries for order lifecycle actions.
	- Correlation id propagation and structured logging context.
	- Resilience policies for external dependencies.
	- Auditability and immutable financial snapshots.

## Frontend Expectations

- Use React with a simple modular structure and plain CSS only.
- Avoid UI frameworks and component libraries.
- Keep the UI intentionally small: catalogue selection, order lines, discount entry, and totals breakdown.

## Delivery Expectations

- Optimize for clarity, maintainability, and assessable tradeoffs over feature breadth.
- Skip unit tests unless explicitly requested in the current phase.
- Update the docs in `docs/assessment/` when assumptions, scope, or decisions change.
- Keep a running decision log so the final submission can be assembled quickly.

## Session Handoff

- At the end of each phase, update `docs/assessment/phase-status.md` with what was completed, what changed, and the next recommended step.
- Prefer continuing from existing docs instead of restating prior reasoning in new sessions.
- When enterprise-grade concerns are acknowledged but not fully implemented, document the deferral and integration seam in ADRs and decision logs.