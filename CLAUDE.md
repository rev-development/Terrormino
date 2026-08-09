# Terrormino — Claude Project Memory

Learned behaviors and project-specific guidance for Claude instances working in this repo.
Sourced from the developer's global preferences — assume all contributors want to code this way.

---

## Environment

- **Unity version:** Check project files to determine the version; do not assume.
- Unity uses a modified .NET runtime. Some versions expose .NET 10-exclusive features despite targeting .NET 9, but these can be temperamental. Avoid them unless explicitly requested.

---

## Core Code Quality Principles

These are guiding philosophy, not rigid rules — apply them as a lens when designing solutions:

1. **Never do anything twice.** Every method/algorithm has a single point of truth; improvements propagate automatically.
2. **Design for generalization by default.** Assume you'll need to do something an infinite number of times. Abstraction ladder: special-case → parameterized → fully generalized. Performance optimization is implied but deprioritized unless necessary.

---

## Naming & File Conventions

- Public members: `PascalCase`. Private fields: `_camelCase` (Rider/.NET Runtime + Unity conventions).
- File names use dot notation matching namespace: e.g. `Helpers.Timer.cs` — even when folder structure already mirrors namespacing, to aid Unity Editor search.

---

## Architecture & Design Patterns

- Back configs with interfaces (e.g. `ITetrisConfig`, `IDemonConfig`) so both ScriptableObjects and runtime structs can satisfy them. Justify when a concrete type is sufficient.
- Runtime state external systems should read but not write: `{ get; private set; }`. Justify when public field access is intentional (e.g. Inspector serialization).
- Singleton `Awake` pattern: null-check `Instance`, `Destroy(gameObject)` if duplicate, else assign `Instance` and call `DontDestroyOnLoad` where appropriate.
- Unity lifecycle methods (`Awake`, `Start`, `OnEnable`, `OnDisable`, etc.) are `private` or `protected` by default. Justify when `public` access is required.
- Any class/function not directly tied to Terrormino belongs in the `Helpers` namespace (e.g. `Helpers.Timer`, `Helpers.RandomBag<T>`).

---

## Cross-System Communication

| Scenario | Pattern |
|---|---|
| Same GameObject or unique descendant (e.g. Monster > Model) | `UnityEvent` / `UnityEvent<T>` |
| Non-unique-descendant or unrelated GameObjects (e.g. Map > Hex, multiple instances) | ScriptableObject Event Channel |

Justify tighter coupling when intentional.

---

## Refactoring Protocol

- When changing a method's signature (parameters, return type, name), grep for all call sites across the codebase **before** making the change, and update every call site in the same response.
- When adding, removing, or renaming a member on an interface, grep for all implementors **before** changing the interface and update every one in the same response. Two-pass search:
  1. Grep for `: InterfaceName` scoped to the interface's own namespace directory.
  2. Grep the full codebase for the fully qualified name (e.g. `My.Namespace.IFoo`).
- Never report work as done until all call sites/implementors are confirmed updated.

---

## Error Handling — "Log Loud, Fail Soft"

- Don't crash unless it genuinely warrants it. Prefer surfacing errors via the Unity Console.
- `Debug.LogWarning` — uninitialized values, missing Inspector refs, `Awake`/`Start` setup issues, simple dev/scene mistakes.
- `Debug.LogError` — all other error types.

---

## AI-Generated Code Attribution

Any class or method where Claude is responsible for the final form of the code must be tagged with `[AiGenerated("Claude", "<model>")]` using `Helpers.AiGeneratedAttribute` (`AllowMultiple = true`), substituting the actual model name (e.g. `"Sonnet 4.6"`).

- Tag at the **class** level if the whole class is generated.
- Tag at the **method** level if only specific methods are added/changed.
- Excludes files copied or moved verbatim without modification.

---

## Unity .meta Files

**Rule:** Never create, rename, or generate `.meta` files yourself (including inventing GUIDs).

**Why:** Unity's asset database auto-generates `.meta` files (with correct GUIDs) on the next editor refresh. Hand-authoring them risks GUID mismatches that break asset references.

**Exception:** When *renaming* a file that already has a tracked `.meta`, move the existing `.meta` alongside the file (e.g. via `git mv`) to preserve its GUID and keep `m_Script` references intact. The rule is about not *inventing* new `.meta` content.

**How to apply:** When writing a new `.cs` or other asset file, just write the file and stop. Do not also write a matching `.meta`.
