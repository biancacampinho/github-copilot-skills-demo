---
name: skill-name
description: <!-- WHAT it does and WHEN to use it. Be specific with trigger phrases the user would actually say. This field decides whether the skill activates automatically. -->
---

<!--
=====================================================
GENERIC SKILL TEMPLATE — fork this and fill it in

FIXED SKELETON (keep these headers in every skill):
  GENERAL
    - YOUR ROLE
    - MODEL TO EXECUTE THE SKILL
    - OUTPUT
  STEPS TO FOLLOW
  RULES

Everything else — the individual steps under
"STEPS TO FOLLOW" and the rule(s) under "RULES" — is
EXAMPLE CONTENT. Replace it with whatever steps and
rules actually fit the skill you're building. A skill
can have 2 steps or 6; one rule or none.
=====================================================
-->

## GENERAL

### YOUR ROLE
<!-- Who/what does this skill make the agent "be" during execution?
Ex: "You are a technical documentarian. Your only job is X, never Y." -->

### MODEL TO EXECUTE THE SKILL
<!-- Which model/profile is recommended for this skill (if relevant).
Ex: a stronger-reasoning model for deep analysis,
a faster model for simple, repetitive tasks. -->

### OUTPUT
<!-- Where the result is saved, in what format, and the file naming convention.
Ex: `docs/<type>/YYYY-MM-DD-description.md` -->

---

## STEPS TO FOLLOW

<!-- EXAMPLE — replace with the steps that fit your skill.
The names, order, and number of steps below are just one
possible pattern, not a required format. -->

### Example step — READ
<!-- What needs to be read/gathered before any analysis? -->

### Example step — ANALYZE
<!-- How to break down and investigate the problem/question. -->

### Example step — OPEN QUESTIONS
<!-- What to do when ambiguities or gaps come up. -->

### Example step — DOCUMENT
<!-- Final output format and how to present results to the user. -->

---

## RULES

<!-- EXAMPLE — one possible rule. Add, remove, or replace with
whatever behavioral rules matter for your skill. This section
can also be empty if the skill doesn't need special rules. -->

**Example rule:** if the research/analysis document for this repository already exists, do NOT execute this skill again. Instead, tell the user:

```
The research for this repository has already been executed. My notes are here: <path>.
```

And give a summary of what was found.
