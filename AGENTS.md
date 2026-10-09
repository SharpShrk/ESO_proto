# ESO_proto — Codex Guidelines

## Project Context

This is a Unity 2D game project.

- Unity version: 6000.6.4f1
- Render pipeline: Universal Render Pipeline (URP)
- Input: Unity Input System
- Main project content: `Assets/_Project/`
- Main scripts: `Assets/_Project/Scripts/`

Existing script areas include:
- `Core`
- `Gameplay`
- `Player`
- `Narrative`
- `Save`
- `UI`
- `Editor`

Treat the existing project architecture as the source of truth. Before implementing a feature, inspect the relevant existing code and understand how the affected systems currently interact.

## General Development Principles

Follow these principles:

- SOLID
- DRY
- KISS
- Prefer composition over inheritance when appropriate.
- Avoid unnecessary abstractions.
- Avoid overengineering.
- Prefer simple, explicit solutions over clever ones.
- Keep systems loosely coupled where practical.
- Keep gameplay/domain logic independent from UI where practical.

Do not introduce a new architectural pattern, framework, service layer, abstraction, or dependency unless it solves an actual problem in this project.

When extending an existing system, prefer integrating with its current architecture rather than creating a parallel implementation.

## C# Code Style

Use four-space indentation.

Opening braces go on a new line.

Use:

- `PascalCase` for classes, structs, enums, methods, properties, and public members.
- `_camelCase` for private fields.
- `camelCase` for parameters and local variables.

Serialized private fields should normally use:

```csharp
[SerializeField] private SomeType _fieldName;
```

Prefer explicit access modifiers.

File names should match their primary type.

Do not perform unrelated formatting or rename existing symbols unless necessary for the task.

Avoid unnecessary comments. Comments should explain intent, constraints, or non-obvious behavior rather than restating the code.

## Unity-Specific Rules

Preserve Unity `.meta` files.

Do not manually create, delete, rename, or modify `.meta` files unless the task explicitly requires it and the implications are understood.

Do not modify the following unless the task explicitly requires it:

- `ProjectSettings/`
- `Packages/manifest.json`
- `Packages/packages-lock.json`

Do not add or remove Unity packages without explicit approval.

Do not modify scenes or prefabs unless the requested task requires it.

When a task requires changes to scenes, prefabs, ScriptableObjects, serialized data, or other Unity assets, explain what will be changed before making broad or potentially destructive modifications.

Do not edit generated directories such as:

- `Library/`
- `Temp/`
- `Logs/`
- `Obj/`

Be aware of Unity serialization rules when designing data structures.

Do not assume that standard .NET behavior automatically maps cleanly to Unity serialization or UnityEngine.Object lifetime semantics.

## Architecture Changes

Before a significant architectural change or refactor:

1. Inspect the relevant implementation.
2. Identify dependencies and usages.
3. Explain the problem being solved.
4. Propose the intended change.
5. Identify the files/systems likely to be affected.

For small and localized changes, implementation can proceed directly.

For large refactors, architectural changes, public API changes, or changes affecting multiple major systems, present a short plan before implementation.

Do not rewrite a working system merely because another architecture is theoretically cleaner.

Preserve existing behavior unless the requested task explicitly changes it.

## Scope Control

Make the smallest coherent change that fully solves the requested problem.

Do not:

- refactor unrelated systems;
- rename unrelated files or symbols;
- introduce unrelated features;
- change formatting across unrelated files;
- add speculative infrastructure for hypothetical future requirements.

If an adjacent issue is discovered but is outside the requested scope, mention it instead of automatically fixing it.

## Dependencies

Before introducing a new third-party dependency:

1. Check whether the project already contains a suitable solution.
2. Explain why the dependency is necessary.
3. Ask for approval before adding it.

Prefer existing project dependencies and Unity-provided functionality when reasonable.

## Error Handling

Do not silently swallow exceptions.

Avoid broad `catch (Exception)` blocks unless there is a clear reason.

For expected failure states, prefer explicit handling.

Unity logs should provide useful diagnostic context without producing unnecessary log spam.

## Testing and Validation

Before considering a programming task complete:

- inspect the resulting diff;
- check for obvious compile errors;
- check affected call sites;
- check for broken references caused by renamed or moved code;
- consider Unity-specific lifecycle and serialization implications.

Use existing tests when available.

Do not invent a project-wide testing convention unless requested.

If automated verification cannot validate Unity-specific behavior, clearly state what must be verified manually in the Unity Editor.

Never claim that a Unity scene, prefab, animation, visual behavior, or runtime interaction was verified unless it was actually tested in an appropriate environment.

## Git

Treat Git as a safety mechanism.

Before large changes, inspect the current working tree.

Do not:

- discard user changes;
- reset the repository;
- rewrite Git history;
- force push;
- delete branches;

unless explicitly requested.

Do not automatically commit changes unless requested.

When finishing a task, summarize which files were created, modified, or deleted.

## Working With Existing Code

Before modifying an existing system:

- search for usages of the relevant types;
- inspect related interfaces and implementations;
- inspect important callers;
- understand ownership and lifecycle where relevant.

Do not infer how an internal API works solely from its name when its implementation is available.

If existing code contradicts these guidelines, do not automatically rewrite it. Follow these guidelines for new code and modify existing code only when necessary for the requested task.

## Agent Behavior

When asked to analyze something, do not modify files unless explicitly asked to implement changes.

When the user asks a question about the project, answer the question before making changes.

Distinguish clearly between:

- observed behavior/code;
- assumptions;
- recommendations.

If requirements are ambiguous and different interpretations would significantly affect the implementation, ask for clarification.

Minor implementation details that do not materially affect behavior may be decided independently.

Do not claim a command, test, build, or Unity validation succeeded unless it was actually executed successfully.

## Current Development Priorities

The project is early in development.

Maintainability and the ability to iterate quickly are more important than premature optimization or elaborate infrastructure.

The project is expected to contain systems such as gameplay, player logic, narrative/quests/dialogue, saves, and UI.

The narrative system is expected to support nonlinear and branching content, so avoid assumptions that quests or dialogues must always follow a single linear sequence.

Do not design the narrative architecture solely from this note. Inspect its current implementation and follow task-specific requirements.