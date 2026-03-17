using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.ClassSystem
{
    /// <summary>
    /// Central registry for all playable classes.
    ///
    /// Usage
    /// =====
    /// 1.  Each concrete class calls <c>ClassRegistry.Register(new MyClass())</c>
    ///     inside its own static <c>Initialize()</c> method (ServUO startup hook).
    ///
    /// 2.  <c>ClassRegistry.Initialize()</c> is called by the server after all
    ///     scripts are loaded – it iterates every registered class and calls
    ///     <c>RegisterCommands()</c> on each one.
    ///
    /// 3.  Use <c>ClassRegistry.Get(ClassType)</c> to obtain the singleton instance
    ///     for a given class, e.g. when applying stats to a player.
    /// </summary>
    public static class ClassRegistry
    {
        // ── Internal storage ────────────────────────────────────────────────────

        private static readonly Dictionary<ClassType, IPlayerClass> _classes
            = new Dictionary<ClassType, IPlayerClass>();

        // ── Registration ────────────────────────────────────────────────────────

        /// <summary>
        /// Registers a class instance.  Called from each class's Initialize().
        /// Throws if the same ClassType is registered twice (catches copy-paste errors).
        /// </summary>
        public static void Register(IPlayerClass playerClass)
        {
            if (playerClass == null)
                throw new ArgumentNullException("playerClass");

            if (_classes.ContainsKey(playerClass.ClassType))
                throw new InvalidOperationException(
                    $"[ClassRegistry] ClassType '{playerClass.ClassType}' is already registered.");

            _classes[playerClass.ClassType] = playerClass;

            Console.WriteLine($"[ClassRegistry] Registered class: {playerClass.DisplayName} ({playerClass.ClassType})");
        }

        // ── Lookup ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the class instance for the given type, or null if not found.
        /// </summary>
        public static IPlayerClass Get(ClassType type)
        {
            IPlayerClass result;
            _classes.TryGetValue(type, out result);
            return result;
        }

        /// <summary>All currently registered classes (read-only view).</summary>
        public static IEnumerable<IPlayerClass> All => _classes.Values;

        // ── Startup hook ────────────────────────────────────────────────────────

        /// <summary>
        /// Called once by the server after all scripts have been initialized.
        /// Iterates every registered class and calls RegisterCommands().
        /// </summary>
        public static void Configure()
        {
            Console.WriteLine($"[ClassRegistry] Initializing {_classes.Count} class(es)...");

            foreach (var kvp in _classes)
            {
                try
                {
                    kvp.Value.RegisterCommands();
                    Console.WriteLine($"[ClassRegistry]   Commands registered for: {kvp.Value.DisplayName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ClassRegistry] ERROR registering commands for {kvp.Value.DisplayName}: {ex}");
                }
            }
        }

        // ── Helper: apply a class's base stats to a player ────────────────────

        /// <summary>
        /// Applies (or re-applies) the stat and skill caps of a class to a player.
        /// Call this whenever a class is assigned or when stats need a refresh.
        /// </summary>
        public static void ApplyClassStats(PlayerMobile player, IPlayerClass cls)
        {
            if (player == null || cls == null)
                return;

            // --- Stats ---
            player.RawStr = cls.BaseStr;
            player.RawDex = cls.BaseDex;
            player.RawInt = cls.BaseInt;

            // --- Skill caps (ServUO uses Skills[SkillName].Cap) ---
            // We set the cap but do NOT set the current value here; the shard's
            // other systems (trainers, skill gain) handle current values.
            SetSkillCap(player, SkillName.Swords,       cls.SkillSwords);
            SetSkillCap(player, SkillName.Macing,       cls.SkillMacing);
            SetSkillCap(player, SkillName.Fencing,      cls.SkillFencing);
            SetSkillCap(player, SkillName.Archery,      cls.SkillArchery);
            SetSkillCap(player, SkillName.Wrestling,    cls.SkillWrestling);
            SetSkillCap(player, SkillName.Tactics,      cls.SkillTactics);
            SetSkillCap(player, SkillName.Parry,        cls.SkillParrying);
            SetSkillCap(player, SkillName.Anatomy,      cls.SkillAnatomy);
            SetSkillCap(player, SkillName.Hiding,       cls.SkillHiding);
            SetSkillCap(player, SkillName.Stealth,      cls.SkillStealth);
            SetSkillCap(player, SkillName.DetectHidden, cls.SkillDetectHidden);
            SetSkillCap(player, SkillName.Healing,      cls.SkillHealing);
            SetSkillCap(player, SkillName.Meditation,   cls.SkillMeditation);
            SetSkillCap(player, SkillName.MagicResist,  cls.SkillMagicResist);
            SetSkillCap(player, SkillName.EvalInt,      cls.SkillEvalInt);
            SetSkillCap(player, SkillName.Magery,       cls.SkillMagery);
        }

        // Sets the cap for a skill if the value is > 0; sets it to 0 otherwise to
        // effectively disable the skill for this class.
        private static void SetSkillCap(PlayerMobile player, SkillName skill, double cap)
        {
            // ServUO stores skill caps as tenths (e.g. 1000 = 100.0).
            // Skill.Cap is a double in ServUO, so we can assign directly.
            player.Skills[skill].Cap = cap;
        }
    }
}
