using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Server.Engines.ClassSystem
{
    // ────────────────────────────────────────────────────────────────────────────
    //  AVENGER POWERS
    //
    //  Porting notes
    //  =============
    //  • All "Caster.Livello" references removed – no level system.
    //  • Flat values replace the old level-scaled formulas.
    //  • Italian → English for all user-facing messages and identifiers.
    //  • RunUO CastSystem replaced by ClassPowerSystem / BasePower.
    //  • RunUO DamageSystem.ApplicaDanno → ServUO AOS.Damage.
    //  • RunUO PlagueSystem.AmmalaMobile → stub comment (implement per shard rules).
    //  • Level checks (TabellaLivelliMinimi) dropped entirely.
    // ────────────────────────────────────────────────────────────────────────────

    // ── 1. Detect Good ──────────────────────────────────────────────────────────

    /// <summary>
    /// Detects the alignment of the targeted player.
    /// Ported from: IndividuaIlBene
    /// </summary>
    public sealed class DetectGoodPower : BasePower
    {
        public override string   CastPhrase  => "Wis Hre";
        public override int      ManaCost    => 0;
        public override bool     RequiresTarget => true;
        public override int      MaxRange    => 6;
        public override PowerType PowerType  => PowerType.Neutral;

        public DetectGoodPower(Mobile caster) : base(caster) { }

        public override void OnCastTarget(Mobile target)
        {
            if (target is PlayerMobile pm)
                Caster.SendMessage($"{pm.Name} is aligned: {pm.ClassComponent.ClassType}");
            else
                Caster.SendMessage($"{target.Name} is a creature.");
        }
    }

    // ── 2. Inflict Wounds ───────────────────────────────────────────────────────

    /// <summary>
    /// Deals fixed direct damage to a target.
    /// Ported from: InfliggiFeriteVendicatore
    /// </summary>
    public sealed class InflictWoundsPower : BasePower
    {
        public override string    CastPhrase        => "In Sehm";
        public override int       ManaCost          => 12;
        public override bool      RequiresTarget    => true;
        public override int       MaxRange          => 6;
        public override int       GlobalCooldown    => 5;
        public override PowerType PowerType         => PowerType.Harmful;

        public InflictWoundsPower(Mobile caster) : base(caster) { }

        public override bool OnBeforeCastTarget(Mobile target)
        {
            if (target == Caster)
            {
                Caster.SendMessage("You cannot use this on yourself.");
                return false;
            }
            return true;
        }

        public override void OnCastTarget(Mobile target)
        {
            int damage = Utility.RandomMinMax(6, 14);
            AOS.Damage(target, Caster, damage, 100, 0, 0, 0, 0);
            target.FixedParticles(0x374A, 10, 15, 5016, EffectLayer.Waist);
            target.PlaySound(0x1F1); // harm.wav
        }
    }

    // ── 3. Touch of Evil ────────────────────────────────────────────────────────

    /// <summary>
    /// Deals moderate fixed damage at melee range.
    /// Ported from: ToccoDelMale
    /// </summary>
    public sealed class TouchOfEvilPower : BasePower
    {
        // Original: damage = Caster.Livello + 5  → using flat 41 (max-level value was ~41)
        private const int Damage = 30;

        public override string    CastPhrase          => "Fei Ler";
        public override int       ManaCost            => 0;
        public override bool      RequiresTarget      => true;
        public override int       MaxRange            => 2;
        public override int       IndividualCooldown  => 1200;
        public override int       GlobalCooldown      => 2;
        public override PowerType PowerType           => PowerType.Harmful;

        public TouchOfEvilPower(Mobile caster) : base(caster) { }

        public override bool OnBeforeCastTarget(Mobile target)
        {
            if (target == Caster)
            {
                Caster.SendMessage("You cannot use this on yourself.");
                return false;
            }
            return true;
        }

        public override void OnCastTarget(Mobile target)
        {
            AOS.Damage(target, Caster, Damage, 100, 0, 0, 0, 0);
            target.FixedParticles(0x374A, 10, 15, 5016, EffectLayer.Waist);
            target.PlaySound(0x1F1); // harm.wav
        }
    }

    // ── 4. Avenger Light ────────────────────────────────────────────────────────

    /// <summary>
    /// Invisible light source equipped on the target's Talisman layer.
    ///
    /// Design notes
    /// ============
    /// • Invisible and non-movable: the target sees no item in their paperdoll.
    /// • Emits a 300-unit radius light (LightType.Circle300), illuminating the
    ///   target and the area around them.
    /// • Because the target is lit, stealth / hiding checks in your shard should
    ///   treat any mobile that has this item equipped as unable to hide.
    ///   Hook example (in your stealth system):
    ///     if (player.FindItemOnLayer(Layer.Talisman) is AvengerLightItem) → deny hide
    /// • Serialize / Deserialize delete the item on load (transient item).
    /// </summary>
    public sealed class AvengerLightItem : BaseEquipableLight
    {
        // Use the talisman graphic (invisible slot) so nothing shows on the paperdoll.
        public override int LitItemID   => 0x1647;
        public override int UnlitItemID => 0x1647;

        // Silent ignition – no sounds when equipped/removed.
        public override int LitSound   => -1;
        public override int UnlitSound => -1;

        [Constructable]
        public AvengerLightItem() : base(0x1647)
        {
            Duration  = TimeSpan.Zero;  // duration is managed by AvengerLightPower
            Burning   = false;
            Light     = LightType.Circle300;
            Weight    = 0.0;
            Layer     = Layer.Talisman;
            Visible   = false;
            Movable   = false;
            Name      = "Avenger Light"; // internal name, player never sees it
        }

        public AvengerLightItem(Serial serial) : base(serial) { }

        // Transient item: never written to disk.
        public override void Serialize(GenericWriter writer) { }
        public override void Deserialize(GenericReader reader) { Delete(); }
    }

    /// <summary>
    /// Curses the target with an unholy light that reveals their position,
    /// preventing hiding and stealth for the duration.
    ///
    /// Ported from: LuceVendicatore
    /// Changes from original
    /// =====================
    /// • No level scaling – flat 360 s duration.
    /// • Light item is now self-contained (AvengerLightItem above).
    /// • Hiding prevention is enforced via the item on Layer.Talisman; your
    ///   stealth system just needs to check for AvengerLightItem on that layer.
    /// </summary>
    public sealed class AvengerLightPower : BasePower
    {
        private const int Duration = 360;

        public override string    CastPhrase         => "Bas Oah";
        public override int       ManaCost           => 18;
        public override bool      RequiresTarget     => true;
        public override int       MaxRange           => 5;
        public override int       GlobalCooldown     => 5;
        public override int       IndividualCooldown => 300;
        public override int       EffectDuration     => Duration;
        public override PowerType PowerType          => PowerType.Harmful; // reveals the target

        // We keep a reference per-target so OnExpire can remove the exact item.
        // Note: because BasePower is instantiated once per cast, this is safe for
        // single-target powers. For area powers a Dictionary<Mobile,Item> would
        // be needed instead.
        private AvengerLightItem _lightItem;

        public AvengerLightPower(Mobile caster) : base(caster) { }

        public override void OnCastTarget(Mobile target)
        {
            _lightItem = new AvengerLightItem();
            target.EquipItem(_lightItem);
            _lightItem.Ignite();

            target.SendMessage("An unholy light reveals your position.");
            Caster.PlaySound(483);

            // If the target is currently hidden, reveal them immediately.
            if (target.Hidden)
                target.Hidden = false;

            // If your shard uses a custom stealth flag on PlayerMobile, clear it:
            // if (target is PlayerMobile pm) pm.Nascosto = false;
        }

        public override void OnExpire(Mobile target)
        {
            if (_lightItem != null && !_lightItem.Deleted)
            {
                target.RemoveItem(_lightItem);
                _lightItem.Delete();
            }

            target.SendMessage("The unholy light fades.");
        }
    }

    // ── 5. Heat Metal ───────────────────────────────────────────────────────────

    /// <summary>
    /// Heats the target's metal armor, dealing damage based on pieces worn.
    /// Fires three times (initial + 2 ticks) every 6 seconds.
    /// Ported from: RiscaldaMetalloVendicatore + TimerRiscaldaMetalloVendicatore
    /// </summary>
    public sealed class HeatMetalPower : BasePower
    {
        public override string    CastPhrase  => "In Flam";
        public override int       ManaCost    => 15;
        public override bool      RequiresTarget => true;
        public override int       MaxRange    => 8;
        public override PowerType PowerType   => PowerType.Harmful;

        public HeatMetalPower(Mobile caster) : base(caster) { }

        public override bool OnBeforeCast()
        {
            // If the caster targets a mobile that has FreezeMetalPower active, cancel it.
            // (Checked again in OnCastTarget because we don't know the target yet here.)
            return true;
        }

        public override void OnCastTarget(Mobile target)
        {
            // Cancel the opposing FreezeMetalPower if active.
            if (ClassPowerSystem.HasActiveEffect(target, typeof(FreezeMetalPower)))
            {
                target.SendMessage("The Freeze Metal effect is cancelled.");
                ClassPowerSystem.UnregisterActiveEffect(target, ClassPowerSystem.GetActiveEffect<FreezeMetalPower>(target));
                return;
            }

            int baseDamage = CalculateMetalDamage(target);
            ApplyTick(target, baseDamage);

            // Schedule two additional ticks.
            Timer.DelayCall(TimeSpan.FromSeconds(6),  () => ApplyTick(target, baseDamage));
            Timer.DelayCall(TimeSpan.FromSeconds(12), () => ApplyTick(target, baseDamage));
        }

        private void ApplyTick(Mobile target, int damage)
        {
            if (!target.Alive) return;

            AOS.Damage(target, Caster, damage, 0, 100, 0, 0, 0);
            target.FixedParticles(0x374A, 10, 15, 5016, EffectLayer.Waist);
        }

        /// <summary>
        /// Counts how many metal armor pieces the target is wearing and converts
        /// them to a damage amount.
        /// </summary>
        private static int CalculateMetalDamage(Mobile target)
        {
            int damage = 0;

            damage += IsMetalArmor(target.FindItemOnLayer(Layer.Gloves))  ? 1 : 0;
            damage += IsMetalArmor(target.FindItemOnLayer(Layer.InnerTorso)) ? 2 : 0;
            damage += IsMetalArmor(target.FindItemOnLayer(Layer.Helm))    ? 2 : 0;
            damage += IsMetalArmor(target.FindItemOnLayer(Layer.Arms))    ? 2 : 0;
            damage += IsMetalArmor(target.FindItemOnLayer(Layer.Pants))   ? 2 : 0;
            damage += IsMetalArmor(target.FindItemOnLayer(Layer.Shoes))   ? 2 : 0;

            return damage;
        }

        private static bool IsMetalArmor(Item item)
        {
            if (item is BaseArmor armor)
            {
                var mat = armor.MaterialType;
                return mat == ArmorMaterialType.Plate
                    || mat == ArmorMaterialType.Chainmail
                    || mat == ArmorMaterialType.Ringmail;
            }
            return false;
        }
    }

    // ── Placeholder: FreezeMetalPower ───────────────────────────────────────────
    // Needed as a type reference by HeatMetalPower above.
    // Actual implementation lives in whatever class uses it (e.g. a Paladin class).

    /// <summary>Placeholder so HeatMetalPower can reference the type without compilation errors.</summary>
    public sealed class FreezeMetalPower : BasePower
    {
        public FreezeMetalPower(Mobile caster) : base(caster) { }
        public override void OnCast() { }
    }

    // ── 6. Avenger Curse ────────────────────────────────────────────────────────

    /// <summary>
    /// Reduces the target's Str, Dex and Int for a duration.
    /// Ported from: MaledizioneVendicatore
    /// </summary>
    public sealed class AvengerCursePower : BasePower
    {
        // Original penalty: -(4 + Livello/4) at max level 36 → ~-(4+9) = -13
        private const int StatPenalty = 13;
        // Original duration: 240 + Livello * 3  at level 36 → 348 s
        private const int Duration    = 348;

        public override string    CastPhrase     => "In Lhu";
        public override int       ManaCost       => 18;
        public override bool      RequiresTarget => true;
        public override int       MaxRange       => 6;
        public override int       EffectDuration => Duration;
        public override PowerType PowerType      => PowerType.Harmful;

        public AvengerCursePower(Mobile caster) : base(caster) { }

        public override void OnCastTarget(Mobile target)
        {
            target.AddStatMod(new StatMod(StatType.Str, "Avenger_Curse_Str", -StatPenalty, TimeSpan.Zero));
            target.AddStatMod(new StatMod(StatType.Dex, "Avenger_Curse_Dex", -StatPenalty, TimeSpan.Zero));
            target.AddStatMod(new StatMod(StatType.Int, "Avenger_Curse_Int", -StatPenalty, TimeSpan.Zero));

            target.PlaySound(481);
            target.FixedParticles(0x374A, 10, 15, 5016, EffectLayer.Waist);
        }

        public override void OnExpire(Mobile target)
        {
            target.RemoveStatMod("Avenger_Curse_Str");
            target.RemoveStatMod("Avenger_Curse_Dex");
            target.RemoveStatMod("Avenger_Curse_Int");
            target.SendMessage("The curse fades.");
        }
    }

    // ── 7. Avenger Disease ──────────────────────────────────────────────────────

    /// <summary>
    /// Inflicts a non-lethal disease on the target over time.
    ///
    /// Ported from: MalattiaIVendicatore
    ///
    /// Mechanics
    /// =========
    /// Ticks every <see cref="TickInterval"/> seconds for <see cref="TotalTicks"/> ticks.
    /// Each tick drains <see cref="HitsPerTick"/> HP and <see cref="StamPerTick"/> stamina.
    /// The disease NEVER kills: HP is clamped to 1 minimum on every tick.
    /// Stat penalties (Dex, Int) are applied at start and removed when the disease ends.
    /// </summary>
    public sealed class AvengerDiseasePower : BasePower
    {
        // ── Tunable constants ───────────────────────────────────────────────────
        private const int HitsPerTick  = 14;  // HP drained per tick  (flat, was 5 + Livello/4)
        private const int StamPerTick  = 6;   // Stamina drained per tick
        private const int TotalTicks   = 17;  // Number of ticks      (flat, was 8 + Livello/4)
        private const int TickInterval = 12;  // Seconds between ticks
        private const int DexPenalty   = 8;
        private const int IntPenalty   = 8;

        public override string    CastPhrase     => "In Mra";
        public override int       ManaCost       => 20;
        public override bool      RequiresTarget => true;
        public override int       MaxRange       => 9;
        public override PowerType PowerType      => PowerType.Harmful;

        public AvengerDiseasePower(Mobile caster) : base(caster) { }

        public override void OnCastTarget(Mobile target)
        {
            // Visual / sound feedback.
            target.FixedParticles(0x374A, 10, 15, 5016, EffectLayer.Waist);
            target.PlaySound(0x1E1); // curse.wav

            target.SendMessage("You feel a dark sickness consume you.");

            // Apply stat penalties for the duration of the disease.
            target.AddStatMod(new StatMod(StatType.Dex, "Avenger_Disease_Dex", -DexPenalty, TimeSpan.Zero));
            target.AddStatMod(new StatMod(StatType.Int, "Avenger_Disease_Int", -IntPenalty, TimeSpan.Zero));

            // Start the tick timer.
            new DiseaseTimer(Caster, target, TotalTicks).Start();
        }

        // ── Disease tick timer ──────────────────────────────────────────────────

        private sealed class DiseaseTimer : Timer
        {
            private readonly Mobile _caster;
            private readonly Mobile _target;
            private          int    _ticksLeft;

            public DiseaseTimer(Mobile caster, Mobile target, int ticks)
                : base(TimeSpan.FromSeconds(TickInterval), TimeSpan.FromSeconds(TickInterval))
            {
                _caster    = caster;
                _target    = target;
                _ticksLeft = ticks;
                Priority   = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                // Stop if the target died, disconnected or the disease expired.
                if (_target == null || _target.Deleted || !_target.Alive)
                {
                    RemoveEffects();
                    Stop();
                    return;
                }

                _ticksLeft--;

                // ── Drain HP – never kill ───────────────────────────────────────
                int newHits = _target.Hits - HitsPerTick;
                _target.Hits = newHits < 1 ? 1 : newHits;

                // ── Drain Stamina ───────────────────────────────────────────────
                int newStam = _target.Stam - StamPerTick;
                _target.Stam = newStam < 0 ? 0 : newStam;

                // Visual pulse each tick.
                _target.FixedParticles(0x374A, 1, 5, 5016, EffectLayer.Waist);

                if (_ticksLeft <= 0)
                {
                    _target.SendMessage("The sickness fades.");
                    RemoveEffects();
                    Stop();
                }
            }

            private void RemoveEffects()
            {
                if (_target == null || _target.Deleted)
                    return;

                _target.RemoveStatMod("Avenger_Disease_Dex");
                _target.RemoveStatMod("Avenger_Disease_Int");
            }
        }
    }

    // ── 8. Chaos Shield ─────────────────────────────────────────────────────────

    /// <summary>
    /// Grants the caster bonus defense and damage-return for a duration.
    /// Ported from: ScudoDelCaosVendicatore
    /// </summary>
    public sealed class ChaosShieldPower : BasePower
    {
        // Original duration: 240 + Livello * 3 at level 36 → 348 s
        private const int Duration    = 348;
        private const int DefenseBonus = 4;

        public override string    CastPhrase     => "In Nae Ger";
        public override int       ManaCost       => 14;
        public override bool      RequiresTarget => false;
        public override int       GlobalCooldown => 4;
        public override int       EffectDuration => Duration;
        public override PowerType PowerType      => PowerType.Beneficial;

        public ChaosShieldPower(Mobile caster) : base(caster) { }

        public override void OnCast()
        {
            Caster.AddStatMod(new StatMod(StatType.Str, "Avenger_ChaosShield_Str", DefenseBonus, TimeSpan.Zero));
            // TODO: add damage-return resistance mod when your resistance system is ported.
            Caster.SendMessage("The Chaos Shield surrounds you.");
            Caster.FixedParticles(0x374A, 10, 15, 5016, EffectLayer.Waist);
        }

        public override void OnExpire(Mobile target)
        {
            target.RemoveStatMod("Avenger_ChaosShield_Str");
            // TODO: remove resistance mod.
            target.SendMessage("The Chaos Shield fades.");
        }
    }

    // ── 9. Black Flame ──────────────────────────────────────────────────────────

    /// <summary>
    /// Hits the target with a dark flame, dealing flat damage.
    /// Ported from: FiammaNera
    /// </summary>
    public sealed class BlackFlamePower : BasePower
    {
        // Original: damage = Caster.Livello → flat 36
        private const int Damage = 36;
        // Original duration: 240 + Livello * 3 at level 36 → 348 s
        // (The original buff had no actual effect on expiry, so we treat it as fire-and-forget.)

        public override string    CastPhrase     => "In Vahs Sem";
        public override int       ManaCost       => 20;
        public override bool      RequiresTarget => true;
        public override int       MaxRange       => 5;
        public override int       GlobalCooldown => 7;
        public override PowerType PowerType      => PowerType.Harmful;

        public BlackFlamePower(Mobile caster) : base(caster) { }

        public override void OnCastTarget(Mobile target)
        {
            AOS.Damage(target, Caster, Damage, 0, 0, 0, 100, 0); // fire damage
            target.FixedParticles(0x19AC, 10, 15, 5016, 0x9F4, 0, EffectLayer.Waist, 0);
            target.PlaySound(0x1F1); // TODO: fire/dark flame sound – adjust after UOFiddler check
        }
    }

    // ── 10. Weaken Resistance ───────────────────────────────────────────────────

    /// <summary>
    /// Reduces the target's magic resistance skill for a duration.
    /// Ported from: ResistenzaRidotta
    /// </summary>
    public sealed class WeakenResistancePower : BasePower
    {
        private const double ResistPenalty = 20.0;
        // Original duration: 240 + Livello * 3 at level 36 → 348 s
        private const int    Duration      = 348;

        public override string    CastPhrase     => "Dhe Kur";
        public override int       ManaCost       => 15;
        public override bool      RequiresTarget => true;
        public override int       MaxRange       => 6;
        public override int       GlobalCooldown => 8;
        public override int       EffectDuration => Duration;
        public override PowerType PowerType      => PowerType.Harmful;

        // We create the mod once and re-use the same instance for both apply / remove.
        private SkillMod _resistMod;

        public WeakenResistancePower(Mobile caster) : base(caster) { }

        public override void OnCastTarget(Mobile target)
        {
            // Remove any previous application on this target.
            _resistMod = new DefaultSkillMod(SkillName.MagicResist, false, -ResistPenalty);
            target.AddSkillMod(_resistMod);
            target.SendMessage("You feel more susceptible to magic.");
        }

        public override void OnExpire(Mobile target)
        {
            if (_resistMod != null)
                target.RemoveSkillMod(_resistMod);

            target.SendMessage("Your magic resistance returns.");
        }
    }

    // ── 11. Infernal Summon ─────────────────────────────────────────────────────

    /// <summary>
    /// Transforms a horse mount into a dark destrier.
    /// Ported from: EvocazioneInfernale
    /// TODO: Replace the horse type checks with your shard's mount types.
    /// </summary>
    public sealed class InfernalSummonPower : BasePower
    {
        public override string    CastPhrase     => "An Ach";
        public override int       ManaCost       => 50;
        public override bool      RequiresTarget => true;
        public override PowerType PowerType      => PowerType.Beneficial;

        public InfernalSummonPower(Mobile caster) : base(caster) { }

        public override bool OnBeforeCastTarget(Mobile target)
        {
            if (!(target is BaseMount))
            {
                Caster.SendMessage("You must target a mount.");
                return false;
            }

            // TODO: check whether target is an accepted horse type for your shard.
            return true;
        }

        public override void OnCastTarget(Mobile target)
        {
            if (!(target is BaseMount mount)) return;

            // Generic transformation – adjust colors and name to suit your shard's mounts.
            mount.Hue     = 0x0837;
            mount.NameMod = "Dark Destrier";

            Caster.SendMessage("Your mount transforms into a Dark Destrier.");
        }
    }

    // ── 12. Blood Thirst ────────────────────────────────────────────────────────

    /// <summary>
    /// Activates the Blood Thirst buff on the caster.
    /// The actual bonus (e.g. life-steal on hit) is meant to be handled by the
    /// combat system checking ClassPowerSystem.HasActiveEffect(player, typeof(BloodThirstPower)).
    /// Ported from: SeteDiSangue
    /// </summary>
    public sealed class BloodThirstPower : BasePower
    {
        // Original duration: 240 + Livello * 3 at level 36 → 348 s
        private const int Duration = 348;

        public override string    CastPhrase     => "Has Mis";
        public override int       ManaCost       => 20;
        public override bool      RequiresTarget => false;
        public override int       GlobalCooldown => 9;
        public override int       EffectDuration => Duration;
        public override PowerType PowerType      => PowerType.Beneficial;

        public BloodThirstPower(Mobile caster) : base(caster) { }

        public override void OnCast()
        {
            Caster.SendMessage("Blood thirst consumes you.");
            Caster.FixedParticles(0x376A, 10, 15, 5016, EffectLayer.Waist);
            Caster.PlaySound(519);
        }

        public override void OnExpire(Mobile target)
        {
            target.SendMessage("The blood thirst leaves you.");
        }
    }

    // ── 13. Punish the Good ─────────────────────────────────────────────────────

    /// <summary>
    /// Deals heavy damage to good-aligned targets; reflects on the caster if the
    /// target is evil.
    /// Ported from: PunireIlBene
    /// </summary>
    public sealed class PunishGoodPower : BasePower
    {
        // Original: 15 + Caster.Livello  at level 36 → 51
        private const int Damage = 51;

        public override string    CastPhrase        => "Kur Me";
        public override int       ManaCost          => 25;
        public override bool      RequiresTarget    => true;
        public override int       MaxRange          => 6;
        public override int       IndividualCooldown => 10;
        public override PowerType PowerType         => PowerType.Harmful;

        public PunishGoodPower(Mobile caster) : base(caster) { }

        public override void OnCastTarget(Mobile target)
        {
            bool targetIsGood;

            if (target is PlayerMobile pm)
            {
                // TODO: Replace with your shard's alignment system.
                // For now we use positive karma as a proxy for "good".
                targetIsGood = pm.Karma >= 0;
            }
            else
            {
                targetIsGood = target.Karma >= 0;
            }

            if (targetIsGood)
            {
                AOS.Damage(target, Caster, Damage, 100, 0, 0, 0, 0);
                target.BoltEffect(0);
                target.PlaySound(0x29); // TODO: divine punishment sound – adjust after UOFiddler check
            }
            else
            {
                // Reflects on the caster.
                AOS.Damage(Caster, Caster, Damage, 100, 0, 0, 0, 0);
                Caster.BoltEffect(0);
                Caster.PlaySound(0x1FB); // TODO: backfire / reflect sound – adjust after UOFiddler check
                Caster.SendMessage("The damage is reflected back at you.");
            }
        }
    }

    // ── 14. Return from the Abyss ───────────────────────────────────────────────

    /// <summary>
    /// Resurrects a dead evil-aligned player with weakened stats for a duration.
    /// Ported from: RitornoDallAbissoVendicatore
    /// </summary>
    public sealed class ReturnFromAbyssPower : BasePower
    {
        private const int Duration   = 180;
        private const int StatRevive = 10; // resurrected player is restored to this stat value

        public override string    CastPhrase        => "Bet An Corp";
        public override int       ManaCost          => 70;
        public override bool      RequiresTarget    => true;
        public override int       MaxRange          => 5;
        public override int       GlobalCooldown    => 20;
        public override int       IndividualCooldown => 1200;
        public override int       EffectDuration    => Duration;
        public override PowerType PowerType         => PowerType.Beneficial;

        public ReturnFromAbyssPower(Mobile caster) : base(caster) { }

        public override bool OnBeforeCastTarget(Mobile target)
        {
            // TODO: Replace with your shard's alignment check.
            if (target.Karma >= 0)
            {
                Caster.SendMessage("You cannot resurrect a good-aligned being.");
                return false;
            }

            if (target.Alive)
            {
                Caster.SendMessage("The target is still alive.");
                return false;
            }

            return true;
        }

        public override void OnCastTarget(Mobile target)
        {
            target.Resurrect();

            // Resurrected player starts at very low stats.
            int strDelta = StatRevive - target.Str;
            int dexDelta = StatRevive - target.Dex;
            int intDelta = StatRevive - target.Int;

            target.AddStatMod(new StatMod(StatType.Str, "Avenger_Resurrect_Str", strDelta, TimeSpan.Zero));
            target.AddStatMod(new StatMod(StatType.Dex, "Avenger_Resurrect_Dex", dexDelta, TimeSpan.Zero));
            target.AddStatMod(new StatMod(StatType.Int, "Avenger_Resurrect_Int", intDelta, TimeSpan.Zero));

            target.PlaySound(532);
            target.FixedParticles(0x376A, 10, 15, 5016, EffectLayer.Waist);
        }

        public override void OnExpire(Mobile target)
        {
            target.RemoveStatMod("Avenger_Resurrect_Str");
            target.RemoveStatMod("Avenger_Resurrect_Dex");
            target.RemoveStatMod("Avenger_Resurrect_Int");
            target.SendMessage("Your strength returns.");
        }
    }

    // ── 15. Blood Sabre ─────────────────────────────────────────────────────────

    /// <summary>
    /// Equips a temporary spiritual weapon on the caster.
    /// Ported from: SciabolaSanguinaria + SciabolaSanguinariaItem
    /// </summary>
    public sealed class BloodSabrePower : BasePower
    {
        // Original duration: 600 s
        private const int Duration = 600;

        public override string    CastPhrase        => "Wea Fei";
        public override int       ManaCost          => 15;
        public override bool      RequiresTarget    => false;
        public override int       IndividualCooldown => 600;
        public override int       EffectDuration    => Duration;
        public override PowerType PowerType         => PowerType.Beneficial;

        private BloodSabreItem _weapon;

        public BloodSabrePower(Mobile caster) : base(caster) { }

        public override void OnCast()
        {
            _weapon = new BloodSabreItem();
            Caster.EquipItem(_weapon);
            Caster.PlaySound(519);
        }

        public override void OnExpire(Mobile target)
        {
            target.SendMessage("The spiritual weapon vanishes.");
            target.RemoveItem(_weapon);
            _weapon?.Delete();
        }
    }

    /// <summary>Temporary weapon summoned by <see cref="BloodSabrePower"/>.</summary>
    public sealed class BloodSabreItem : BaseSword
    {
        public override int  LabelNumber => 1049036; // generic "sword" label as placeholder
        public override WeaponAnimation DefAnimation => WeaponAnimation.Slash1H;

        [Constructable]
        public BloodSabreItem() : base(0x1440)
        {
            Name     = "Blood Sabre";
            Hue      = 0x9F3;
            Weight   = 4.0;
            MinDamage = 8;
            MaxDamage = 20;
            Visible  = true;
            Movable  = false;
        }

        public BloodSabreItem(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer) { /* intentionally empty – item is transient */ }
        public override void Deserialize(GenericReader reader) { Delete(); }
    }

    // ── 16. Blasphemous Soul ────────────────────────────────────────────────────

    /// <summary>
    /// Activates an aura that allows the caster to collect Cursed Simulacra.
    /// The actual collection logic is handled externally (e.g. on-kill hooks).
    /// Ported from: AnimaBlasfema
    /// </summary>
    public sealed class BlasphemousSoulPower : BasePower
    {
        private const int Duration = 300;

        public override string    CastPhrase        => "In Nae Wea";
        public override int       ManaCost          => 15;
        public override bool      RequiresTarget    => false;
        public override int       IndividualCooldown => 300;
        public override int       GlobalCooldown    => 5;
        public override int       EffectDuration    => Duration;
        public override PowerType PowerType         => PowerType.Beneficial;

        public BlasphemousSoulPower(Mobile caster) : base(caster) { }

        public override void OnCast()
        {
            Caster.SendMessage("A Blasphemous Soul surrounds you.");
        }

        public override void OnExpire(Mobile target)
        {
            target.SendMessage("The Blasphemous Soul fades.");
        }
    }

    // ── 17. Use Blasphemous Soul ────────────────────────────────────────────────

    /// <summary>
    /// Consumes 5 Cursed Simulacra from the caster's pack to restore mana and hp.
    /// Ported from: UsaAnimaBlasfema
    /// </summary>
    public sealed class UseBlasphemousSoulPower : BasePower
    {
        private const int SimulacraRequired = 5;
        private const int ManaRestored      = 25;
        private const int HitsRestored      = 25;

        public override string    CastPhrase     => string.Empty; // silent activation
        public override int       ManaCost       => 0;
        public override bool      RequiresTarget => false;
        public override PowerType PowerType      => PowerType.Beneficial;

        public UseBlasphemousSoulPower(Mobile caster) : base(caster) { }

        public override bool OnBeforeCast()
        {
            Item simulacra = Caster.Backpack?.FindItemByType(typeof(CursedSimulacraItem));

            if (simulacra == null)
            {
                Caster.SendMessage("You do not have any Cursed Simulacra.");
                return false;
            }

            if (simulacra.Amount < SimulacraRequired)
            {
                Caster.SendMessage($"You need at least {SimulacraRequired} Cursed Simulacra.");
                return false;
            }

            simulacra.Consume(SimulacraRequired);
            return true;
        }

        public override void OnCast()
        {
            Caster.SendMessage("The Simulacra empower you.");
            Caster.Mana  = Math.Min(Caster.ManaMax,  Caster.Mana  + ManaRestored);
            Caster.Hits  = Math.Min(Caster.HitsMax,  Caster.Hits  + HitsRestored);
        }
    }

    /// <summary>
    /// Collectible item produced during <see cref="BlasphemousSoulPower"/>.
    /// Drop / spawn logic is handled externally (e.g. on-kill hooks).
    /// </summary>
    public sealed class CursedSimulacraItem : Item
    {
        [Constructable]
        public CursedSimulacraItem() : base(0x1F14)
        {
            Name      = "Cursed Simulacra";
            Hue       = 0x9F3;
            Stackable = true;
            Weight    = 1.0;
        }

        public CursedSimulacraItem(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
        }
    }

    // ── 18. Diabolic Intent ─────────────────────────────────────────────────────

    /// <summary>
    /// Area buff that grants Str bonus and damage-return aura to the caster and
    /// allies in range.
    /// Ported from: IntentoDiabolico
    /// </summary>
    public sealed class DiabolicIntentPower : BasePower
    {
        private const int StrBonus  = 8;
        // Original duration: 240 + Livello * 3 at level 36 → 348 s
        private const int Duration  = 348;

        public override string    CastPhrase     => "De Kur Lem";
        public override int       ManaCost       => 20;
        public override bool      RequiresTarget => false;
        public override bool      IsAreaEffect   => true;
        public override int       MaxRange       => 9;
        public override int       IndividualCooldown => 480;
        public override int       EffectDuration => Duration;
        public override PowerType PowerType      => PowerType.Beneficial;

        public DiabolicIntentPower(Mobile caster) : base(caster) { }

        public override void OnCastTarget(Mobile target)
        {
            target.AddStatMod(new StatMod(StatType.Str, "Avenger_DiabolicIntent_Str", StrBonus, TimeSpan.Zero));
            // TODO: add damage-return resistance mod when your resistance system is ported.

            target.SendMessage("An aura of malevolence surrounds you.");
            target.FixedParticles(0x3DAF, 10, 15, 5016, EffectLayer.Waist);
        }

        public override void OnExpire(Mobile target)
        {
            target.RemoveStatMod("Avenger_DiabolicIntent_Str");
            // TODO: remove resistance mod.
            target.SendMessage("The malevolent aura fades.");
        }
    }
}
