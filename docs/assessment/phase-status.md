# Phase Status

## Current Status

- Phase 1 completed: workspace instructions, ADR baseline, assumptions, implementation plan, and decision-log scaffolding are in place.
- Phase 2 started: backend projects are wired, template API endpoint removed, and initial `/api/v1` checkout endpoints are implemented.
- SQLite persistence baseline is wired with EF Core code-first mapping and startup seed data.
- Centralized exception middleware and application-layer validation baseline are implemented.
- Enterprise API concerns have been elevated into ADRs and phase planning for the next implementation slice.
- Phase 3 subset implemented:
	- Correlation id middleware now accepts/generates `X-Correlation-ID` and returns it in responses.
	- Idempotent order submission seam implemented via `POST /api/v1/orders/submit` with required `Idempotency-Key` and persisted replay behavior.
	- Optimistic concurrency metadata added through product `Version` and request `productVersion` checks with `409 Conflict` behavior.
- Additional backend hardening implemented before UI phase:
	- Domain event dispatcher abstraction added with submitted-order event publication.
	- Immutable order line-item snapshots are persisted at submit time.
	- Append-only order status history baseline is persisted for submitted orders.
	- Submit endpoint orchestration moved to application command handler with unit-of-work transaction boundary.
	- Idempotency handling strengthened with deterministic race-safe save-if-absent behavior.
	- Outbox message persistence seam added for reliable integration event handling.
	- Startup database initialization now applies EF Core migrations instead of ensure-created.
	- Product version increment strategy formalized in DbContext save pipeline for future mutable writes.
- Phase 5 implemented:
	- React and Vite UI scaffold added under `ui/` using a small feature-oriented structure and plain CSS.
	- Catalogue selection UI is wired to `GET /api/v1/catalogue`.
	- Totals calculation UI is wired to `POST /api/v1/orders/calculate` with auto-recalculation on valid quantity/discount edits.
	- Order submission UI is wired to `POST /api/v1/orders/submit` with internal idempotency/correlation handling.
	- Submitted-order summary is shown after successful submission.
	- Development CORS policy added to the API to support local browser-based integration.
- API documentation improved:
	- Swagger UI is exposed in development for the versioned checkout endpoints.
	- Request and response examples now document catalogue retrieval, order calculation, and idempotent order submission.
	- Enum payloads are now serialized as strings to align API docs with UI requests.
- Phase 6 implemented:
	- Part Two split-payment support now extends the existing order calculation result without changing request contracts.
	- `POST /api/v1/orders/calculate` and `POST /api/v1/orders/submit` now return a three-way `splitShares` breakdown from the final total.
	- The React order workspace now displays the three-way split beneath the totals summary.
	- Idempotent submit replay returns the same split share distribution as the original submission.
- Phase 7 implemented:
	- Final documentation consistency pass completed across ADRs, assessment notes, and wiki pages.
	- Production hardening notes were refreshed to separate already-implemented baseline controls from deferred improvements.

## Next Recommended Step

- Submission package is implementation-complete for the planned POC phases.
- Optional final cleanups: remove non-blocking package vulnerability warnings and run one final interactive smoke demo recording.

## Notes For Next Session

- Unit tests remain intentionally deferred due to scope and time box.
- Resilience policies for external dependencies remain design-level only because no outbound HTTP integration exists yet.
- Outbox dispatch worker remains pending and should be added when outbound integrations are introduced.
- UI expects the API at `http://localhost:5152/api/v1` by default and can be overridden via `VITE_API_BASE_URL`.
- Split-payment logic currently targets exactly 3 whole-number shares (no cents), with any remainder whole unit assigned to payer 1.