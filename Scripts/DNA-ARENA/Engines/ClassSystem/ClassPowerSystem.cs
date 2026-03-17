using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.ClassSystem
{
    /// <summary>
    /// Static service that manages per-player cooldowns and active effect tracking
    /// for all class powers.
    ///
    /// This replaces the RunUO CastSystem's delay / curse-or-bless tables.
    ///
    /// Cooldown model
    /// ==============
    /// • <b>Global cooldown</b>  – a shared timer across all powers for one player.
    ///   Prevents ability spam between different powers.
    /// • <b>Individual cooldown</b> – a per-power timer that prevents re-using the
    ///   same power too quickly.
    ///
    /// Active effect tracking
    /// ======================
    /// Powers that have a timed effect (e.g. buffs, debuffs) register themselves
    /// here so they can be looked up and cancelled if a counter-effect is applied.
    /// Call <see cref="GetActiveEffect{T}"/> to check whether a specific power type
    /// is currently active on a mobile.
    /// </summary>
    public static class ClassPowerSystem
    {
        // ── Internal tables ─────────────────────────────────────────────────────

        // global cooldown: mobile → expiry time
        private static readonly Dictionary<Mobile, DateTime> _globalCooldowns
            = new Dictionary<Mobile, DateTime>();

        // individual cooldowns: mobile → (power type → expiry time)
        private static readonly Dictionary<Mobile, Dictionary<Type, DateTime>> _individualCooldowns
            = new Dictionary<Mobile, Dictionary<Type, DateTime>>();

        // active effects: mobile → list of active BasePower instances
        private static readonly Dictionary<Mobile, List<BasePower>> _activeEffects
            = new Dictionary<Mobile, List<BasePower>>();

        // ── Global cooldown ──────────────────────────────────────────────────────

        /// <summary>Returns true if the mobile is still on global cooldown.</summary>
        public static bool IsOnGlobalCooldown(Mobile m)
        {
            DateTime expiry;
            return _globalCooldowns.TryGetValue(m, out expiry)
                   && DateTime.UtcNow < expiry;
        }

        /// <summary>Sets the global cooldown for a mobile.</summary>
        public static void SetGlobalCooldown(Mobile m, int seconds)
        {
            _globalCooldowns[m] = DateTime.UtcNow + TimeSpan.FromSeconds(seconds);
        }

        // ── Individual cooldown ──────────────────────────────────────────────────

        /// <summary>Returns true if the specific power type is still on cooldown for the mobile.</summary>
        public static bool IsOnIndividualCooldown(Mobile m, Type powerType)
        {
            Dictionary<Type, DateTime> table;
            if (!_individualCooldowns.TryGetValue(m, out table))
                return false;

            DateTime expiry;
            return table.TryGetValue(powerType, out expiry)
                   && DateTime.UtcNow < expiry;
        }

        /// <summary>
        /// Returns the remaining individual cooldown in whole seconds, or 0 if not on cooldown.
        /// </summary>
        public static int GetIndividualCooldownRemaining(Mobile m, Type powerType)
        {
            Dictionary<Type, DateTime> table;
            if (!_individualCooldowns.TryGetValue(m, out table))
                return 0;

            DateTime expiry;
            if (!table.TryGetValue(powerType, out expiry))
                return 0;

            double remaining = (expiry - DateTime.UtcNow).TotalSeconds;
            return remaining > 0 ? (int)Math.Ceiling(remaining) : 0;
        }

        /// <summary>
        /// Returns the remaining global cooldown in whole seconds, or 0 if not on cooldown.
        /// </summary>
        public static int GetGlobalCooldownRemaining(Mobile m)
        {
            DateTime expiry;
            if (!_globalCooldowns.TryGetValue(m, out expiry))
                return 0;

            double remaining = (expiry - DateTime.UtcNow).TotalSeconds;
            return remaining > 0 ? (int)Math.Ceiling(remaining) : 0;
        }

        /// <summary>Sets the individual cooldown for a power type on a mobile.</summary>
        public static void SetIndividualCooldown(Mobile m, Type powerType, int seconds)
        {
            Dictionary<Type, DateTime> table;
            if (!_individualCooldowns.TryGetValue(m, out table))
            {
                table = new Dictionary<Type, DateTime>();
                _individualCooldowns[m] = table;
            }

            table[powerType] = DateTime.UtcNow + TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Clears all individual cooldowns for a mobile.
        /// Useful for GM commands or special items.
        /// </summary>
        public static void ClearAllCooldowns(Mobile m)
        {
            _globalCooldowns.Remove(m);

            Dictionary<Type, DateTime> table;
            if (_individualCooldowns.TryGetValue(m, out table))
                table.Clear();
        }


        // ── Duration formatting ────────────────────────────────────────────────

        /// <summary>
        /// Formats a duration in seconds into a human-readable string.
        ///   less than 60s    - "X second(s)"
        ///   less than 3600s  - "X minute(s) Y second(s)"
        ///   3600s or more    - "X hour(s) Y minute(s)"
        /// </summary>
        public static string FormatDuration(int totalSeconds)
        {
            if (totalSeconds <= 0)
                return "0 seconds";

            if (totalSeconds < 60)
            {
                string unit = totalSeconds == 1 ? "second" : "seconds";
                return totalSeconds + " " + unit;
            }
            else if (totalSeconds < 3600)
            {
                int minutes = totalSeconds / 60;
                int seconds = totalSeconds % 60;

                string mUnit = minutes == 1 ? "minute" : "minutes";
                string result = minutes + " " + mUnit;

                if (seconds > 0)
                {
                    string sUnit = seconds == 1 ? "second" : "seconds";
                    result += " " + seconds + " " + sUnit;
                }

                return result;
            }
            else
            {
                int hours   = totalSeconds / 3600;
                int minutes = (totalSeconds % 3600) / 60;

                string hUnit = hours == 1 ? "hour" : "hours";
                string result = hours + " " + hUnit;

                if (minutes > 0)
                {
                    string mUnit = minutes == 1 ? "minute" : "minutes";
                    result += " " + minutes + " " + mUnit;
                }

                return result;
            }
        }

        // ── Active effects ────────────────────────────────────────────────────────

        /// <summary>
        /// Registers an active effect instance for a target mobile.
        /// Called by <see cref="BasePower"/> when a timed effect starts.
        /// </summary>
        public static void RegisterActiveEffect(Mobile target, BasePower power)
        {
            List<BasePower> list;
            if (!_activeEffects.TryGetValue(target, out list))
            {
                list = new List<BasePower>();
                _activeEffects[target] = list;
            }

            // Prevent duplicates of the same power type.
            if (!list.Exists(p => p.GetType() == power.GetType()))
                list.Add(power);
        }

        /// <summary>
        /// Removes an active effect instance when it expires or is cancelled.
        /// Called by <see cref="BasePower"/>'s effect timer.
        /// </summary>
        public static void UnregisterActiveEffect(Mobile target, BasePower power)
        {
            List<BasePower> list;
            if (!_activeEffects.TryGetValue(target, out list))
                return;

            list.RemoveAll(p => p.GetType() == power.GetType());
        }

        /// <summary>
        /// Returns the active effect of type <typeparamref name="T"/> on the target,
        /// or null if not present.
        ///
        /// Usage example:
        /// <code>
        ///   var existing = ClassPowerSystem.GetActiveEffect&lt;HeatMetalPower&gt;(target);
        ///   if (existing != null) existing.Cancel();
        /// </code>
        /// </summary>
        public static T GetActiveEffect<T>(Mobile target) where T : BasePower
        {
            List<BasePower> list;
            if (!_activeEffects.TryGetValue(target, out list))
                return null;

            foreach (var p in list)
            {
                if (p is T match)
                    return match;
            }

            return null;
        }

        /// <summary>
        /// Returns true if the target has an active effect of the given type.
        /// </summary>
        public static bool HasActiveEffect(Mobile target, Type powerType)
        {
            List<BasePower> list;
            if (!_activeEffects.TryGetValue(target, out list))
                return false;

            return list.Exists(p => p.GetType() == powerType);
        }

        /// <summary>
        /// Immediately expires and unregisters all active effects on a mobile.
        /// Call this on player death or class removal.
        /// </summary>
        public static void ExpireAllEffects(Mobile target)
        {
            List<BasePower> list;
            if (!_activeEffects.TryGetValue(target, out list))
                return;

            // Iterate a copy because OnExpire may modify the list.
            var copy = new List<BasePower>(list);

            foreach (var power in copy)
            {
                try
                {
                    power.OnExpire(target);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ClassPowerSystem] Error expiring {power.GetType().Name} on {target}: {ex.Message}");
                }
                finally
                {
                    UnregisterActiveEffect(target, power);
                }
            }
        }

        // ── Main cast entry point ────────────────────────────────────────────────

        /// <summary>
        /// Validates that the caster has the correct class, then begins the power.
        /// This is the single public entry point called from class command handlers.
        ///
        /// <paramref name="requiredClass"/> is the <see cref="ClassType"/> the caster
        /// must have; pass <see cref="ClassType.None"/> to skip the class check
        /// (e.g. for GM testing).
        /// </summary>
        public static void Cast(BasePower power, ClassType requiredClass = ClassType.None)
        {
            if (power == null)
                throw new ArgumentNullException("power");

            Mobile caster = power.Caster;

            // Class check (skipped for GMs or if requiredClass == None).
            if (requiredClass != ClassType.None
                && caster is PlayerMobile pm
                && pm.AccessLevel < AccessLevel.GameMaster)
            {
                if (!pm.ClassComponent.HasClass(requiredClass))
                {
                    caster.SendMessage("You are not the right class to use this power.");
                    return;
                }
            }

            // Delegate all further validation to BasePower.
            power.Begin();
        }
    }
}
