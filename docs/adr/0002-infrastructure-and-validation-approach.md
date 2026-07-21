# ADR 0002: Infrastructure and Validation Approach

## Status

Accepted

## Context

Phase 2 requires a practical backend baseline with persistence, validation, and extensibility while staying inside a short assessment time box.

## Decision

The solution adopts the following approach for Part One:

- Use `CheckoutSystem.Infrastructure` as the persistence boundary.
- Use Entity Framework Core with SQLite in code-first mode.
- Seed a small fixed product catalogue at startup.
- Keep business calculation logic in `CheckoutSystem.Application` and `CheckoutSystem.Domain`.
- Add a lightweight validator engine in the application layer to validate incoming order calculation requests.
- Apply discount before tax and perform rounding at order level.
- Defer Part Two split-payment implementation until Part One is complete and stable.

## Rationale

- The stack aligns with assessment constraints and demonstrates good separation without excessive complexity.
- Code-first plus seed data enables a reproducible local environment for reviewers.
- A custom simple validator engine avoids overengineering while still providing structured validation behavior.
- Explicit order-level rounding avoids cumulative per-line rounding drift.

## Consequences

- Validation logic is explicit and testable but currently lightweight.
- Seed data is intentionally static and suited only for proof-of-concept use.
- The model remains extensible for later addition of order splitting and richer catalog management.