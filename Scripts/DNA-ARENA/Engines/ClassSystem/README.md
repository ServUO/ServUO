# Class System – Architecture Guide

## Overview

This is a clean, modular class system for a ServUO shard **without levels**.
Every player that belongs to a class starts at full power.

---

## File Layout

```
ClassSystem/
├── ClassType.cs            ← enum: one entry per class (Avenger, Ninja, …)
├── IPlayerClass.cs         ← interface contract for every class
├── BasePlayerClass.cs      ← abstract base with default (no-op / zero) implementations
├── ClassRegistry.cs        ← singleton registry; bootstraps commands at startup
├── ClassComponent.cs       ← serializable per-player state (current class)
├── BasePower.cs            ← base for every class power (replaces RunUO Cast/CastPreghiera)
├── ClassPowerSystem.cs     ← cooldown tables + active effect registry (replaces CastSystem)
└── Classes/
    ├── AvengerClass.cs     ← Avenger class definition + command dispatch
    └── AvengerPowers.cs    ← all 18 Avenger power implementations
```

---

## Adding a New Class (e.g. Ninja)

1. Add `Ninja = 2` to `ClassType.cs`.
2. Create `Classes/NinjaClass.cs` extending `BasePlayerClass`:
   - Override `ClassType`, `DisplayName`, stats, skills, armor list.
   - Override `RegisterCommands()` to hook `.ninja <n>`.
   - Build a `PowerTable` mapping index → `BasePower` factory.
3. Create `Classes/NinjaPowers.cs` with your power classes extending `BasePower`.
4. Add `ClassRegistry.Register(new NinjaClass())` in a static `Initialize()` call.

You never need to touch the core files.

---

## Adding a New Power to an Existing Class

1. Add a new class in `AvengerPowers.cs` extending `BasePower`.
2. Add an entry to `AvengerClass.PowerTable`.
3. That's it.

---

## PlayerMobile Integration

Add these members to `PlayerMobile`:

```csharp
// --- Field ---
private ClassComponent m_ClassComponent;

// --- Property ---
public ClassComponent ClassComponent
{
    get { return m_ClassComponent ?? (m_ClassComponent = new ClassComponent(this)); }
}

// --- In constructor ---
m_ClassComponent = new ClassComponent(this);

// --- In Serialize() ---
m_ClassComponent.Serialize(writer);

// --- In Deserialize() ---
m_ClassComponent = new ClassComponent(this);
m_ClassComponent.Deserialize(reader);
```

Checking the player's class anywhere in the shard:

```csharp
if (player.ClassComponent.HasClass(ClassType.Avenger)) { … }
IPlayerClass cls = player.ClassComponent.PlayerClass; // null if None
```

Assigning a class (e.g. from a GM command or quest):

```csharp
player.ClassComponent.Assign(ClassType.Avenger);
```

---

## Startup Bootstrap

ServUO's script-loading system picks up any `public static void Initialize()` method.
Create one file per class, or a single bootstrap file:

```csharp
public static class ClassSystemBootstrap
{
    public static void Initialize()
    {
        // Register all classes.
        AvengerClass.Initialize();
        // NinjaClass.Initialize();

        // After all classes are registered, hook up their commands.
        ClassRegistry.Initialize();
    }
}
```

---

## Power Authoring Quick Reference

```csharp
public sealed class MyPower : BasePower
{
    // --- What the caster says overhead ---
    public override string    CastPhrase          => "An Corp";

    // --- Cost ---
    public override int       ManaCost            => 10;

    // --- Targeting ---
    public override bool      RequiresTarget      => true;   // false = self / area
    public override int       MaxRange            => 6;
    public override bool      IsAreaEffect        => false;  // true = hits all in MaxRange

    // --- Timing ---
    public override double    CastTime            => 0.0;    // seconds to cast; 0 = instant
    public override int       GlobalCooldown      => 5;      // shared cooldown after use
    public override int       IndividualCooldown  => 30;     // this-power-only cooldown
    public override int       EffectDuration      => 60;     // 0 = fire-and-forget

    // --- Category (governs friendly-fire logic) ---
    public override PowerType PowerType           => PowerType.Harmful;

    public MyPower(Mobile caster) : base(caster) { }

    // Called before anything is consumed – return false to abort.
    public override bool OnBeforeCast() => true;

    // Called after mana is consumed, before target is accepted.
    public override bool OnBeforeCastTarget(Mobile target) => true;

    // Main effect for self / no-target powers.
    public override void OnCast() { … }

    // Main effect for targeted / area powers.
    public override void OnCastTarget(Mobile target) { … }

    // Called when EffectDuration expires.
    public override void OnExpire(Mobile target) { … }
}
```

---

## Cooldown Model

| Type | Scope | Description |
|------|-------|-------------|
| Global | Per player, shared | Set via `GlobalCooldown` property. Prevents using ANY power too quickly. |
| Individual | Per player, per power type | Set via `IndividualCooldown`. Limits re-use of the same power. |

Both are stored in-memory in `ClassPowerSystem` with `DateTime` expiry.
No timers are scheduled for cooldowns – they are checked lazily on each cast attempt.

---

## Porting Notes (RunUO → ServUO)

| RunUO | ServUO equivalent |
|-------|-------------------|
| `CastSystem.LanciaCast(new MySpell(player))` | `ClassPowerSystem.Cast(new MyPower(player), ClassType.Avenger)` |
| `CastSystem.RegisterCurseOrBless(m, cast)` | Automatic – happens inside `BasePower` when `EffectDuration > 0` |
| `CastSystem.CurseOrBlessLookUp(m, typeof(X))` | `ClassPowerSystem.HasActiveEffect(m, typeof(X))` |
| `DamageSystem.ApplicaDanno(caster, target, dmg, type)` | `AOS.Damage(target, caster, dmg, phys%, fire%, cold%, poison%, energy%)` |
| `Caster.Livello` | Remove – use flat constants |
| `TabellaLivelliMinimi` | Remove – no levels |
| `PlagueSystem.AmmalaMobile(…)` | `target.ApplyPoison(…)` or custom disease system |
| `CastPreghiera` | Extend `BasePower` instead |
| `ClasseBase` | Extend `BasePlayerClass` instead |
