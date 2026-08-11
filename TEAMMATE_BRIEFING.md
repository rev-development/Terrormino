# Terrormino — Teammate Briefing

This document is intended to be pasted as a system prompt into Claude Desktop (not Claude Code).
It covers two things:
1. The coding standards and behaviors to adopt for this project.
2. A plain-English walkthrough of the `dataflow-rework` branch and why it exists.

---

## Part 1 — How to Work on This Project

These are the rules and patterns the lead developer has established. Treat them as ground truth.

### Naming
- Public members and class names: `PascalCase`
- Private fields: `_camelCase`
- File names use dot-notation matching their namespace, e.g. `EC.Demon.Manager.cs` — even if the folder already mirrors the namespace. This makes Unity Editor search reliable.

### Unity Lifecycle Methods
`Awake`, `Start`, `Update`, `OnEnable`, `OnDisable`, etc. are `private` or `protected` by default. Only make them `public` if there is a deliberate reason.

### Singleton Pattern
```csharp
public void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }
    Instance = this;
    DontDestroyOnLoad(gameObject); // only where appropriate
}
```

### Config Pattern (Interface + ScriptableObject + Data Struct)
Every system's configuration is defined three ways:
- `IConfig` — an interface listing all config properties.
- `ConfigSO` — a ScriptableObject that implements `IConfig`, serialized in the Unity Inspector.
- `ConfigData` — a plain C# `[Serializable]` class that also implements `IConfig`, used when you need to set config in code (e.g. for testing, or when you don't have an asset file).

This means any system that accepts an `IConfig` can be driven from either the Inspector *or* code — no rewriting required.

### Cross-System Communication
| Situation | Pattern to use |
|---|---|
| Two components on the **same** GameObject (or a unique child) | `UnityEvent` / `UnityEvent<T>` |
| Unrelated GameObjects, or many instances of the same type | ScriptableObject Event Channel (`VoidEC`, `GameObjectEC`, etc.) |

Event Channels are ScriptableObject assets. Any script can subscribe to them — no direct reference between senders and receivers needed. This is how the project avoids spaghetti `GetComponent` chains.

### Error Handling — "Log Loud, Fail Soft"
Don't throw exceptions or crash unless something is genuinely unrecoverable.
- `Debug.LogWarning` — missing Inspector references, uninitialized values, simple setup mistakes.
- `Debug.LogError` — everything else that's wrong.

### Code Quality Principles (guiding philosophy, not rigid rules)
1. **Never do anything twice.** If you write the same logic in two places, one of them should call the other.
2. **Design for generalization.** Write things so they'd work for more than one case, even if you only need one case right now.

### AI Attribution
Any class or method where Claude wrote the final code must be tagged:
```csharp
[AiGenerated("Claude", "Sonnet 4.6")]
```
Tag the class if Claude wrote the whole thing. Tag individual methods if only those methods were generated.

### `.meta` Files
Never write or generate `.meta` files by hand. Unity creates them automatically on the next editor refresh. If you rename a file, use `git mv` so the existing `.meta` moves with it and keeps its GUID intact.

### Helpers Namespace
Any utility class not specifically tied to Terrormino goes in the `Helpers` namespace (e.g. `Helpers.Timer`, `Helpers.RandomBag<T>`). This keeps generic tools reusable.

### Refactoring Protocol
Before changing a method's signature or an interface's members, search the whole codebase for every place it's used and update all of them in the same change. Never leave call sites out of sync.

---

## Part 2 — What Changed in `dataflow-rework` vs `main`

### The Big Picture

`main` had several systems (the Demon AI, Tetris, the game loop) written as relatively "flat" MonoBehaviours — one big class doing many things, config values as plain public fields, systems calling each other directly by reference.

`dataflow-rework` is a near-complete architectural rework. The goals were:
- Make every major system's config injectable from both the Inspector and from code.
- Give the Demon a proper finite state machine so its AI states are isolated, swappable, and testable.
- Replace direct inter-system calls with Event Channels so systems don't need to know about each other.
- Extract reusable infrastructure into the `Helpers` namespace so it can be used by any future system.

---

### Change 1 — The EC (Entity-Component) Namespace

In `main`, the Demon was one namespace (`Demon`) with a handful of classes.

In `dataflow-rework`, everything was reorganized under the `EC` (Entity-Component) namespace. Each major game object (Demon, Tetris) now has a folder structure like:
```
EC/
  Demon/
    EC.Demon.Manager.cs        ← spawning, lifecycle
    EC.Demon.EventBus.cs       ← all events this demon raises/receives
    EC.Demon.Health.cs         ← health tracking
    EC.Demon.Jumpscare.cs      ← jumpscare behavior
    EC.Demon.Manager.IConfig.cs
    EC.Demon.Manager.ConfigSO.cs
    EC.Demon.Manager.ConfigData.cs
    Pathing/
      EC.Demon.Pathing.Controller.cs
      Patrol/
        EC.Demon.Pathing.Patrol.State.cs
        ...
      Chase/
        EC.Demon.Pathing.Chase.State.cs
        ...
```

