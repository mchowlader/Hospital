---
name: git-shipping
description: >-
  Automates Hospital git workflow — branch, logical commits, push, and PR draft.
  Use when the user asks to commit, push, create a branch, draft a PR, or run /ship.
---

# Git Shipping & Workflow Automation Guide

Use this skill when the user runs `/ship`, asks to commit code, create branches, stage files, or draft a Pull Request.

## Workflow Execution Steps

Follow these steps in sequence when this skill is triggered:

### 1. Analyze Working Directory
* Execute `git status` to identify modified, untracked, and deleted files.
* Note: Always prefer using standard Command Prompt (`cmd /c`) or explicit environment paths if PowerShell throws a `DriveNotFoundException`.
* Analyze the diff of the files to understand what changes were made.

### 2. Branch Management (if on default branch or new feature)
* If the user is currently on `main` or `master` and has untracked/modified changes, propose switching to a new feature branch.
* Generate a perfect, descriptive branch name based on the modifications.
  * *Pattern:* `feature/short-description` or `fix/bug-description` or `refactor/clean-up` (e.g., `feature/add-supplier-entity`, `style/separate-buyer-components`).
* Switch to the branch using:
  ```powershell
  git checkout -b <branch-name>
  ```

### 3. Group Changes into Logical Chunks
Do **NOT** commit everything in one single giant commit. Group the changes into separate logical commits:
* *Database/Core changes:* Entity creation, configurations, migrations.
* *API/Backend changes:* Endpoints, Handlers, DTOs, Validators.
* *UI/Frontend changes:* Razor markup, Code-behind classes, Scoped CSS files.

### 4. Stage and Commit Chunks
For each logical group:
1. Stage only the files belonging to that group:
   ```powershell
   git add <specific-files-or-folders>
   ```
2. Write a high-quality commit message following the **Conventional Commits** standard:
   * **Language Constraint:** All commit messages **MUST** be written strictly in English. Do **NOT** use Bengali or any other language under any circumstances.
   * *Examples:*
     * `feat(api): add create buyer endpoint and validator`
     * `style(ui): separate code-behind and isolated css for buyer view`
     * `db(infra): configure buyer entity unique constraints and run migration`
3. Run the commit command:
   ```powershell
   git commit -m "conventional commit message"
   ```

### 5. Push Branch & Generate Pull Request Link
* Run the push command automatically:
  ```powershell
  git push origin <branch-name>
  ```
* Output a formatted Markdown summary representing a **Pull Request (PR) Draft**:
  * **PR Title:** High-level summary (e.g., `feat: Implement Buyer Management feature`).
  * **Description:** Bullet-pointed list of what was done.
  * **Commits:** List of generated commits in this flow.
  * **PR Link:** Display the exact URL to create a PR on GitHub (e.g., `https://github.com/mchowlader/Hospital/pull/new/<branch-name>`).
