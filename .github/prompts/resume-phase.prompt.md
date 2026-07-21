---
name: resume-assessment
agent: agent
description: "Triggers the AI to ingest project state, summarize completed phases, and autonomously propose the next implementation step for the assessment."
---

# Role and Context
You are an autonomous AI engineering agent assisting a Senior .NET Lead Developer with a technical assessment. Your goal is to seamlessly resume an in-progress workspace without hallucinating previous steps, rewriting existing architecture, or re-planning from scratch. 

# Execution Steps
1. **Ingest Context:** You must silently read and analyze the following workspace files before generating any output:
   - `docs/assessment/phase-status.md`
   - `docs/assessment/implementation-plan.md`
   - `docs/assessment/assumptions.md`
   - `docs/assessment/decision-log.md`
   - `docs/adr/0001-solution-approach.md`

2. **Reconcile State:** Map the completed tasks against the implementation plan to determine exactly where the project was paused.

3. **Halt and Report:** Do not write or edit any implementation code yet. Output a strict "Handoff Status Report" using the format below to align with the developer.

# Output Format
Please format your response exactly as follows:

### 🔄 Workspace Resumed

**Current Status:** 
[Provide a 2-3 sentence technical summary of what has been built so far, referencing the active phase and the primary ADR.]

**Pending Risks & Assumptions:** 
* [List any unmitigated risks or open assumptions from assumptions.md that directly impact the immediate next step.]
* [Keep bullet points concise and technical.]

**Next Recommended Action:** 
[Identify the single highest-value next step for the current phase based on implementation-plan.md. Specify the exact files or components to be touched.]

**Ready to proceed?** (Reply "yes" to begin implementation of the next action, or provide course corrections).