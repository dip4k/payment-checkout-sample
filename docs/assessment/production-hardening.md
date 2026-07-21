# Production Hardening Notes

If this were being built beyond the assessment proof of concept, these would be the next improvements.

## Already Implemented In POC Baseline

- Migration-based startup and seeded SQLite persistence.
- Centralized exception handling and structured validation error flow.
- Idempotent order submission with replay-safe persistence seam.
- Optimistic concurrency checks on submit.
- Correlation ID propagation middleware.
- Immutable line snapshots, append-only order status history, and outbox persistence seam.

## Engineering Improvements

- Add unit, integration, and API contract tests for calculation and persistence behavior.
- Expand endpoint coverage for validation and negative-path contract tests.
- Add environment-specific configuration and operational health checks.
- Add authentication and authorization if order access becomes user-specific.
- Introduce CI checks for formatting, build, tests, and static analysis.
- Add package-vulnerability remediation workflow and dependency update automation.

## Domain and Product Improvements

- Support configurable tax rates rather than a fixed constant.
- Support richer catalogue management and product categories.
- Expand split-payment behavior beyond a fixed three-way split.
- Capture audit history for order recalculations and updates.
- Add explicit split-payment reconciliation fields when split semantics differ from display total precision.
- Formalize versioned domain-event contracts and external integration schemas.

## Operational Improvements

- Add metrics and distributed tracing (OpenTelemetry) in addition to current correlation-ID logs.
- Move secrets and environment configuration out of local files.
- Consider a production database and concurrency strategy once multi-user behavior matters.
- Add resilience policies (retry with jitter, timeout, and circuit breaker) for external service dependencies.