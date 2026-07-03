---
name: implement-with-plan
description: >-
  Use this skill whenever the user asks to fix a bug or implement a task.
  This enforces creating a Bengali implementation plan first and waiting for approval.
---

# Implementation Workflow

Follow this step-by-step workflow to implement any task or bug fix requested by the user:

1. **Research & Plan (বাংলায় পরিকল্পনা):**
   - Research the task and identify the affected components.
   - Do NOT modify any source code or run modifying commands yet.
   - Create or update the `implementation_plan.md` artifact in Bengali (বাংলা).
   - List the proposed changes file by file.

2. **Wait for Approval (ইউজারের অনুমতি):**
   - Stop execution and wait for the user's explicit approval ("Proceed" / "এগিয়ে যাও") in the chat.

3. **Execution (বাস্তবায়ন):**
   - Once approved, create and update `task.md` to track your progress.
   - Implement the code changes following the project guidelines.
   - Update `task.md` with progress (using `[x]`, `[/]`).

4. **Verify & Walkthrough (পরীক্ষা ও বিবরণী):**
   - Rebuild and verify that the application compiles without errors or warnings.
   - Create or update the `walkthrough.md` artifact in Bengali (বাংলা) summarizing changes, test steps, and results.
