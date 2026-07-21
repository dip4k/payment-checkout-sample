---
applyTo: "ui/**"
description: "Use when editing the React frontend for the checkout assessment. Covers modular structure, plain CSS, no UI libraries, and minimal proof-of-concept scope."
---

# UI Implementation Guidance

- Build a small, understandable React app focused on the assessment flow.
- Prefer feature-oriented folders over a flat component dump.
- Use plain CSS modules or scoped CSS files; do not add a UI framework.
- Keep state predictable and colocated near the feature that owns it.
- Make calculation results easy to inspect: subtotal, discount, tax, and total should always be visible.
- Preserve room for Part Two by keeping order summary logic separate from presentation.