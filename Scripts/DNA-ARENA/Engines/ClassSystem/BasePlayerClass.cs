using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.ClassSystem
{
    /// <summary>
    /// Abstract base class for all player classes.
    ///
    /// Provides sensible defaults (zero / empty) for every IPlayerClass member so
    /// that concrete classes only need to override what they actually use.
    /// </summary>
    public abstract class BasePlayerClass : IPlayerClass
    {
        // ── Identity ────────────────────────────────────────────────────────────

        public abstract ClassType ClassType  { get; }
        public abstract string    DisplayName { get; }

        // ── Base Stats ──────────────────────────────────────────────────────────

        public virtual int BaseStr { get; } = 0;
        public virtual int BaseDex { get; } = 0;
        public virtual int BaseInt { get; } = 0;

        // ── Skills (all default to 0 – override only what the class uses) ───────

        public virtual double SkillSwords       { get; } = 0.0;
        public virtual double SkillMacing       { get; } = 0.0;
        public virtual double SkillFencing      { get; } = 0.0;
        public virtual double SkillArchery      { get; } = 0.0;
        public virtual double SkillWrestling    { get; } = 0.0;
        public virtual double SkillTactics      { get; } = 0.0;
        public virtual double SkillParrying     { get; } = 0.0;
        public virtual double SkillAnatomy      { get; } = 0.0;
        public virtual double SkillHiding       { get; } = 0.0;
        public virtual double SkillStealth      { get; } = 0.0;
        public virtual double SkillDetectHidden { get; } = 0.0;
        public virtual double SkillHealing      { get; } = 0.0;
        public virtual double SkillMeditation   { get; } = 0.0;
        public virtual double SkillMagicResist  { get; } = 0.0;
        public virtual double SkillEvalInt      { get; } = 0.0;
        public virtual double SkillMagery       { get; } = 0.0;

        // ── Equipment ───────────────────────────────────────────────────────────

        /// <summary>
        /// Override in subclasses to list the armor types the class may wear.
        /// Default: no armor allowed (empty list).
        /// </summary>
        public virtual IReadOnlyList<ArmorMaterialType> AllowedArmorMaterials { get; }
            = Array.AsReadOnly(Array.Empty<ArmorMaterialType>());

        /// <summary>
        /// Override to restrict weapon types.  Default: all weapons allowed.
        /// </summary>
        public virtual bool CanEquipWeapon(BaseWeapon weapon) => true;

        // ── Lifecycle ───────────────────────────────────────────────────────────

        /// <summary>
        /// Override to register class-specific player commands.
        /// Called once at startup by <see cref="ClassRegistry"/>.
        /// </summary>
        public virtual void RegisterCommands() { }

        /// <summary>Called when the class is granted to a player.</summary>
        public virtual void OnClassAssigned(PlayerMobile player) { }

        /// <summary>Called when the class is removed from a player.</summary>
        public virtual void OnClassRemoved(PlayerMobile player) { }
    }
}
