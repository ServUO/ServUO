using Server.Items;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    public abstract class SpecialAbility
    {
        public virtual int ManaCost => 10;
        public virtual int MaxRange => 1;
        public virtual double TriggerChance => 0.1;
        public virtual bool RequiresCombatant => true;
        public virtual TimeSpan CooldownDuration => TimeSpan.FromSeconds(30);

        public virtual MagicalAbility RequiredSchool => MagicalAbility.None;
        public virtual bool NaturalAbility => false;

        public virtual bool TriggerOnGotMeleeDamage => false;
        public virtual bool TriggerOnDoMeleeDamage => false;
        public virtual bool TriggerOnGotSpellDamage => false;
        public virtual bool TriggerOnDoSpellDamage => false;
        public virtual bool TriggerOnThink => false;
        public virtual bool TriggerOnApproach => false;

        public abstract void DoEffects(BaseCreature creature, Mobile defender, ref int damage);

        public static void CheckCombatTrigger(Mobile attacker, Mobile defender, ref int damage, DamageType type)
        {
            if (defender == null)
                return;

            if (attacker is BaseCreature && !((BaseCreature)attacker).Summoned)
            {
                BaseCreature bc = attacker as BaseCreature;
                AbilityProfile profile = PetTrainingHelper.GetAbilityProfile(bc);

                if (profile != null)
                {
                    SpecialAbility ability = null;

                    SpecialAbility[] abilties = profile.EnumerateSpecialAbilities().Where(m =>
                        ((type == DamageType.Melee && m.TriggerOnDoMeleeDamage) || (type >= DamageType.Spell && m.TriggerOnDoSpellDamage)) &&
                        !m.IsInCooldown(attacker)).ToArray();

                    if (abilties != null && abilties.Length > 0)
                    {
                        ability = abilties[Utility.Random(abilties.Length)];
                    }

                    if (ability != null)
                    {
                        ability.Trigger(bc, defender, ref damage);
                    }
                }
            }

            if (defender is BaseCreature && !((BaseCreature)defender).Summoned)
            {
                BaseCreature bc = defender as BaseCreature;
                AbilityProfile profile = PetTrainingHelper.GetAbilityProfile(bc);

                if (profile != null)
                {
                    SpecialAbility ability = null;

                    SpecialAbility[] abilties = profile.EnumerateSpecialAbilities().Where(m =>
                        ((type == DamageType.Melee && m.TriggerOnGotMeleeDamage) || (type >= DamageType.Spell && m.TriggerOnGotSpellDamage)) &&
                        !m.IsInCooldown(defender)).ToArray();

                    if (abilties != null && abilties.Length > 0)
                    {
                        ability = abilties[Utility.Random(abilties.Length)];
                    }

                    if (ability != null)
                    {
                        ability.Trigger(bc, attacker, ref damage);
                    }
                }
            }
        }

        public static bool CheckThinkTrigger(BaseCreature bc)
        {
            IDamageable combatant = bc.Combatant;
            AbilityProfile profile = PetTrainingHelper.GetAbilityProfile(bc);

            if (combatant is Mobile)
            {
                if (profile != null)
                {
                    SpecialAbility ability = null;

                    SpecialAbility[] abilties = profile.EnumerateSpecialAbilities().Where(m => m.TriggerOnThink && !m.IsInCooldown(bc)).ToArray();

                    if (abilties != null && abilties.Length > 0)
                    {
                        ability = abilties[Utility.Random(abilties.Length)];
                    }

                    if (ability != null)
                    {
                        int d = 0;
                        ability.Trigger(bc, (Mobile)combatant, ref d);
                    }
                }
            }
            else
            {
                SpecialAbility ability = null;

                SpecialAbility[] abilties =
                    profile.EnumerateSpecialAbilities().Where(
                        m =>
                        m.TriggerOnThink &&
                        !m.IsInCooldown(bc) &&
                        !m.RequiresCombatant).ToArray();

                if (abilties != null && abilties.Length > 0)
                {
                    ability = abilties[Utility.Random(abilties.Length)];
                }

                if (ability != null)
                {
                    int d = 0;
                    return ability.Trigger(bc, bc, ref d);
                }
            }

            return false;
        }

        public static bool CheckApproachTrigger(BaseCreature bc, Mobile mobile, Point3D oldLocation)
        {
            AbilityProfile profile = PetTrainingHelper.GetAbilityProfile(bc);

            if (profile != null)
            {
                SpecialAbility ability = null;

                SpecialAbility[] abilties = profile.EnumerateSpecialAbilities().Where(m =>
                    m.TriggerOnApproach &&
                    bc.InRange(mobile.Location, m.MaxRange) &&
                    !bc.InRange(oldLocation, m.MaxRange) &&
                    !m.IsInCooldown(bc)).ToArray();

                if (abilties != null && abilties.Length > 0)
                {
                    ability = abilties[Utility.Random(abilties.Length)];
                }

                if (ability != null)
                {
                    int d = 0;
                    return ability.Trigger(bc, mobile, ref d);
                }
            }

            return false;
        }

        public virtual bool Trigger(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (CheckMana(creature) && Validate(creature, defender) && TriggerChance >= Utility.RandomDouble())
            {
                creature.Mana -= ManaCost;

                DoEffects(creature, defender, ref damage);
                AddToCooldown(creature);

                return true;
            }

            return false;
        }

        public virtual bool Validate(BaseCreature attacker, Mobile defender)
        {
            if (RequiredSchool != MagicalAbility.None)
            {
                AbilityProfile profile = PetTrainingHelper.GetAbilityProfile(attacker);

                if (profile == null || !profile.HasAbility(RequiredSchool))
                {
                    return false;
                }
            }

            return defender != null && defender.Alive && !defender.Deleted && !defender.IsDeadBondedPet &&
                    attacker.Alive && !attacker.IsDeadBondedPet && defender.InRange(attacker.Location, MaxRange) &&
                    defender.Map == attacker.Map && attacker.InLOS(defender) && !attacker.BardPacified && attacker.CanBeHarmful(defender);
        }

        public bool CheckMana(Mobile m)
        {
            return m.Mana >= ManaCost;
        }

        protected List<Mobile> _Cooldown;

        public bool IsInCooldown(Mobile m)
        {
            return _Cooldown != null && _Cooldown.Contains(m);
        }

        public virtual void AddToCooldown(BaseCreature m)
        {
            if (CooldownDuration != TimeSpan.MinValue)
            {
                if (_Cooldown == null)
                    _Cooldown = new List<Mobile>();

                _Cooldown.Add(m);
                Timer.DelayCall<Mobile>(CooldownDuration, RemoveFromCooldown, m);
            }
        }

        public void RemoveFromCooldown(Mobile m)
        {
            _Cooldown.Remove(m);
        }

        public static SpecialAbility[] Abilities => _Abilities;
        private static readonly SpecialAbility[] _Abilities;

        static SpecialAbility()
        {
            _Abilities = new SpecialAbility[29];

            _Abilities[0] = new AngryFire();
            _Abilities[1] = new ConductiveBlast();
            _Abilities[2] = new DragonBreath();
            _Abilities[3] = new GraspingClaw();
            _Abilities[4] = new Inferno();
            _Abilities[5] = new LightningForce();
            _Abilities[6] = new ManaDrain();
            _Abilities[7] = new RagingBreath();
            _Abilities[8] = new Repel();
            _Abilities[9] = new SearingWounds();
            _Abilities[10] = new StealLife();
            _Abilities[11] = new VenomousBite();
            _Abilities[12] = new ViciousBite();
            _Abilities[13] = new RuneCorruption();
            _Abilities[14] = new LifeLeech();
            _Abilities[15] = new StickySkin();
            _Abilities[16] = new TailSwipe();
            _Abilities[17] = new FlurryForce();
            _Abilities[18] = new Rage();

            // Non-Trainable
            _Abilities[19] = new Heal();
            _Abilities[20] = new HowlOfCacophony();
            _Abilities[21] = new Webbing();
            _Abilities[22] = new Anemia();
            _Abilities[23] = new BloodDisease();
            _Abilities[24] = new PoisonSpit();
            _Abilities[25] = new TrueFear();
            _Abilities[26] = new ColossalBlow();
            _Abilities[27] = new LifeDrain();
            _Abilities[28] = new ColossalRage();
        }

        public static SpecialAbility AngryFire => _Abilities[0];

        public static SpecialAbility ConductiveBlast => _Abilities[1];

        public static SpecialAbility DragonBreath => _Abilities[2];

        public static SpecialAbility GraspingClaw => _Abilities[3];

        public static SpecialAbility Inferno => _Abilities[4];

        public static SpecialAbility LightningForce => _Abilities[5];

        public static SpecialAbility ManaDrain => _Abilities[6];

        public static SpecialAbility RagingBreath => _Abilities[7];

        public static SpecialAbility Repel => _Abilities[8];

        public static SpecialAbility SearingWounds => _Abilities[9];

        public static SpecialAbility StealLife => _Abilities[10];

        public static SpecialAbility RuneCorruption => _Abilities[13];

        public static SpecialAbility LifeLeech => _Abilities[14];

        public static SpecialAbility StickySkin => _Abilities[15];

        public static SpecialAbility TailSwipe => _Abilities[16];

        public static SpecialAbility VenomousBite => _Abilities[11];

        public static SpecialAbility ViciousBite => _Abilities[12];

        public static SpecialAbility FlurryForce => _Abilities[17];

        public static SpecialAbility Rage => _Abilities[18];

        public static SpecialAbility Heal => _Abilities[19];

        public static SpecialAbility HowlOfCacophony => _Abilities[20];

        public static SpecialAbility Webbing => _Abilities[21];

        public static SpecialAbility Anemia => _Abilities[22];

        public static SpecialAbility BloodDisease => _Abilities[23];

        public static SpecialAbility PoisonSpit => _Abilities[24];

        public static SpecialAbility TrueFear => _Abilities[25];

        public static SpecialAbility ColossalBlow => _Abilities[26];

        public static SpecialAbility LifeDrain => _Abilities[27];

        public static SpecialAbility ColossalRage => _Abilities[28];
    }

    public class AngryFire : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage => true;
        public override int ManaCost => 30;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            int d = Utility.RandomMinMax(30, 40);

            AOS.Damage(defender, creature, d, 60, 20, 0, 0, 20);

            defender.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
            defender.PlaySound(0x208);

            defender.SendLocalizedMessage(1070823); // The creature hits you with its Angry Fire.    
        }
    }

    public class ConductiveBlast : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage => true;
        public override int ManaCost => 30;

        private static Dictionary<Mobile, ExpireTimer> _Table;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            ExpireTimer timer = null;

            if (_Table == null)
            {
                _Table = new Dictionary<Mobile, ExpireTimer>();
            }

            if (_Table.ContainsKey(defender))
            {
                timer = _Table[defender];
            }

            if (timer != null)
            {
                timer.DoExpire();
                defender.SendLocalizedMessage(1070828); // The creature continues to hinder your energy resistance!
            }
            else
                defender.SendLocalizedMessage(1070827); // The creature's attack has made you more susceptible to energy attacks!

            int effect = -(defender.EnergyResistance / 2);

            ResistanceMod mod = new ResistanceMod(ResistanceType.Energy, effect);

            defender.FixedEffect(0x37B9, 10, 5);
            defender.AddResistanceMod(mod);

            timer = new ExpireTimer(defender, mod, TimeSpan.FromSeconds(10.0));
            timer.Start();
            _Table[defender] = timer;
        }

        public static void RemoveFromTable(Mobile m)
        {
            if (_Table != null && _Table.ContainsKey(m))
            {
                _Table.Remove(m);
            }
        }

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly ResistanceMod m_Mod;

            public ExpireTimer(Mobile m, ResistanceMod mod, TimeSpan delay)
                : base(delay)
            {
                m_Mobile = m;
                m_Mod = mod;
                Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                m_Mobile.RemoveResistanceMod(m_Mod);
                Stop();
                RemoveFromTable(m_Mobile);
            }

            protected override void OnTick()
            {
                m_Mobile.SendLocalizedMessage(1070829); // Your resistance to energy attacks has returned.
                DoExpire();
            }
        }
    }

    public class FlurryForce : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage => true;
        public override bool NaturalAbility => true;
        public override int ManaCost => 0;

        private static Dictionary<Mobile, ExpireTimer> _Table;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            ExpireTimer timer = null;

            if (_Table == null)
            {
                _Table = new Dictionary<Mobile, ExpireTimer>();
            }

            if (_Table.ContainsKey(defender))
            {
                timer = _Table[defender];
            }

            if (timer != null)
            {
                timer.DoExpire();
                defender.SendLocalizedMessage(1070851); // The creature lands another blow in your weakened state.
            }
            else
                defender.SendLocalizedMessage(1070850); // The creature's flurry of twigs has made you more susceptible to physical attacks!

            int effect = -(defender.PhysicalResistance * 15 / 100);

            ResistanceMod mod = new ResistanceMod(ResistanceType.Physical, effect);

            defender.FixedEffect(0x37B9, 10, 5);
            defender.AddResistanceMod(mod);

            timer = new ExpireTimer(defender, mod, TimeSpan.FromSeconds(10.0));
            timer.Start();
            _Table[defender] = timer;
        }

        public static void RemoveFromTable(Mobile m)
        {
            if (_Table != null && _Table.ContainsKey(m))
            {
                _Table.Remove(m);
            }
        }

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly ResistanceMod m_Mod;

            public ExpireTimer(Mobile m, ResistanceMod mod, TimeSpan delay)
                : base(delay)
            {
                m_Mobile = m;
                m_Mod = mod;
                Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                m_Mobile.RemoveResistanceMod(m_Mod);
                Stop();
                RemoveFromTable(m_Mobile);
            }

            protected override void OnTick()
            {
                m_Mobile.SendLocalizedMessage(1070829); // Your resistance to energy attacks has returned.
                DoExpire();
            }
        }
    }

    public class DragonBreath : SpecialAbility
    {
        public override int MaxRange => 12;
        public override bool TriggerOnThink => true;
        public override int ManaCost => 30;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            DoBreath(creature, defender);
        }

        public override void AddToCooldown(BaseCreature m)
        {
            if (CooldownDuration != TimeSpan.MinValue)
            {
                if (_Cooldown == null)
                    _Cooldown = new List<Mobile>();

                DragonBreathDefinition def = DragonBreathDefinition.GetDefinition(m);

                _Cooldown.Add(m);
                Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(Utility.RandomMinMax(def.MinDelay, def.MaxDelay)), RemoveFromCooldown, m);
            }
        }

        public virtual void DoBreath(BaseCreature creature, Mobile target)
        {
            DragonBreathDefinition def = DragonBreathDefinition.GetDefinition(creature);

            creature.RevealingAction();

            if (creature.AIObject != null)
            {
                creature.AIObject.NextMove = Core.TickCount + (int)(def.StallTime * 1000);
            }

            creature.PlaySound(creature.GetAngerSound());
            creature.Animate(AnimationType.Pillage, 0);

            creature.Direction = creature.GetDirectionTo(target);

            if (def.AttacksMultipleTargets)
            {
                List<Mobile> list = Spells.SpellHelper.AcquireIndirectTargets(creature, target, creature.Map, 5).OfType<Mobile>().Where(m => m.InRange(creature.Location, MaxRange)).ToList();

                for (int i = 0; i < 5; i++)
                {
                    if (list.Count == 0)
                        break;

                    Mobile m = i == 0 ? target : list[Utility.Random(list.Count)];

                    list.Remove(m);
                    Timer.DelayCall(TimeSpan.FromSeconds(def.EffectDelay), BreathEffect_Callback, creature, m, def);
                }

                ColUtility.Free(list);
            }
            else
            {
                Timer.DelayCall(TimeSpan.FromSeconds(def.EffectDelay), BreathEffect_Callback, creature, target, def);
            }
        }

        public void BreathEffect_Callback(BaseCreature creature, Mobile target, DragonBreathDefinition def)
        {
            creature.RevealingAction();

            if (!target.Alive || !creature.CanBeHarmful(target))
            {
                return;
            }

            creature.PlaySound(def.EffectSound);

            Effects.SendMovingEffect(
                creature,
                target,
                def.EffectItemID,
                def.EffectSpeed,
                def.EffectDuration,
                def.EffectFixedDir,
                def.EffectExplodes,
                def.EffectHue,
                def.EffectRenderMode);

            Timer.DelayCall(TimeSpan.FromSeconds(def.DamageDelay), BreathDamage_Callback, creature, target, def);
        }

        public void BreathDamage_Callback(BaseCreature creature, Mobile target, DragonBreathDefinition def)
        {
            if (target is BaseCreature && ((BaseCreature)target).BreathImmune)
            {
                return;
            }

            if (creature.CanBeHarmful(target))
            {
                creature.DoHarmful(target);
                BreathDealDamage(creature, target, def);
            }
        }

        public void BreathDealDamage(BaseCreature creature, Mobile target, DragonBreathDefinition def)
        {
            if (!Spells.Bushido.Evasion.CheckSpellEvasion(target))
            {
                AOS.Damage(
                    target,
                    creature,
                    BreathComputeDamage(creature, def),
                    def.GetElementalDamage(creature, ElementType.Physical),
                    def.GetElementalDamage(creature, ElementType.Fire),
                    def.GetElementalDamage(creature, ElementType.Cold),
                    def.GetElementalDamage(creature, ElementType.Poison),
                    def.GetElementalDamage(creature, ElementType.Energy),
                    def.GetElementalDamage(creature, ElementType.Chaos),
                    def.GetElementalDamage(creature, ElementType.Direct));
            }
        }

        public int BreathComputeDamage(BaseCreature creature, DragonBreathDefinition def)
        {
            int damage = (int)(creature.Hits * def.DamageScalar);

            if (creature.IsParagon)
            {
                damage = (int)(damage / Paragon.HitsBuff);
            }

            if (damage > 200)
            {
                damage = 200;
            }

            return damage;
        }

        public class DragonBreathDefinition
        {
            public Type[] Uses { get; private set; }

            // Base damage given is: CurrentHitPoints * BreathDamageScalar
            public double DamageScalar { get; private set; }

            // Creature stops moving for 1.0 seconds while breathing
            public double StallTime { get; private set; }

            // Effect is sent 1.3 seconds after BreathAngerSound and BreathAngerAnimation is played
            public double EffectDelay { get; private set; }

            // Damage is given 1.0 seconds after effect is sent
            public double DamageDelay { get; private set; }

            // Damage types
            public int PhysicalDamage { get; private set; }
            public int FireDamage { get; private set; }
            public int ColdDamage { get; private set; }
            public int PoisonDamage { get; private set; }
            public int EnergyDamage { get; private set; }
            public int ChaosDamage { get; private set; }
            public int DirectDamage { get; private set; }

            public double MinDelay { get; private set; }
            public double MaxDelay { get; private set; }

            // Effect details and sound
            public int EffectItemID { get; private set; }
            public int EffectSpeed { get; private set; }
            public int EffectDuration { get; private set; }
            public bool EffectExplodes { get; private set; }
            public bool EffectFixedDir { get; private set; }
            public int EffectHue { get; private set; }
            public int EffectRenderMode { get; private set; }

            public int EffectSound { get; private set; }

            // Anger sound/animations
            public int AngerAnimation { get; private set; }

            public bool AttacksMultipleTargets { get; private set; }

            public static List<DragonBreathDefinition> Definitions { get; private set; } = new List<DragonBreathDefinition>();

            public static void Initialize()
            {
                Definitions.Add(new DragonBreathDefinition(
                    0.16,
                    1.0,
                    1.3,
                    1.0,
                    0, 100, 0, 0, 0, 0, 0,
                    30.0, 45.0,
                    0x36D4,
                    5,
                    0,
                    false,
                    false,
                    0,
                    0,
                    0x227,
                    12));

                // Skeletal Dragon / Renowned
                Definitions.Add(new DragonBreathDefinition(
                    0.16,
                    1.0,
                    1.3,
                    1.0,
                    0, 0, 100, 0, 0, 0, 0,
                    30.0, 45.0,
                    0x36D4,
                    5,
                    0,
                    false,
                    false,
                    0x480,
                    0,
                    0x227,
                    12,
                    false,
                    new Type[] { typeof(SkeletalDragonRenowned), typeof(SkeletalDragon) }));

                // Leviathan
                Definitions.Add(new DragonBreathDefinition(
                    0.05,
                    1.0,
                    1.3,
                    1.0,
                    70, 0, 30, 0, 0, 0, 0,
                    5.0, 7.5,
                    0x36D4,
                    5,
                    0,
                    false,
                    false,
                    0x1ED,
                    0,
                    0x227,
                    12,
                    false,
                    new Type[] { typeof(Leviathan) }));

                // Red Death
                Definitions.Add(new DragonBreathDefinition(
                    0.16,
                    1.0,
                    1.3,
                    1.0,
                    0, 0, 0, 0, 0, 100, 0,
                    5.0, 7.5,
                    0x36D4,
                    5,
                    0,
                    false,
                    false,
                    0x1ED,
                    0,
                    0x227,
                    12,
                    false,
                    new Type[] { typeof(RedDeath) }));

                // Frost Dragon/Drake
                Definitions.Add(new DragonBreathDefinition(
                    0.16,
                    1.0,
                    1.3,
                    1.0,
                    0, 0, 100, 0, 0, 0, 0,
                    30.0, 45.0,
                    0x36D4,
                    5,
                    0,
                    false,
                    false,
                    1264,
                    0,
                    0x227,
                    12,
                    false,
                    new Type[] { typeof(FrostDragon), typeof(ColdDrake) }));

                // Antlion
                Definitions.Add(new DragonBreathDefinition(
                    0.16,
                    1.0,
                    1.3,
                    1.0,
                    0, 0, 0, 100, 0, 0, 0,
                    30.0, 45.0,
                    0x36D4,
                    5,
                    0,
                    false,
                    false,
                    0x3F,
                    0,
                    0,
                    12,
                    false,
                    new Type[] { typeof(AntLion) }));

                // Rend
                Definitions.Add(new DragonBreathDefinition(
                    0.06,
                    1.0,
                    1.3,
                    1.0,
                    0, 100, 0, 0, 0, 0, 0,
                    30.0, 45.0,
                    0x36D4,
                    5,
                    0,
                    false,
                    false,
                    0,
                    0,
                    0x227,
                    12,
                    false,
                    new Type[] { typeof(Rend) }));

                // Crystal Sea Serpent
                Definitions.Add(new DragonBreathDefinition(
                   0.55,
                   1.0,
                   1.3,
                   1.0,
                   0, 0, 50, 0, 50, 0, 0,
                   30.0, 45.0,
                   0x36D4,
                   5,
                   0,
                   false,
                   false,
                   0x1ED,
                   0,
                   0x227,
                   12,
                   false,
                   new Type[] { typeof(CrystalSeaSerpent) }));

                // Crystal Hydra
                Definitions.Add(new DragonBreathDefinition(
                    0.13,
                    1.0,
                    1.3,
                    1.0,
                    0, 0, 100, 0, 0, 0, 0,
                    5.0, 7.0,
                    0x36D4,
                    5,
                    0,
                    false,
                    false,
                    0x47E,
                    0,
                    0x56D,
                    12,
                    true,
                    new Type[] { typeof(CrystalHydra) }));
            }

            public static DragonBreathDefinition GetDefinition(BaseCreature bc)
            {
                DragonBreathDefinition def = Definitions.FirstOrDefault(d => d.Uses != null && d.Uses.Any(type => type == bc.GetType()));

                if (def == null)
                {
                    return Definitions[0]; // default
                }

                return def;
            }

            public DragonBreathDefinition(
                double scalar,
                double stallTime,
                double effectDelay,
                double damageDelay,
                int physDamage, int fireDamage, int coldDamage, int poisonDamage, int energyDamage, int chaosDamage, int directDamage,
                double minDelay, double maxDelay,
                int effectID,
                int effectDuration,
                int effectSpeed,
                bool explodes,
                bool fixedDirection,
                int effectHue,
                int renderMode,
                int effectSound,
                int angerAnimation,
                bool attacksMultiples = false,
                Type[] uses = null)
            {
                DamageScalar = scalar;
                StallTime = stallTime;
                EffectDelay = effectDelay;
                DamageDelay = damageDelay;

                PhysicalDamage = physDamage;
                FireDamage = fireDamage;
                ColdDamage = coldDamage;
                PoisonDamage = poisonDamage;
                EnergyDamage = energyDamage;
                ChaosDamage = chaosDamage;
                DirectDamage = directDamage;

                MinDelay = minDelay;
                MaxDelay = maxDelay;

                EffectItemID = effectID;
                EffectSpeed = effectDuration;
                EffectExplodes = explodes;
                EffectFixedDir = fixedDirection;
                EffectHue = effectHue;
                EffectRenderMode = renderMode;
                EffectSound = effectSound;
                AngerAnimation = angerAnimation;
                AttacksMultipleTargets = attacksMultiples;

                Uses = uses;
            }

            public int GetElementalDamage(BaseCreature bc, ElementType element)
            {
                switch (element)
                {
                    default:
                    case ElementType.Physical:
                        if (bc is IElementalCreature)
                        {
                            return ((IElementalCreature)bc).ElementType == ElementType.Physical ? 100 : 0;
                        }
                        else
                        {
                            return PhysicalDamage;
                        }
                    case ElementType.Fire:
                        if (bc is IElementalCreature)
                        {
                            return ((IElementalCreature)bc).ElementType == ElementType.Fire ? 100 : 0;
                        }
                        else
                        {
                            return FireDamage;
                        }
                    case ElementType.Cold:
                        if (bc is IElementalCreature)
                        {
                            return ((IElementalCreature)bc).ElementType == ElementType.Cold ? 100 : 0;
                        }
                        else
                        {
                            return ColdDamage;
                        }
                    case ElementType.Poison:
                        if (bc is IElementalCreature)
                        {
                            return ((IElementalCreature)bc).ElementType == ElementType.Poison ? 100 : 0;
                        }
                        else
                        {
                            return PoisonDamage;
                        }
                    case ElementType.Energy:
                        if (bc is IElementalCreature)
                        {
                            return ((IElementalCreature)bc).ElementType == ElementType.Energy ? 100 : 0;
                        }
                        else
                        {
                            return EnergyDamage;
                        }
                    case ElementType.Chaos:
                        if (bc is IElementalCreature)
                        {
                            return ((IElementalCreature)bc).ElementType == ElementType.Chaos ? 100 : 0;
                        }
                        else
                        {
                            return ChaosDamage;
                        }
                    case ElementType.Direct:
                        if (bc is IElementalCreature)
                        {
                            return ((IElementalCreature)bc).ElementType == ElementType.Direct ? 100 : 0;
                        }
                        else
                        {
                            return DirectDamage;
                        }
                }
            }
        }
    }

    public class HowlOfCacophony : SpecialAbility
    {
        private static Dictionary<Mobile, InternalTimer> _Table;
        public override int ManaCost => 25;
        public override bool TriggerOnDoMeleeDamage => true;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (_Table != null && _Table.ContainsKey(defender))
            {
                return;
            }

            if (_Table == null)
                _Table = new Dictionary<Mobile, InternalTimer>();

            _Table[defender] = new InternalTimer(defender);

            defender.SendSpeedControl(SpeedControlType.WalkSpeed);
            defender.SendLocalizedMessage(1072069); // // A cacophonic sound lambastes you, suppressing your ability to move.
            defender.PlaySound(0x584);

            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.HowlOfCacophony, 1153793, 1153820, TimeSpan.FromSeconds(30), defender, "60\t5\t5"));
        }

        public static bool IsUnderEffects(Mobile m)
        {
            return _Table != null && _Table.ContainsKey(m);
        }

        private class InternalTimer : Timer
        {
            public Mobile Defender { get; set; }

            public InternalTimer(Mobile defender)
                : base(TimeSpan.FromSeconds(30))
            {
                Defender = defender;
                Start();
            }

            protected override void OnTick()
            {
                if (_Table != null && _Table.ContainsKey(Defender))
                {
                    _Table.Remove(Defender);

                    BuffInfo.RemoveBuff(Defender, BuffIcon.HowlOfCacophony);
                    Defender.SendSpeedControl(SpeedControlType.Disable);
                }

                Stop();
            }
        }
    }

    public class GraspingClaw : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage => true;
        public override int ManaCost => 30;

        public static Dictionary<Mobile, ExpireTimer> _Table;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (_Table == null)
                _Table = new Dictionary<Mobile, ExpireTimer>();

            ExpireTimer timer = null;

            if (_Table.ContainsKey(defender))
                timer = _Table[defender];

            if (timer != null)
            {
                timer.DoExpire();
                defender.SendLocalizedMessage(1070837); // The creature lands another blow in your weakened state.
            }
            else
                defender.SendLocalizedMessage(1070836); // The blow from the creature's claws has made you more susceptible to physical attacks.

            int effect = -(defender.PhysicalResistance * 15 / 100);

            ResistanceMod mod = new ResistanceMod(ResistanceType.Physical, effect);

            defender.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
            defender.AddResistanceMod(mod);

            timer = new ExpireTimer(defender, mod, TimeSpan.FromSeconds(5.0));
            timer.Start();

            _Table[defender] = timer;
        }

        public static void Expire(Mobile m)
        {
            if (_Table != null && _Table.ContainsKey(m))
            {
                _Table.Remove(m);
            }
        }

        public class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly ResistanceMod m_Mod;

            public ExpireTimer(Mobile m, ResistanceMod mod, TimeSpan delay)
                : base(delay)
            {
                m_Mobile = m;
                m_Mod = mod;
                Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                m_Mobile.RemoveResistanceMod(m_Mod);
                Stop();

                Expire(m_Mobile);
            }

            protected override void OnTick()
            {
                m_Mobile.SendLocalizedMessage(1070838); // Your resistance to physical attacks has returned.
                DoExpire();
            }
        }
    }

    public class Inferno : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage => true;
        public override int ManaCost => 30;

        public static Dictionary<Mobile, ExpireTimer> _Table;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (_Table == null)
                _Table = new Dictionary<Mobile, ExpireTimer>();

            ExpireTimer timer = null;

            if (_Table.ContainsKey(defender))
                timer = _Table[defender];

            if (timer != null)
            {
                timer.DoExpire();
            }

            defender.SendLocalizedMessage(1070833); // The creature fans you with fire, reducing your resistance to fire attacks.

            ResistanceMod mod = new ResistanceMod(ResistanceType.Fire, -25);

            Effects.SendLocationParticles(defender, 0x3709, 10, 30, 5052);
            Effects.PlaySound(defender.Location, defender.Map, 0x208);

            timer = new ExpireTimer(defender, mod, TimeSpan.FromSeconds(6.0));
            timer.Start();

            AOS.Damage(defender, creature, Utility.RandomMinMax(46, 79), 0, 100, 0, 0, 0);

            _Table[defender] = timer;
        }

        public static void Expire(Mobile m)
        {
            if (_Table != null && _Table.ContainsKey(m))
            {
                _Table.Remove(m);
            }
        }

        public class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly ResistanceMod m_Mod;

            public ExpireTimer(Mobile m, ResistanceMod mod, TimeSpan delay)
                : base(delay)
            {
                m_Mobile = m;
                m_Mod = mod;
                Priority = TimerPriority.TwoFiftyMS;

                m.AddResistanceMod(mod);
            }

            public void DoExpire()
            {
                m_Mobile.RemoveResistanceMod(m_Mod);
                Stop();

                Expire(m_Mobile);
            }

            protected override void OnTick()
            {
                m_Mobile.SendLocalizedMessage(1070834); // Your resistance to fire attacks has returned.
                DoExpire();
            }
        }
    }

    public class LightningForce : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage => true;
        public override int ManaCost => 30;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            Effects.SendBoltEffect(defender, true);
            AOS.Damage(defender, creature, Utility.RandomMinMax(15, 20), 0, 0, 0, 0, 100);
        }
    }

    public class ManaDrain : SpecialAbility
    {
        public override bool TriggerOnGotMeleeDamage => true;
        public override bool TriggerOnDoMeleeDamage => true;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (creature.Map == null)
                return;

            List<Mobile> list = new List<Mobile>();
            IPooledEnumerable eable = creature.GetMobilesInRange(8);

            foreach (Mobile m in eable)
            {
                if (AreaEffect.ValidTarget(creature, m))
                    list.Add(m);
            }

            eable.Free();

            foreach (Mobile m in list)
            {
                creature.DoHarmful(m, false);

                m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                m.PlaySound(0x231);

                m.SendMessage("You feel the mana drain out of you!");

                int toDrain = Utility.RandomMinMax(40, 60);

                creature.Mana += toDrain;
                m.Mana -= toDrain;
            }
        }
    }

    public class RagingBreath : SpecialAbility
    {
        public static Dictionary<Mobile, InternalTimer> _Table;
        public override bool TriggerOnDoMeleeDamage => true;
        public override int ManaCost => 30;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (_Table == null)
                _Table = new Dictionary<Mobile, InternalTimer>();

            InternalTimer timer = null;

            if (_Table.ContainsKey(defender))
            {
                timer = _Table[defender];
            }

            if (timer != null)
            {
                timer.Stop();
                defender.SendLocalizedMessage(1070841); // The creature's breath continues to burn you!
            }
            else
            {
                defender.SendLocalizedMessage(1070842); // The creature's breath is burning you!
            }

            int layer = 0;

            if (defender is BaseCreature)
            {
                layer = 163;
            }
            else
            {
                layer = 45;
            }

            Effects.SendPacket(defender.Location, defender.Map, new ParticleEffect(EffectType.FixedFrom, defender.Serial, Serial.Zero, 0x3709, defender.Location, defender.Location, 1, 15, false, false, 2735, 0, 4, 9502, 1, defender.Serial, layer, 0));
            Effects.SendPacket(defender.Location, defender.Map, new ParticleEffect(EffectType.FixedFrom, defender.Serial, Serial.Zero, 0x3709, defender.Location, defender.Location, 10, 30, false, false, 0, 0, 0, 52, 1, defender.Serial, layer, 0));
            defender.PlaySound(520);

            timer = new InternalTimer(creature, defender);
            timer.Start();

            _Table[defender] = timer;
        }

        public static void EndFire(Mobile m)
        {
            if (_Table != null && _Table.ContainsKey(m))
            {
                _Table[m].Stop();
                _Table.Remove(m);
                m.SendLocalizedMessage(1070843); // The fiery breath dissipates.
            }
        }

        public class InternalTimer : Timer
        {
            public Mobile Attacker { get; set; }
            public Mobile Defender { get; set; }

            private int _Tick;

            public InternalTimer(Mobile attacker, Mobile defender)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                Attacker = attacker;
                Defender = defender;
            }

            protected override void OnTick()
            {
                _Tick++;

                if (_Tick > 10 || !Defender.Alive)
                {
                    EndFire(Defender);
                }
                else
                {
                    int layer = 0;

                    if (Defender is BaseCreature)
                    {
                        layer = 163;
                    }
                    else
                    {
                        layer = 45;
                    }

                    Effects.SendPacket(Defender.Location, Defender.Map, new ParticleEffect(EffectType.FixedFrom, Defender.Serial, Serial.Zero, 0x3709, Defender.Location, Defender.Location, 1, 15, false, false, 2735, 0, 4, 9502, 1, Defender.Serial, layer, 0));
                    AOS.Damage(Defender, Attacker, 10, 0, 100, 0, 0, 0);
                }
            }
        }
    }

    public class Repel : SpecialAbility
    {
        public override bool TriggerOnGotMeleeDamage => true;
        public override bool TriggerOnGotSpellDamage => true;
        public override int ManaCost => 30;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            defender.SendLocalizedMessage(1070844); // The creature repels the attack back at you.
            defender.FixedEffect(0x37B9, 10, 5);
            AOS.Damage(defender, creature, damage / 2, 0, 0, 0, 0, 0, 0, 100);

            damage = 0;
        }
    }

    public class SearingWounds : SpecialAbility
    {
        private static Dictionary<Mobile, InternalTimer> _Table;
        public override int ManaCost => 25;
        public override bool TriggerOnDoMeleeDamage => true;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (_Table != null && _Table.ContainsKey(defender))
            {
                return;
            }

            if (_Table == null)
                _Table = new Dictionary<Mobile, InternalTimer>();

            _Table[defender] = new InternalTimer(defender);

            defender.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
            defender.SendLocalizedMessage(1151177); // The searing attack cauterizes the wound on impact.
        }

        public static bool IsUnderEffects(Mobile m)
        {
            return _Table != null && _Table.ContainsKey(m);
        }

        private class InternalTimer : Timer
        {
            public Mobile Defender { get; set; }

            public InternalTimer(Mobile defender)
                : base(TimeSpan.FromSeconds(10))
            {
                Defender = defender;
                Start();
            }

            protected override void OnTick()
            {
                if (_Table != null && _Table.ContainsKey(Defender))
                {
                    _Table.Remove(Defender);
                }

                Stop();
            }
        }
    }

    public class StealLife : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage => true;
        public override int ManaCost => 30;
        public override TimeSpan CooldownDuration => TimeSpan.FromSeconds(Utility.Random(20, 40));

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            var dam = AOS.Damage(defender, creature, 22, 0, 0, 0, 0, 100);
            creature.Hits = Math.Min(creature.HitsMax, creature.Hits + dam);
            defender.SendLocalizedMessage(1070848); // You feel your life force being stolen away!

            var timer = new InternalTimer(creature, defender);
            timer.Start();
        }

        private class InternalTimer : Timer
        {
            public BaseCreature Attacker { get; set; }
            public Mobile Defender { get; set; }
            public int Ticks { get; set; }

            public InternalTimer(BaseCreature creature, Mobile defender)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                Attacker = creature;
                Defender = defender;
            }

            protected override void OnTick()
            {
                if (Ticks > 5 || !Defender.Alive)
                {
                    Defender.SendLocalizedMessage(1070849); // The drain on your life force is gone.
                    Stop();
                }
                else
                {
                    var dam = AOS.Damage(Defender, Attacker, 2, 0, 0, 0, 0, 100);
                    Attacker.Hits = Math.Min(Attacker.HitsMax, Attacker.Hits + 2);

                    Ticks++;
                }
            }
        }
    }

    public class VenomousBite : SpecialAbility
    {
        public override int ManaCost => 30;
        public override bool TriggerOnDoMeleeDamage => true;
        public override MagicalAbility RequiredSchool => MagicalAbility.Poisoning;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            IPooledEnumerable eable = creature.GetMobilesInRange(3);
            List<Mobile> list = new List<Mobile>();

            list.Add(defender);

            foreach (Mobile m in eable)
            {
                if (AreaEffect.ValidTarget(creature, m))
                    list.Add(m);
            }

            eable.Free();
            Poison p = creature.GetHitPoison();

            if (p == null)
                return;

            foreach (Mobile m in list)
            {
                defender.PlaySound(0xDD);
                defender.FixedParticles(0x3728, 244, 25, 9941, 1266, 0, EffectLayer.Waist);

                m.SendLocalizedMessage(1008097, false, creature.Name); //  : poisoned you!

                m.ApplyPoison(creature, p);
            }

            if (creature.Controlled && list.Count > 0)
            {
                AbilityProfile profile = PetTrainingHelper.GetAbilityProfile(creature);

                if ((profile != null && profile.HasAbility(MagicalAbility.Poisoning)) || 0.2 > Utility.RandomDouble())
                    creature.CheckSkill(SkillName.Poisoning, 0, creature.Skills[SkillName.Poisoning].Cap);
            }

            ColUtility.Free(list);
        }
    }

    public class ViciousBite : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage => true;
        public override int ManaCost => 20;

        private static Dictionary<Mobile, InternalTimer> _Table;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (_Table != null && _Table.ContainsKey(defender))
            {
                return;
            }

            if (_Table == null)
            {
                _Table = new Dictionary<Mobile, InternalTimer>();
            }

            defender.SendLocalizedMessage(1112472); // You've suffered a vicious bite!
            defender.SendLocalizedMessage(1113211); // The kepetch gives you a particularly vicious bite!

            Effects.SendPacket(defender.Location, defender.Map, new ParticleEffect(EffectType.FixedFrom, defender.Serial, Serial.Zero, 0x37CC, defender.Location, defender.Location, 1, 10, false, false, 0, 0, 0, 1003, 1, defender.Serial, 8, 0));

            _Table[defender] = new InternalTimer(defender);
        }

        private class InternalTimer : Timer
        {
            private Mobile Defender { get; }
            private int Damage { get; set; }

            public InternalTimer(Mobile defender)
                : base(TimeSpan.FromMinutes(1.0), TimeSpan.FromSeconds(20.0), 10)
            {
                Defender = defender;
                Damage = 5;
                Start();
            }

            protected override void OnTick()
            {
                if (!Defender.Alive || Defender.IsDeadBondedPet)
                {
                    Stop();

                    if (_Table.ContainsKey(Defender))
                    {
                        _Table.Remove(Defender);
                    }
                }
                else
                {
                    Defender.Damage(Damage);
                    Defender.SendLocalizedMessage(1112473); // Your vicious wound is festering!

                    Damage += 5;

                    if (Damage > 50)
                    {
                        _Table.Remove(Defender);
                    }
                }
            }
        }
    }

    public class RuneCorruption : SpecialAbility
    {
        public static Dictionary<Mobile, ExpireTimer> _Table;

        public override bool TriggerOnDoMeleeDamage => true;
        public override int ManaCost => 30;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (_Table == null)
                _Table = new Dictionary<Mobile, ExpireTimer>();

            ExpireTimer timer = null;

            if (_Table.ContainsKey(creature))
                timer = _Table[creature];

            if (timer != null)
            {
                timer.DoExpire();
                defender.SendLocalizedMessage(1070845); // The creature continues to corrupt your armor!
            }
            else
                defender.SendLocalizedMessage(1070846); // The creature magically corrupts your armor!

            List<ResistanceMod> mods = new List<ResistanceMod>();

            int phy = 0; int fire = 0; int cold = 0; int poison = 0; int energy = 0;

            if (defender.PhysicalResistance > 0)
            {
                phy = defender.PhysicalResistance / 2;

                mods.Add(new ResistanceMod(ResistanceType.Physical, -phy));
            }

            if (defender.FireResistance > 0)
            {
                fire = defender.FireResistance / 2;

                mods.Add(new ResistanceMod(ResistanceType.Fire, -fire));
            }

            if (defender.ColdResistance > 0)
            {
                cold = defender.ColdResistance / 2;

                mods.Add(new ResistanceMod(ResistanceType.Cold, -cold));
            }

            if (defender.PoisonResistance > 0)
            {
                poison = defender.PoisonResistance / 2;

                mods.Add(new ResistanceMod(ResistanceType.Poison, -poison));
            }

            if (defender.EnergyResistance > 0)
            {
                energy = defender.EnergyResistance / 2;

                mods.Add(new ResistanceMod(ResistanceType.Energy, -energy));
            }

            for (int i = 0; i < mods.Count; ++i)
                defender.AddResistanceMod(mods[i]);

            defender.FixedEffect(0x37B9, 10, 5);

            timer = new ExpireTimer(defender, mods, TimeSpan.FromSeconds(5.0));
            timer.Start();

            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.RuneBeetleCorruption, 1153796, 1153823, TimeSpan.FromSeconds(5.0), defender, string.Format("{0}\t{1}\t{2}\t{3}\t{4}", phy, cold, poison, energy, fire)));

            _Table[defender] = timer;
        }

        public class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly List<ResistanceMod> m_Mods;

            public ExpireTimer(Mobile m, List<ResistanceMod> mods, TimeSpan delay)
                : base(delay)
            {
                m_Mobile = m;
                m_Mods = mods;
                Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                for (int i = 0; i < m_Mods.Count; ++i)
                    m_Mobile.RemoveResistanceMod(m_Mods[i]);

                Stop();

                if (_Table != null && _Table.ContainsKey(m_Mobile))
                    _Table.Remove(m_Mobile);
            }

            protected override void OnTick()
            {
                m_Mobile.SendLocalizedMessage(1071967); // The corruption of your armor has worn off
                DoExpire();
            }
        }
    }

    public class LifeLeech : SpecialAbility
    {
        public override int ManaCost => 5;
        public override TimeSpan CooldownDuration => TimeSpan.FromSeconds(1);
        public override bool TriggerOnDoMeleeDamage => true;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            creature.Hits = Math.Min(creature.HitsMax, creature.Hits + (int)(damage * Utility.RandomMinMax(.4, .5)));
        }
    }

    public class Webbing : SpecialAbility
    {
        public override int ManaCost => 0;
        public override bool NaturalAbility => true;
        public override bool TriggerOnDoMeleeDamage => true;
        public override bool TriggerOnGotMeleeDamage => true;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (creature.Map == null)
                return;

            List<Mobile> list = new List<Mobile>();
            IPooledEnumerable eable = creature.GetMobilesInRange(12);

            foreach (Mobile m in eable)
            {
                if (AreaEffect.ValidTarget(creature, m))
                    list.Add(m);
            }

            eable.Free();

            if (list.Count > 0)
            {
                Mobile m = list[Utility.Random(list.Count)];

                creature.DoHarmful(m, false);
                creature.Direction = creature.GetDirectionTo(m);

                SpiderWebbing web = new SpiderWebbing(m);
                Effects.SendMovingParticles(creature, m, web.ItemID, 12, 0, false, false, 0, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                Timer.DelayCall(TimeSpan.FromSeconds(0.5), () => web.MoveToWorld(m.Location, m.Map));
            }
        }
    }

    public class Anemia : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage => true;
        public override bool TriggerOnGotMeleeDamage => true;
        public override bool NaturalAbility => true;
        public override int ManaCost => 0;

        private static Dictionary<Mobile, ExpireTimer> _Table;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (_Table == null)
                _Table = new Dictionary<Mobile, ExpireTimer>();

            if (_Table.ContainsKey(defender))
            {
                defender.PublicOverheadMessage(MessageType.Regular, 0x25, 1111668); // * The creature is repulsed by your diseased blood. *
            }
            else
            {
                defender.PublicOverheadMessage(MessageType.Regular, 0x25, 1111698); // *The creature drains some of your blood to replenish its health.*

                creature.Hits = Math.Min(creature.HitsMax, creature.Hits + Utility.RandomMinMax(50, 70));
            }

            TryInfect(creature, defender);
        }

        private void TryInfect(BaseCreature creature, Mobile attacker)
        {
            if (!_Table.ContainsKey(attacker) && creature.InRange(attacker, 1) && 0.25 > Utility.RandomDouble() && !FountainOfFortune.UnderProtection(attacker))
            {
                attacker.SendLocalizedMessage(1111669); // The creature's attack weakens you. You have become anemic.

                Effects.SendPacket(attacker, attacker.Map, new GraphicalEffect(EffectType.FixedFrom, attacker.Serial, Serial.Zero, 0x375A, attacker.Location, attacker.Location, 9, 20, true, false));
                Effects.SendTargetParticles(attacker, 0x373A, 1, 15, 0x26B9, EffectLayer.Head);
                Effects.SendLocationParticles(attacker, 0x11A6, 9, 32, 0x253A);

                attacker.PlaySound(0x1ED);

                ExpireTimer timer = new ExpireTimer(attacker);
                timer.Start();

                int str = attacker.RawStr / 3;
                int dex = attacker.RawDex / 3;
                int Int = attacker.RawInt / 3;

                attacker.AddStatMod(new StatMod(StatType.Str, "BloodWorm_Str", -str, TimeSpan.FromSeconds(60)));
                attacker.AddStatMod(new StatMod(StatType.Dex, "BloodWorm_Dex", -dex, TimeSpan.FromSeconds(60)));
                attacker.AddStatMod(new StatMod(StatType.Int, "BloodWorm_Int", -Int, TimeSpan.FromSeconds(60)));

                // -~1_STR~ strength.<br>-~2_INT~ intelligence.<br>-~3_DEX~ dexterity.<br> Drains all stamina.
                BuffInfo.AddBuff(attacker, new BuffInfo(BuffIcon.BloodwormAnemia, 1153797, 1153824, string.Format("{0}\t{1}\t{2}", str, dex, Int)));

                _Table.Add(attacker, timer);
            }
        }

        private class ExpireTimer : Timer
        {
            private readonly DateTime _Expires;
            private readonly Mobile m_Victim;

            public ExpireTimer(Mobile m)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_Victim = m;
                _Expires = DateTime.UtcNow + TimeSpan.FromMinutes(1);
            }

            protected override void OnTick()
            {
                if (_Expires < DateTime.UtcNow || m_Victim.Deleted || !m_Victim.Alive || m_Victim.IsDeadBondedPet)
                {
                    m_Victim.SendLocalizedMessage(1111670); // You recover from your anemia.

                    _Table.Remove(m_Victim);

                    BuffInfo.RemoveBuff(m_Victim, BuffIcon.BloodwormAnemia);

                    Stop();
                }
                else
                {
                    m_Victim.Stam -= m_Victim.Stam < 2 ? 0 : Utility.RandomMinMax(2, 5);
                }
            }
        }
    }

    public class BloodDisease : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage => true;
        public override bool TriggerOnGotMeleeDamage => true;
        public override bool NaturalAbility => true;
        public override int ManaCost => 0;

        private static Dictionary<Mobile, ExpireTimer> _Table;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (_Table == null)
                _Table = new Dictionary<Mobile, ExpireTimer>();

            if (!_Table.ContainsKey(defender) && creature.InRange(defender, 1) && 0.25 > Utility.RandomDouble() && !FountainOfFortune.UnderProtection(defender))
            {
                // The rotworm has infected you with blood disease!!
                defender.SendLocalizedMessage(1111672, "", 0x25);

                defender.PlaySound(0x213);
                Effects.SendTargetParticles(defender, 0x373A, 1, 15, 0x26B9, EffectLayer.Head);

                ExpireTimer timer = new ExpireTimer(defender, creature);
                timer.Start();

                // TODO: 2nd cliloc
                BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.RotwormBloodDisease, 1153798, 1153798));

                _Table.Add(defender, timer);
            }
        }

        private class ExpireTimer : Timer
        {
            private const int MaxCount = 8;

            private int m_Count;
            private readonly Mobile m_Victim;
            private readonly Mobile m_Attacker;

            public ExpireTimer(Mobile victim, Mobile attacker)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_Victim = victim;
                m_Attacker = attacker;
            }

            protected override void OnTick()
            {
                if (m_Count == MaxCount || m_Victim.Deleted || !m_Victim.Alive || m_Victim.IsDeadBondedPet)
                {
                    // You no longer feel sick.
                    m_Victim.SendLocalizedMessage(1111673);

                    _Table.Remove(m_Victim);

                    BuffInfo.RemoveBuff(m_Victim, BuffIcon.RotwormBloodDisease);

                    Stop();
                }
                else if (m_Count > 0)
                {
                    AOS.Damage(m_Victim, m_Attacker, Utility.RandomMinMax(10, 20), 0, 0, 0, 100, 0);
                    m_Victim.Combatant = null;
                }

                m_Count++;
            }
        }
    }

    public class StickySkin : SpecialAbility
    {
        public override int ManaCost => 5;
        public override bool TriggerOnDoMeleeDamage => true;
        public override bool TriggerOnGotMeleeDamage => true;

        public static List<Mobile> _Table;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            //TODO: Effects/Sound
            if (_Table == null)
                _Table = new List<Mobile>();

            defender.SendLocalizedMessage(1153752); // Your attack speed has been slowed.

            _Table.Add(defender);
            Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
                {
                    RemoveEffects(defender);
                });
        }

        public static bool IsUnderEffects(Mobile m)
        {
            return _Table != null && _Table.Contains(m);
        }

        public static void RemoveEffects(Mobile m)
        {
            if (_Table != null && _Table.Contains(m))
            {
                _Table.Remove(m);
            }
        }
    }

    public class TailSwipe : SpecialAbility
    {
        public override int ManaCost => 30;
        public override bool TriggerOnDoMeleeDamage => true;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (Utility.RandomBool())
            {
                defender.SendLocalizedMessage(1112554); // You're stunned as the creature's tail knocks the wind out of you.

                defender.PlaySound(0x204);
                defender.FixedEffect(0x376A, 6, 1);

                defender.Paralyze(TimeSpan.FromSeconds(3));
            }
            else
            {
                defender.SendLocalizedMessage(1112555); // You're left confused as the creature's tail catches you right in the face!

                defender.PlaySound(0x204);
                defender.FixedEffect(0x376A, 6, 1);

                defender.AddStatMod(new StatMod(StatType.Dex, "[TailSwipe] Dex", -20, TimeSpan.FromSeconds(5)));
                defender.AddStatMod(new StatMod(StatType.Int, "[TailSwipe] Int", -20, TimeSpan.FromSeconds(5)));
            }
        }
    }

    /// <summary>
    /// This is an ability of certain wild creatures. This is not a trainable pet ability
    /// </summary>
    public class Rage : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage => true;
        public override bool NaturalAbility => true;
        public override int ManaCost => 0;

        private static Dictionary<Mobile, ExpireTimer> _Table;

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (_Table == null)
                _Table = new Dictionary<Mobile, ExpireTimer>();

            ExpireTimer timer = null;

            if (_Table.ContainsKey(defender))
                timer = _Table[defender];

            if (timer != null)
            {
                timer.DoExpire();
                defender.SendLocalizedMessage(1070825); // The creature continues to rage!
            }
            else
                defender.SendLocalizedMessage(1070826); // The creature goes into a rage, inflicting heavy damage!

            timer = new ExpireTimer(defender, creature);
            timer.Start();
            _Table[defender] = timer;
        }

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly Mobile m_From;
            private int m_Count;

            public ExpireTimer(Mobile m, Mobile from)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_Mobile = m;
                m_From = from;
                Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                Stop();
                _Table.Remove(m_Mobile);
            }

            public void DrainLife()
            {
                if (m_Mobile.Alive)
                    m_Mobile.Damage(2, m_From);
                else
                    DoExpire();
            }

            protected override void OnTick()
            {
                DrainLife();

                if (++m_Count >= 5)
                {
                    DoExpire();
                    m_Mobile.SendLocalizedMessage(1070824); // The creature's rage subsides.
                }
            }
        }
    }

    /// <summary>
    /// This is an ability of certain creatures. While this ability cannot be directly trained by a tamed creaure, it is used when they have the ability to heal
    /// </summary>
    public class Heal : SpecialAbility
    {
        public override bool TriggerOnThink => true;
        public override double TriggerChance => 1.0;
        public override bool RequiresCombatant => false;
        public override int ManaCost => 15;
        public override TimeSpan CooldownDuration => TimeSpan.MinValue;
        public override bool NaturalAbility => true;

        public override bool Trigger(BaseCreature creature, Mobile defender, ref int damage)
        {
            if (CheckMana(creature) && Validate(creature, defender) && TriggerChance >= Utility.RandomDouble())
            {
                if (creature.CheckHeal())
                {
                    creature.Mana -= ManaCost;
                    AddToCooldown(creature);
                    return true;
                }
            }

            return false;
        }

        public override void DoEffects(BaseCreature creature, Mobile target, ref int damage)
        {
            creature.CheckHeal();
        }
    }

    /// <summary>
    /// This is an ability of certain wild creatures. This is not a trainable pet ability
    /// </summary>
    public class PoisonSpit : SpecialAbility
    {
        public override bool TriggerOnGotMeleeDamage => true;
        public override bool TriggerOnGotSpellDamage => true;
        public override int MaxRange => 10;

        public override bool NaturalAbility => true;
        public override int ManaCost => 0;

        public override void DoEffects(BaseCreature creature, Mobile target, ref int damage)
        {
            Effects.SendMovingEffect(creature, target, 0x36D4, 5, 0, false, true, 63, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
            {
                creature.DoHarmful(target);
                target.ApplyPoison(creature, creature.HitPoison ?? Poison.Regular);
                target.SendLocalizedMessage(1070821, creature.Name); // ~1_CREATURE~ spits a poisonous substance at you!
            });
        }
    }

    /// <summary>
    /// This is an ability of certain wild creatures. This is not a trainable pet ability
    /// </summary>
    public class TrueFear : SpecialAbility
    {
        public override bool TriggerOnApproach => true;
        public override int MaxRange => 8;

        public override bool NaturalAbility => true;
        public override int ManaCost => 0;
        public override TimeSpan CooldownDuration => TimeSpan.FromMinutes(Utility.RandomMinMax(1, 3));

        public override void DoEffects(BaseCreature creature, Mobile target, ref int damage)
        {
            int seconds = (int)Math.Max(1, (13.0 - (target.Skills[SkillName.MagicResist].Value / 10.0)));

            int number;

            if (seconds <= 2)
                number = 1080339; // A sense of discomfort passes through you, but it fades quickly
            else if (seconds <= 4)
                number = 1080340; // An unfamiliar fear washes over you, and for a moment you're unable to move
            else if (seconds <= 7)
                number = 1080341; // Panic grips you! You're unable to move, to think, to feel anything but fear!
            else if (seconds <= 10)
                number = 1080342; // Terror slices into your very being, destroying any chance of resisting ~1_name~ you might have had
            else
                number = 1080343; // Everything around you dissolves into darkness as ~1_name~'s burning eyes fill your vision

            target.SendLocalizedMessage(number, creature.Name, 0x21);
            target.Frozen = true;

            BuffInfo.AddBuff(target, new BuffInfo(BuffIcon.TrueFear, 1153791, 1153827, TimeSpan.FromSeconds(seconds), target));

            Timer.DelayCall(TimeSpan.FromSeconds(seconds), () =>
            {
                target.Frozen = false;
                target.SendLocalizedMessage(1005603); // You can move again!
            });
        }
    }

    /// <summary>
    /// This is an ability of certain wild creatures. This is not a trainable pet ability
    /// </summary>
    public class ColossalBlow : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage => true;
        public override bool NaturalAbility => true;
        public override int ManaCost => 0;
        public override double TriggerChance => 0.3;
        public override TimeSpan CooldownDuration => TimeSpan.FromSeconds(10);

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            defender.Animate(AnimationType.Die, 0);
            creature.PlaySound(0xEE);
            defender.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1070696); // You have been stunned by a colossal blow!

            BaseWeapon weapon = creature.Weapon as BaseWeapon;

            if (weapon != null)
                weapon.OnHit(creature, defender);

            if (defender.Alive)
            {
                defender.Frozen = true;

                Timer.DelayCall(TimeSpan.FromSeconds(5.0), victim =>
                {
                    victim.Frozen = false;
                    victim.Combatant = null;
                    victim.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1070695); // You recover your senses.

                }, defender);
            }
        }
    }

    /// <summary>
    /// This is the Succubus/Semidar type life drain. This is not a trainable pet ability
    /// </summary>
    public class LifeDrain : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage => true;
        public override bool TriggerOnGotMeleeDamage => true;
        public override bool NaturalAbility => true;
        public override TimeSpan CooldownDuration => TimeSpan.FromSeconds(1);

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            foreach (Mobile m in AreaEffect.FindValidTargets(creature, 2).OfType<Mobile>())
            {
                if (!CanDrainLife(creature, defender))
                {
                    continue;
                }

                creature.DoHarmful(defender);

                defender.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                defender.PlaySound(0x231);

                defender.SendMessage("You feel the life drain out of you!");

                int toDrain = GetDrainAmount(creature, defender);

                if (defender is PlayerMobile)
                {
                    toDrain = (int)LifeShieldLotion.HandleLifeDrain((PlayerMobile)defender, toDrain);
                }

                creature.Hits += toDrain;
                AOS.Damage(m, creature, toDrain, 0, 0, 0, 0, 0, 0, 100);

                creature.OnDrainLife(defender);
            }
        }

        public bool CanDrainLife(BaseCreature bc, Mobile victim)
        {
            if (bc is Succubus && victim.Female)
            {
                return false;
            }

            return true;
        }

        public int GetDrainAmount(BaseCreature bc, Mobile victim)
        {
            int amount = Utility.RandomMinMax(10, 40);

            if (bc is Semidar && !victim.Female)
            {
                amount *= 2;
            }

            return amount;
        }
    }

    /// <summary>
    /// This is an ability of certain wild creatures. This is not a trainable pet ability
    /// </summary>
    public class ColossalRage : SpecialAbility
    {
        public override bool TriggerOnGotMeleeDamage => true;
        public override bool NaturalAbility => true;
        public override int ManaCost => 0;
        public override double TriggerChance => .5;
        public override TimeSpan CooldownDuration => TimeSpan.FromSeconds(15);

        public override bool Validate(BaseCreature attacker, Mobile defender)
        {
            if (defender.Hits < (defender.HitsMax * .33))
            {
                return false;
            }

            return base.Validate(attacker, defender);
        }

        public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
        {
            AddRage(creature);

            Timer.DelayCall(TimeSpan.FromSeconds(.25), () =>
            {
                creature.PublicOverheadMessage(MessageType.Regular, 0x20, 1113587); // The creature goes into a frenzied rage!
            });

            Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
            {
                RemoveRage(creature);
            });
        }

        public static void AddRage(BaseCreature bc)
        {
            if (InRage == null)
            {
                InRage = new List<BaseCreature>();
            }

            InRage.Add(bc);
            bc.HueMod = 1157;
            bc.Stam = bc.StamMax;
        }

        public static void RemoveRage(BaseCreature bc)
        {
            if (InRage != null && InRage.Contains(bc))
            {
                bc.HueMod = -1;
                InRage.Remove(bc);

                if (InRage.Count == 0)
                {
                    InRage = null;
                }
            }
        }

        public static bool HasRage(BaseCreature bc)
        {
            return InRage != null && InRage.Contains(bc);
        }

        public static List<BaseCreature> InRage { get; set; }
    }
}
