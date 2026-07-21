# Decision Log

## Initial Decisions

- Use a modular proof-of-concept architecture instead of a single large API project so calculation logic remains isolated.
- Use SQLite for local persistence to match the requested stack and keep setup friction low.
- Keep the UI intentionally simple and use plain CSS only.
- Defer unit tests due to the assessment time box and record that tradeoff explicitly.

## Phase 2 Decisions

- Added explicit Infrastructure and Application projects to keep persistence and business logic separated from API transport concerns.
- Selected EF Core code-first with SQLite and migration-based startup for a predictable local proof-of-concept environment.
- Implemented a lightweight validator engine in the application layer instead of introducing an external validation framework.
- Confirmed calculation order as discount-before-tax with order-level rounding.
- Adopted `MidpointRounding.AwayFromZero` for currency rounding in the service.

## Phase 3 API Development Decisions

- Added an explicit enterprise API phase to address distributed-system risks in the order/product domain.
- Committed to idempotency-key request handling as a required seam for order submission safety.
- Committed to optimistic concurrency metadata and conflict response strategy for mutable writes.
- Committed to event-driven extension points so side effects do not couple directly to core order logic.
- Committed to correlation id propagation and structured logging as baseline observability behavior.
- Committed to immutable pricing/tax snapshots and auditable state transition direction for financial integrity.

## Phase 3 Implemented Subset

- Added correlation middleware that propagates `X-Correlation-ID` and enriches logging scope.
- Added idempotency persistence model and repository with unique key per operation.
- Added `POST /api/v1/orders/submit` that requires `Idempotency-Key`, replays prior result for duplicate payloads, and returns conflict for key reuse with a different payload.
- Added product version metadata in catalogue and order-line request contracts for optimistic concurrency checks.
- Added `409 Conflict` exception path for concurrency mismatches.

## Backend Next-Step Decisions Before UI

- Renamed domain `Class1.cs` to `Product.cs` to align file naming with domain intent.
- Introduced domain event abstractions and dispatcher to decouple order side effects from endpoint/service orchestration.
- Added immutable order snapshot persistence and append-only status history as the first auditability baseline.

## Hardening Pass Before UI

- Refactored `POST /api/v1/orders/submit` orchestration into an application-level submit command handler.
- Added a unit-of-work transaction boundary so idempotency persistence and audit/outbox writes execute atomically.
- Replaced naive idempotency insert behavior with deterministic race-safe save-if-absent handling.
- Added outbox message entity and persistence seam for reliable future integration publishing.
- Replaced `EnsureCreated` startup behavior with migration-based startup and generated initial EF Core migration.
- Formalized product concurrency version strategy by incrementing versions on entity updates in DbContext save pipeline.

## UI Implementation And Integration

- Added a minimal React and Vite frontend in `ui/` with feature-oriented structure and plain CSS.
- Bound the UI directly to `GET /api/v1/catalogue`, `POST /api/v1/orders/calculate`, and `POST /api/v1/orders/submit` using the versioned API contracts.
- Kept totals presentation separate from the API client and request-building logic so Part Two order splitting can extend from the current summary slice.
- Added a development CORS policy in the API for local browser-based integration with the React app.

## API Documentation

- Added Swagger UI for the minimal API endpoints so the assessment surface can be explored and exercised interactively.
- Switched HTTP JSON enum serialization to strings so API payloads, UI requests, and Swagger examples all use the same readable contract values.
- Added endpoint-level summaries, problem response documentation, request examples, and submission header examples for the versioned checkout routes.

## Part Two Split Payments

- Extended the existing order calculation result instead of introducing a separate split-payment request flow.
- Calculated payment shares only after the final order total is rounded, preserving the Part One money rules.
- Updated the split to exactly 3 whole-number shares (no cents) per latest requirement and assigned any remainder whole unit to payer 1.
- Reused the same split calculation for idempotent replay paths so initial submit and replayed submit responses stay consistent.

## Final UI Refinements

- Switched order recalculation from manual trigger to auto-recalculation on quantity/discount changes.
- Hid idempotency key and correlation id from the page and kept them as internal submit concerns.
- Added a submitted-order summary panel under the order workspace after successful submission.

## AI Usage Notes

- Use AI to accelerate scaffolding, structure, documentation, and implementation drafts.
- Review generated outputs before accepting them, especially around business rules and architecture boundaries.
- Record any material corrections or rejections here during later phases.

## To Complete Before Submission

- Summarize the highest-value prompts used.
- Note where AI output was adjusted or rejected and why.
- Call out deliberate omissions caused by the time constraint.