Each component has a single responsibility. The `EventBus` component acts as the demon's internal communication hub — other components on the same GameObject talk through it using `UnityEvent`, while the EventBus itself talks to the wider world using ScriptableObject Event Channels.

---

### Change 2 — The Config Triad (IConfig / ConfigSO / ConfigData)

**Before (`main`):**
```csharp
public class Manager : MonoBehaviour
{
    public float GracePeriod = 30f;
    public float SpawnInterval = 15f;
    // ... set in Inspector only
}
```

**After (`dataflow-rework`):**
```csharp
// The contract
public interface IConfig
{
    float SpawnGracePeriod { get; set; }
    float SpawnInterval { get; set; }
    // ...
}

// Inspector version (a ScriptableObject asset file)
public class ConfigSO : ScriptableObject, IConfig { ... }

// Code version (a plain serializable struct)
public class ConfigData : IConfig { ... }
```

**Why?** Configs backed by an interface can be swapped between the live SO (used in the real game) and a `ConfigData` struct (used in tests or procedural generation). You write the system once, and it doesn't care which one it gets.

---

### Change 3 — The Finite State Machine (FSMState)

The Demon's movement AI in `main` was handled with flags and `if/else` chains inside a single `Update()` loop.

In `dataflow-rework`, `Helpers.FSMState<TStateType, TStateConfig, TController>` provides a reusable abstract base for any state machine:

```csharp
// What this means in plain English:
// TStateType  = an enum of the possible states (e.g. Patrol, Chase)
// TStateConfig = the config object for this specific state
// TController = the MonoBehaviour that owns this state machine
public abstract class FSMState<TStateType, TStateConfig, TController>
    where TStateType : Enum
    where TController : MonoBehaviour
{
    public abstract void Start();
    public abstract void Update();
    public abstract void Exit();
}
```

The `< >` angle brackets are *generics* — think of them as blanks to fill in later. `FSMState` is like a form with three blanks: "what are the states?", "what config does this state need?", "who runs this state machine?" Each concrete state (e.g. `Patrol.State`, `Chase.State`) fills in those blanks for its specific context.

The Controller holds the current state and delegates `Update()` to it:
```csharp
// In Pathing.Controller.Update():
CurrentState?.Update();
```

When a transition happens (e.g. demon sees player → switch to Chase), the old state's `Exit()` is called, a new state is created and its `Start()` is called. States are isolated — Patrol doesn't know Chase exists.

---

### Change 4 — Event Bus per Demon

**Before:** The demon's components called each other's public methods directly.

**After:** Every demon prefab has an `EventBus` component. It exposes `UnityEvent`s for things like:
- `BanishTriggered` — something wants this demon removed
- `JumpscareTriggered` — something wants to trigger a jumpscare
- `Illuminated` — the flashlight hit or left this demon

Other components on the same demon subscribe to these events in `OnEnable` and unsubscribe in `OnDisable`. The EventBus itself bridges outward to ScriptableObject Event Channels (like `GameObjectEC _removeDemon`) so the wider game systems can react without holding a reference to any specific demon instance.

---

### Change 5 — ScriptableObject Event Channels

The project uses a custom `GenericEC<T>` base class:
```csharp
public abstract class GenericEC<T> : ScriptableObject
{
    public UnityAction<T> OnEventRaised;
    public void RaiseEvent(T parameter) => OnEventRaised?.Invoke(parameter);
}
```

Concrete channels are just one-liners:
```csharp
public class GameObjectEC : GenericEC<GameObject> { }
public class VoidEC : GenericEC<Unit> { }  // Unit = "no data"
```

You create a `GameObjectEC` asset in the Unity Editor, drag it into any sender's Inspector slot and any receiver's Inspector slot. They communicate without ever having a C# reference to each other. This is especially important for systems that have multiple instances (like multiple demons) — you don't need to wire up every single one.

---

### What Hasn't Changed

- The core Tetris gameplay logic is largely the same, just reorganized into the `EC.Tetris` namespace with the same config and event bus patterns applied.
- The `Helpers` utilities (`Timer`, `RandomBag`, `SingletonMonoBehaviour`, extension methods) were mostly already there and have been cleaned up/expanded.
- The game loop (`GameLoop.NightManager`, etc.) still drives night progression the same way — it just now communicates via Event Channels instead of direct method calls.

---

### Things to Watch Out For

- If you add a new component to the Demon prefab, ask yourself: does it need to know about other components, or can it just subscribe to the `EventBus`? Prefer the EventBus.
- Config changes go in the `IConfig` interface first — then both `ConfigSO` and `ConfigData` need to be updated to match.
- State transitions happen in the `Pathing.Controller` — if you add a new state, register it in the `States` dictionary there.
- `[DisableInEditor]` on a serialized field means "this is wired up at runtime, not in the Inspector" — don't panic if it shows as empty in the Editor.
