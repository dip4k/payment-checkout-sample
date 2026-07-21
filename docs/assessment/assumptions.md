# Assessment Assumptions

These assumptions reduce ambiguity in the brief and keep the proof of concept implementable within the time budget.

## Functional Assumptions

- The product catalogue is locally managed and seeded with a small fixed dataset.
- Each product has a name, unit price, and taxability flag.
- Order line quantities are positive whole numbers.
- The standard tax rate is fixed at 20% for taxable items only.
- Discounts apply to the whole order, not to individual lines.
- A discount may be either percentage-based or fixed-amount, but not both at the same time.
- Fixed discounts cannot reduce the payable amount below zero.

## Validation Assumptions

- API input validation is enforced using a simple validator engine with composable validation rules.
- Validation includes required fields, positive quantities, non-negative prices, and discount constraints.
- Invalid requests return structured error payloads and are not processed further.

## Calculation Assumptions

- Monetary values are stored and calculated using `decimal`.
- Discount is applied before tax.
- Tax is calculated only on taxable line amounts after discount.
- Rounding is applied at order level, not per line.
- Monetary outputs are rounded to 2 decimal places using one consistent rounding rule across the service.

## Persistence Assumptions

- SQLite is sufficient for both catalogue and order persistence in this proof of concept.
- Persistence uses a dedicated infrastructure layer with Entity Framework Core code-first approach.
- The database is seeded with a minimal fixed product catalogue for local development.
- A local SQLite file in the API project is acceptable for development and assessment submission.

## Part Two Assumptions

- Part Two (split payments) was intentionally deferred until Part One was complete.
- When implemented, order splitting applies after the final order total is calculated.
- An order can currently be split into exactly 3 equal shares because that is the stated requirement.
- For this POC, split shares are represented as whole-number amounts only (no cents).
- Any remainder from equal whole-number distribution is assigned to payer 1.
- Because split shares are whole-number-only, the sum of shares is aligned to a rounded whole split base and can differ from the two-decimal displayed total.

## Enterprise API Phase Assumptions

- Enterprise-grade concerns are captured in ADRs and API contracts during this assessment, even if full behavior is partially deferred by the time box.
- Idempotency for order submission is treated as mandatory design intent and should be represented as a request contract seam.
- Optimistic concurrency is the expected strategy for mutable writes when stale client data is submitted.
- Domain events are preferred for side effects to avoid coupling order logic with external integration actions.
- Correlation id propagation and structured logging are baseline observability expectations for API flows.

## Delivery Assumptions

- Unit tests are intentionally deferred due to time constraints and should be called out explicitly in the final decision log.
- The UI is a simple operator-facing screen, not a production-polished customer checkout journey.