# Implementation Plan

## Objective

Deliver the smallest complete proof of concept that satisfies Part One cleanly, then extend it for Part Two without reworking the core design.

## Time-Boxed Phases

### Phase 1: Workspace Setup and Design Baseline

- Add workspace Copilot instructions.
- Capture architecture direction, assumptions, and delivery constraints.
- Create a reusable phase handoff trail for new chat sessions.

### Phase 2: Backend Foundation

- Replace the template API with a clean bootstrap.
- Add centralized exception handling and logging.
- Introduce domain models and calculation services.
- Add SQLite persistence and seeded catalogue data.

### Phase 3: API Development (Enterprise Considerations)

- Add idempotency handling seam and endpoint contract for order submission.
- Introduce optimistic concurrency metadata strategy for mutable resources.
- Add domain-event dispatching abstractions for order lifecycle actions.
- Add correlation id middleware and structured logging scope enrichment.
- Define resilience abstraction for external catalogue dependencies.
- Define immutable line-item snapshot and audit trail model direction.

### Phase 4: Part One API Slice

- Create versioned endpoints to read the catalogue and submit an order.
- Return subtotal, discount applied, tax, and total.
- Keep request and response models explicit and documented.

### Phase 5: React UI Slice

- Scaffold the React app in `ui/`.
- Build a simple screen for catalogue selection, quantities, discount entry, and totals.
- Connect the UI to the Part One API.

### Phase 6: Part Two Extension

- Extend the backend to calculate three-way splits from the final total.
- Expose the split breakdown through the API.
- Add the split display to the UI.

### Phase 7: Submission Docs

- Finalize the decision log.
- Add architecture notes and known limitations.
- Document what would change for a production-grade build.

## Prioritization Rules

- Finish one thin vertical slice before expanding horizontally.
- Prefer observable behavior over premature abstractions.
- Capture rejected ideas only when the tradeoff matters to the assessment.