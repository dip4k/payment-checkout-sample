---
applyTo: "api/**/*.cs"
description: "Use when editing the .NET backend for the checkout assessment. Covers clean architecture, versioned APIs, money calculation rules, XML docs, and proof-of-concept constraints."
---

# API Implementation Guidance

- Keep business rules independent from HTTP concerns.
- Prefer layers or projects that separate domain, application, infrastructure, and API contracts when the added structure pays for itself.
- Keep endpoints thin and push calculation logic into named services.
- Use `decimal` for prices, discounts, tax, subtotal, and totals.
- Round monetary values deliberately and consistently; document the chosen rule in the assumptions file before implementation.
- For ambiguous financial behavior, encode the simplest defensible rule and record it in `docs/assessment/assumptions.md`.
- Use XML documentation comments on all public contracts and methods.
- Keep DTOs explicit and version-friendly.
- Add centralized exception handling and structured logging before adding multiple endpoints.
- Treat order submission as idempotent; model an `Idempotency-Key` contract and persistence seam.
- Add optimistic concurrency support for mutable entities using row version or ETag-compatible contracts.
- Keep domain logic decoupled through domain events and handlers so side effects are extensible.
- Flow `X-Correlation-ID` through middleware and log scopes for every request.
- Do not couple business services directly to remote calls; isolate external dependencies behind interfaces ready for retry/circuit-breaker policies.
- Preserve financial history by snapshotting line-item price and taxability at checkout boundaries.