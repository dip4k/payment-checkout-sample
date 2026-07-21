# Payment Solution

## Problem Summary

This repository contains a time-boxed interview assessment implementation of a checkout system. It delivers an end-to-end .NET + React + SQLite vertical slice, then extends it with Part Two split-payment behavior.

## Original Problem Statement

### AI Software Engineer - Tech Test

### Before You Start

We expect you to use AI tools (Copilot, Claude, ChatGPT, Cursor, or anything else you would normally reach for) to complete this exercise. That is not a loophole, it is the point. We are evaluating how you are empowered by AI to do the job at hand, not whether you can memorise syntax.

What we are assessing is your judgement: the decisions you make, the assumptions you challenge, and how you direct a tool rather than just accept what it gives you. To do that fairly, we need to see your working, not just your output.

### What To Submit

Please return the following three things:

1. Your code: a working solution with the tech we use (C#, React, etc.)
2. Your AI session logs: the full chat history/transcript from whichever AI tool you used. Most tools let you export or copy this directly. If your tool does not support export, screenshots or PDF prints are fine.
3. A short decision log: no more than half a page. What did you ask the AI for? What did you change or reject from its output, and why? We are not looking for polish here, bullet points are fine.

### The Brief

#### Part One

Build an app that lets someone add items from a product catalogue to an order, which then calculates the totals. How the catalogue is structured and sourced is up to you.

Requirements:

1. An order has multiple line items, each with a product name, unit price, and quantity.
2. Some items are taxable, some are zero-rated (e.g. cold takeaway food).
3. The standard tax rate is 20%.
4. A discount can be applied to the whole order, either as a percentage or a fixed amount.
5. The service should return a breakdown: subtotal, discount applied, tax, and total.

#### Part Two

Only open this once you are happy with Part One. It builds directly on what you have already built, and it is designed to test how you extend and adapt an existing design, not how you plan ahead for it. Please do not jump ahead.

Requirement:

1. A table of customers now wants to split one order 3 ways evenly. Extend your solution so an order total can be divided into equal shares, with any rounding remainder allocated to a single payer rather than lost or duplicated.

### A Note On Scope

The brief above is intentionally light on detail in places. That is deliberate. Real requirements rarely arrive fully specified, and we want to see how you handle that: what you ask about, what you assume, and how you justify the assumptions you make.

There is not a single correct interpretation. Where you set the bar and how you show off is also completely up to you.

### Logistics

- Time: aim for no more than 2-3 hours total across both parts. We are not clock-watching, but please do not over-invest.
- Return by: reply to the assessment email with your submission.
- Questions: if anything is unclear, email us. We are expecting it.

## Submission Snapshot

Implemented scope:

- A versioned ASP.NET Core API under `/api/v1`.
- Product catalogue retrieval, order calculation, and order submission.
- Calculation output with subtotal, discount, tax, total, and Part Two three-way split shares.
- Idempotent order submission with optimistic concurrency checks.
- SQLite persistence for products, orders, idempotency records, snapshots, status history, and outbox seam.
- A React operator UI for catalogue selection, totals review, submission, split display, and submitted-order summary.
- Swagger UI for API exploration and example payloads.
- Assessment docs, ADRs, architecture diagrams, and enterprise follow-up planning.

## Relevant Documents

### ADRs

- [Solution approach](docs/adr/0001-solution-approach.md)
- [Infrastructure and validation approach](docs/adr/0002-infrastructure-and-validation-approach.md)
- [Enterprise API considerations](docs/adr/0003-enterprise-api-considerations.md)

### Assessment And Design Notes

- [Assumptions](docs/assessment/assumptions.md)
- [Implementation plan](docs/assessment/implementation-plan.md)
- [Decision log](docs/assessment/decision-log.md)
- [Phase status](docs/assessment/phase-status.md)
- [Design decisions wiki index](docs/assessment/design-decisions-wiki.md)
- [Production hardening notes](docs/assessment/production-hardening.md)

### Wiki Pages

- [Application architecture and flow](docs/wiki/application-flow.md)
- [Backend architecture and data model](docs/wiki/backend-architecture-and-data-model.md)
- [Enterprise improvement plan](docs/wiki/enterprise-improvement-plan.md)
- [Enterprise delivery roadmap](docs/wiki/enterprise-delivery-roadmap.md)

## Tech Stack Summary

### Backend

- .NET 10
- ASP.NET Core minimal APIs
- C#
- Entity Framework Core
- SQLite
- Swashbuckle for Swagger UI

### Frontend

- React 19
- TypeScript
- Vite
- Plain CSS

## Architecture Summary

The current implementation follows a modular clean-architecture direction:

- `CheckoutSystem.Api`: HTTP transport, middleware, contracts, and endpoint wiring.
- `CheckoutSystem.Application`: use cases, calculation logic, validation, commands, and orchestration.
- `CheckoutSystem.Domain`: domain types and business concepts.
- `CheckoutSystem.Infrastructure`: persistence, repository implementations, transaction boundaries, and outbox seam.
- `ui/`: React operator console for interacting with the API.

## Key Behaviors

- Monetary calculations use `decimal`.
- Discounts are applied before tax.
- Tax is calculated only on taxable items.
- Monetary values are rounded to 2 decimal places using `MidpointRounding.AwayFromZero`.
- Part Two split logic rounds the final total to a whole-unit amount for split purposes, returns exactly 3 whole-number shares, and assigns any remainder to payer 1.
- Order submission requires `Idempotency-Key`.
- Product version metadata is used for optimistic concurrency checks on submit.

### UI Behavior Notes

- Totals are recalculated automatically after quantity/discount edits (debounced) when inputs are valid.
- Idempotency key and correlation id are handled internally in the UI submit flow and are not exposed as editable fields.
- A submitted-order summary is shown after successful submit.

## How To Run

### Prerequisites

- .NET SDK 10
- Node.js and npm

### Run The API

```powershell
Set-Location api/CheckoutSystem.Api
dotnet run
```

The API runs from the launch settings profile and is available at `http://localhost:5152` in local development.

Swagger UI:

- `http://localhost:5152/swagger`

### Run The UI

```powershell
Set-Location ui
npm install
npm run dev
```

The UI runs at:

- `http://localhost:5173`

By default, the UI calls:

- `http://localhost:5152/api/v1`

You can override the API base URL with:

```text
VITE_API_BASE_URL=http://localhost:5152/api/v1
```

See [ui/.env.example](ui/.env.example).

## Build Commands

### Backend

```powershell
Set-Location api
dotnet build CheckoutSystem.slnx
```

### Frontend

```powershell
Set-Location ui
npm run build
```

## Current Enterprise Seams

The implementation already includes initial seams for concerns that matter in a production system:

- Correlation ID middleware.
- Centralized exception handling.
- Idempotent order submission.
- Optimistic concurrency metadata.
- Domain event dispatcher abstraction.
- Outbox persistence seam.
- Immutable order line snapshots.
- Append-only order status history baseline.

## Deferred By Design (POC)

These are intentionally deferred due to assessment scope:

- Full authentication and authorization.
- Real event broker and asynchronous consumers.
- Notification service.
- Distributed tracing and full observability stack.
- Full audit/event-log platform.
- Rich product management workflows.
- Dedicated mapper module for DTO, domain, and persistence translations.
- Automated test suite beyond manual build and smoke validation.

## Why This Scope

This repository is intentionally optimized for an interview assessment. The goal was to demonstrate:

- a working vertical slice
- clean extension seams
- reasonable architectural judgment
- clear documentation of what is deferred and why

The broader enterprise direction is documented rather than fully built so the implementation stays focused and reviewable.

## Reviewer Notes

- The backend currently emits a small number of non-blocking warnings, including package vulnerability advisories and one nullable warning in the Swagger operation filter.
- The enterprise target-state and phased evolution plan are documented in the wiki pages rather than implemented directly in the proof-of-concept runtime.