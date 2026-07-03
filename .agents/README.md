# Hospital Management System AI Agent Configuration

This folder contains the configuration, rules, and task-specific skills for AI agents (Gemini, Antigravity, etc.) used in the Hospital Management System project.

This directory is committed to version control so that all team members and agents share the same guidelines and workflow skills.

## Structure

```text
.agents/                    # Main Agent Configuration Folder (Tracked in Git)
├── README.md               # This file
├── rules/                  # Always-on + file-scoped rules (.mdc)
│   ├── hosp-core-architecture.mdc
│   ├── hosp-api-handlers.mdc
│   ├── hosp-database-entities.mdc
│   └── hosp-blazor-ui.mdc
└── skills/                 # Task-specific workflow skills
    ├── add-new-feature/
    ├── clean-slice-api-design/
    ├── clean-slice-database/
    ├── clean-slice-ui-design/
    ├── cleanup-garbage-code/
    ├── database-migrations/
    └── git-shipping/
```

## Rules vs Skills

| Type | Folder | When applied |
|------|--------|--------------|
| **Rules** | `rules/*.mdc` | Automatically loaded when working on matching file types or globally |
| **Skills** | `skills/*/SKILL.md` | Manually/automatically triggered for specific tasks |

## Guidelines

- All agent instructions must follow the core guidelines in `AGENTS.md` (located in the project root).
