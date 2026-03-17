using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.ClassSystem
{
    /// <summary>
    /// Contract that every playable class must satisfy.
    ///
    /// Design notes
    /// ============
    /// • Stats / skills are flat values (no level scaling): the shard has no level system.
    /// • AllowedArmorMaterials and CanEquipWeapon gate what a class may wear/wield.
    /// • RegisterCommands is called once at server startup (via ClassRegistry) and must
    ///   register any player-facing commands that belong to the class.
    /// • OnClassAssigned / OnClassRemoved let a class react when it is given to or
    ///   stripped from a player.
    /// </summary>
    public interface IPlayerClass
    {
        // ── Identity ────────────────────────────────────────────────────────────

        /// <summary>Unique enum value that identifies this class.</summary>
        ClassType ClassType { get; }

        /// <summary>Display name shown to players (e.g. in gumps or system messages).</summary>
        string DisplayName { get; }

        // ── Base Stats ──────────────────────────────────────────────────────────

        int BaseStr { get; }
        int BaseDex { get; }
        int BaseInt { get; }

        // ── Skill Caps ──────────────────────────────────────────────────────────
        // Return 0.0 if the class cannot use that skill.

        double SkillSwords     { get; }
        double SkillMacing     { get; }
        double SkillFencing    { get; }
        double SkillArchery    { get; }
        double SkillWrestling  { get; }
        double SkillTactics    { get; }
        double SkillParrying   { get; }
        double SkillAnatomy    { get; }
        double SkillHiding     { get; }
        double SkillStealth    { get; }
        double SkillDetectHidden { get; }
        double SkillHealing    { get; }
        double SkillMeditation { get; }
        double SkillMagicResist { get; }
        double SkillEvalInt    { get; }
        double SkillMagery     { get; }

        // ── Equipment Restrictions ──────────────────────────────────────────────

        /// <summary>Armor material types this class is allowed to wear.</summary>
        IReadOnlyList<ArmorMaterialType> AllowedArmorMaterials { get; }

        /// <summary>
        /// Returns true if the class may equip the given weapon.
        /// Called by the equip hook in PlayerMobile.
        /// </summary>
        bool CanEquipWeapon(BaseWeapon weapon);

        // ── Lifecycle ───────────────────────────────────────────────────────────

        /// <summary>
        /// Register player-facing commands for this class.
        /// Called once at startup by <see cref="ClassRegistry"/>.
        /// </summary>
        void RegisterCommands();

        /// <summary>Called when the class is assigned to a player for the first time.</summary>
        void OnClassAssigned(PlayerMobile player);

        /// <summary>Called when the class is removed from a player (e.g. class change).</summary>
        void OnClassRemoved(PlayerMobile player);
    }
}
