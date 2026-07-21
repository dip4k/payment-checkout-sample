# ADR 0001: Baseline Solution Approach

## Status

Accepted

## Context

The assessment requires a small but extensible checkout application using C#, .NET APIs, SQLite, and React. The brief is intentionally ambiguous and time-boxed, so the design must balance clarity, speed, and extension points for Part Two.

## Decision

We will build a modular proof of concept with these characteristics:

- ASP.NET Core Web API backend under `api/`.
- React frontend under `ui/`.
- SQLite persistence for product catalogue and order data.
- Clean architecture direction: domain logic separated from transport and persistence concerns.
- Versioned HTTP contracts under `/api/v1`.
- Catalogue seeded locally rather than sourced from an external system.
- Calculation logic implemented as a dedicated service so Part Two can extend totals into payment shares without rewriting order rules.

## Rationale

- The brief values judgement more than breadth, so the solution should expose decisions clearly.
- SQLite keeps setup small while still demonstrating persistence and data modeling.
- A dedicated calculation service isolates the most important assessment logic: tax, discounts, totals, and later split payments.
- Versioned contracts and modular boundaries show how the proof of concept would evolve without requiring enterprise-scale ceremony.

## Consequences

- Additional backend projects may be introduced as implementation begins if they materially improve separation of concerns.
- Some infrastructure concerns will remain intentionally lightweight because this is a proof of concept.
- Assumptions must be documented whenever the brief does not fully define a business rule.