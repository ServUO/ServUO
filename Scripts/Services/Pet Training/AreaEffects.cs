using Server.Items;
using Server.Spells;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    public abstract class AreaEffect
    {
        public virtual int ManaCost => 20;
        public virtual int MaxRange => 3;
        public virtual double TriggerChance => 1.0;
        public virtual TimeSpan CooldownDuration => TimeSpan.FromSeconds(30);
        public virtual bool RequiresCombatant => true;

        public virtual int EffectRange => 5;

        public static bool CheckThinkTrigger(BaseCreature bc)
        {
            AbilityProfile profile = PetTrainingHelper.GetAbilityProfile(bc);

            if (profile != null)
            {
                AreaEffect effect = null;

                AreaEffect[] effects = profile.GetAreaEffects().Where(a => !a.IsInCooldown(bc)).ToArray();

                if (effects != null && effects.Length > 0)
                {
                    effect = effects[Utility.Random(effects.Length)];
                }

                if (effect != null)
                {
                    return effect.Trigger(bc, bc.Combatant as Mobile);
                }
            }

            return false;
        }

        public virtual bool Trigger(BaseCreature creature, Mobile combatant)
        {
            if (CheckMana(creature) && Validate(creature, combatant) && TriggerChance >= Utility.RandomDouble())
            {
                creature.Mana -= ManaCost;

                DoEffects(creature, combatant);
                AddToCooldown(creature);
                return true;
            }

            return false;
        }

        public virtual bool Validate(BaseCreature attacker, Mobile defender)
        {
            if (!attacker.Alive || attacker.Deleted || attacker.IsDeadBondedPet || attacker.BardPacified)
            {
                return false;
            }

            return !RequiresCombatant || (defender != null && defender.Alive && !defender.Deleted &&
                    !defender.IsDeadBondedPet && defender.InRange(attacker.Location, MaxRange) &&
                    defender.Map == attacker.Map && attacker.InLOS(defender));
        }

        public bool CheckMana(BaseCreature bc)
        {
            return !bc.Controlled || bc.Mana >= ManaCost;
        }

        public virtual void DoEffects(BaseCreature creature, Mobile combatant)
        {
            if (creature.Map == null || creature.Map == Map.Internal)
                return;

            int count = 0;

            foreach (Mobile m in FindValidTargets(creature, EffectRange))
            {
                count++;
                DoEffect(creature, m);
            }

            if (count > 0)
            {
                OnAfterEffects(creature, combatant);
            }
        }

        public virtual void DoEffect(BaseCreature creature, Mobile defender)
        {
        }

        public virtual void OnAfterEffects(BaseCreature creature, Mobile defender)
        {
        }

        public static IEnumerable<Mobile> FindValidTargets(BaseCreature creature, int range)
        {
            IPooledEnumerable eable = creature.GetMobilesInRange(range);

            foreach (Mobile m in eable.OfType<Mobile>())
            {
                if (ValidTarget(creature, m))
                {
                    yield return m;
                }
            }

            eable.Free();
        }

        public static bool ValidTarget(Mobile from, Mobile to)
        {
            return to != from && to.Alive && !to.IsDeadBondedPet &&
                    from.CanBeHarmful(to, false) &&
                    SpellHelper.ValidIndirectTarget(from, to) &&
                    from.InLOS(to);
        }

        public List<BaseCreature> _Cooldown;

        public bool IsInCooldown(BaseCreature m)
        {
            return _Cooldown != null && _Cooldown.Contains(m);
        }

        public void AddToCooldown(BaseCreature bc)
        {
            TimeSpan cooldown = GetCooldown(bc);

            if (cooldown != TimeSpan.MinValue)
            {
                if (_Cooldown == null)
                    _Cooldown = new List<BaseCreature>();

                _Cooldown.Add(bc);
                Timer.DelayCall(cooldown, RemoveFromCooldown, bc);
            }
        }

        public virtual TimeSpan GetCooldown(BaseCreature m)
        {
            return CooldownDuration;
        }

        public void RemoveFromCooldown(BaseCreature m)
        {
            _Cooldown.Remove(m);
        }

        public static AreaEffect[] Effects => _Effects;
        private static readonly AreaEffect[] _Effects;

        static AreaEffect()
        {
            _Effects = new AreaEffect[7];

            _Effects[0] = new AuraOfEnergy();
            _Effects[1] = new AuraOfNausea();
            _Effects[2] = new EssenceOfDisease();
            _Effects[3] = new EssenceOfEarth();
            _Effects[4] = new ExplosiveGoo();
            _Effects[5] = new AuraDamage();
            _Effects[6] = new PoisonBreath();
        }

        public static AreaEffect AuraOfEnergy => _Effects[0];

        public static AreaEffect AuraOfNausea => _Effects[1];

        public static AreaEffect EssenceOfDisease => _Effects[2];

        public static AreaEffect EssenceOfEarth => _Effects[3];

        public static AreaEffect ExplosiveGoo => _Effects[4];

        public static AreaEffect AuraDamage => _Effects[5];

        public static AreaEffect PoisonBreath => _Effects[6];
    }

    public class AuraOfEnergy : AreaEffect
    {
        public override void DoEffect(BaseCreature creature, Mobile defender)
        {
            AOS.Damage(defender, creature, Utility.RandomMinMax(20, 30), 0, 0, 0, 0, 100);

            defender.SendLocalizedMessage(1072073, false, creature.Name); //  : The creature's aura of energy is damaging you!

            creature.DoHarmful(defender);
            defender.FixedParticles(0x374A, 10, 30, 5052, 1278, 0, EffectLayer.Waist);
            defender.PlaySound(0x51D);
        }
    }

    public class AuraOfNausea : AreaEffect
    {
        public override TimeSpan CooldownDuration => TimeSpan.FromSeconds(40 + Utility.RandomDouble() * 30);
        public override int MaxRange => 4;
        public override int EffectRange => 4;
        public override int ManaCost => 100;

        public static Dictionary<Mobile, Timer> _Table;

        public override void DoEffect(BaseCreature creature, Mobile defender)
        {
            if (_Table == null)
            {
                _Table = new Dictionary<Mobile, Timer>();
            }

            if (_Table.ContainsKey(defender))
            {
                Timer timer = _Table[defender];

                if (timer != null)
                    timer.Stop();

                _Table[defender] = Timer.DelayCall(TimeSpan.FromSeconds(30), EndNausea, defender);
            }
            else
            {
                _Table.Add(defender, Timer.DelayCall(TimeSpan.FromSeconds(30), EndNausea, defender));
            }

            defender.Animate(32, 5, 1, true, false, 0); // bow animation
            defender.SendLocalizedMessage(1072068); // Your enemy's putrid presence envelops you, overwhelming you with nausea.

            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.AuraOfNausea, 1153792, 1153819, TimeSpan.FromSeconds(30), defender, "60\t60\t60\t5"));
        }

        public static void EndNausea(Mobile m)
        {
            if (_Table != null && _Table.ContainsKey(m))
            {
                _Table.Remove(m);

                BuffInfo.RemoveBuff(m, BuffIcon.AuraOfNausea);
                m.Delta(MobileDelta.WeaponDamage);

                if (_Table.Count == 0)
                    _Table = null;
            }
        }

        public static bool UnderNausea(Mobile m)
        {
            return _Table != null && _Table.ContainsKey(m);
        }
    }

    public class EssenceOfDisease : AreaEffect
    {
        public override void DoEffect(BaseCreature creature, Mobile defender)
        {
            AOS.Damage(defender, creature, Utility.RandomMinMax(20, 30), 0, 0, 0, 100, 0);

            defender.SendLocalizedMessage(1072074, false, creature.Name);

            creature.DoHarmful(defender);
            defender.FixedParticles(0x374A, 10, 30, 5052, 1272, 0, EffectLayer.Waist);
            defender.PlaySound(0x476);
        }
    }

    public class EssenceOfEarth : AreaEffect
    {
        public override void DoEffect(BaseCreature creature, Mobile defender)
        {
            AOS.Damage(defender, creature, Utility.RandomMinMax(20, 30), 100, 0, 0, 0, 0);

            defender.SendLocalizedMessage(1072075, false, creature.Name);

            creature.DoHarmful(defender);
            defender.FixedParticles(0x374A, 10, 30, 5052, 1836, 0, EffectLayer.Waist);
            defender.PlaySound(0x22C);
        }
    }

    public class ExplosiveGoo : AreaEffect
    {
        public override int ManaCost => 30;

        private bool _DoingEffect;

        public override void DoEffects(BaseCreature creature, Mobile combatant)
        {
            if (_DoingEffect)
                return;

            Server.Effects.SendTargetParticles(creature, 0x3709, 10, 15, 2724, 0, 9907, EffectLayer.LeftFoot, 0);
            creature.PlaySound(0x348);

            _DoingEffect = true;

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), () =>
                {
                    base.DoEffects(creature, combatant);
                    _DoingEffect = false;
                });
        }

        public override void DoEffect(BaseCreature creature, Mobile defender)
        {
            Timer.DelayCall(TimeSpan.FromMilliseconds(Utility.RandomMinMax(10, 1000)), m =>
            {
                if (m.Alive && !m.Deleted && m.Map != null)
                {
                    Point3D p = m.Location;
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            Server.Effects.SendLocationEffect(new Point3D(p.X + x, p.Y + y, p.Z), m.Map, 0x3728, 13, 1921, 3);
                        }
                    }

                    creature.DoHarmful(defender);
                    AOS.Damage(m, creature, Utility.RandomMinMax(30, 40), 0, 100, 0, 0, 0);
                    m.SendLocalizedMessage(1112366); // The flammable goo covering you bursts into flame!
                }
            }, defender);
        }
    }

    public class PoisonBreath : AreaEffect
    {
        public override double TriggerChance => 0.4;
        public override int EffectRange => 10;
        public override int ManaCost => 50;

        public override void DoEffect(BaseCreature creature, Mobile m)
        {
            m.ApplyPoison(creature, GetPoison(creature));

            Server.Effects.SendLocationParticles(
                EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x36B0, 1, 14, 63, 7, 9915, 0);

            Server.Effects.PlaySound(m.Location, m.Map, 0x229);
            int damage = GetDamage(creature);

            if (damage > 0)
            {
                creature.DoHarmful(m);
                AOS.Damage(m, creature, damage, 0, 0, 0, 100, 0);
            }
        }

        public override void OnAfterEffects(BaseCreature creature, Mobile defender)
        {
            if (creature.Controlled)
            {
                AbilityProfile profile = PetTrainingHelper.GetAbilityProfile(creature);

                if ((profile != null && profile.HasAbility(MagicalAbility.Poisoning)) || 0.2 > Utility.RandomDouble())
                    creature.CheckSkill(SkillName.Poisoning, 0, creature.Skills[SkillName.Poisoning].Cap);
            }
        }

        public Poison GetPoison(BaseCreature bc)
        {
            int level = 0;
            double total = bc.Skills[SkillName.Poisoning].Value;

            if (total >= 100)
                level = 4;
            else if (total > 60)
                level = 3;
            else if (total > 40)
                level = 2;
            else if (total > 20)
                level = 1;

            return Poison.GetPoison(level);
        }

        public int GetDamage(BaseCreature bc)
        {
            if (_DamageCreatures.Any(t => t == bc.GetType()))
                return 50;

            return 0;
        }

        private readonly Type[] _DamageCreatures =
        {
            typeof(ValoriteElemental), typeof(BronzeElemental), typeof(Dimetrosaur), typeof(ChiefParoxysmus)
        };
    }

    public class AuraDamage : AreaEffect
    {
        public override double TriggerChance => 0.4;
        public override int EffectRange => 3;
        public override int ManaCost => 0;
        public override bool RequiresCombatant => false;

        public override TimeSpan GetCooldown(BaseCreature bc)
        {
            return AuraDefinition.GetDefinition(bc).Cooldown;
        }

        public override void DoEffect(BaseCreature creature, Mobile m)
        {
            AuraDefinition def = AuraDefinition.GetDefinition(creature);

            if (def.Damage > 0)
            {
                AOS.Damage(
                    m,
                    creature,
                    def.Damage,
                    def.Physical,
                    def.Fire,
                    def.Cold,
                    def.Poison,
                    def.Energy,
                    def.Chaos,
                    def.Direct,
                    DamageType.SpellAOE);

                creature.DoHarmful(m); // Need to re-look at this.
                m.RevealingAction();
            }

            if (creature is IAuraCreature)
            {
                ((IAuraCreature)creature).AuraEffect(m);
            }
        }

        public class AuraDefinition
        {
            public TimeSpan Cooldown { get; set; }

            public int Damage { get; set; }
            public int Physical { get; set; }
            public int Fire { get; set; }
            public int Cold { get; set; }
            public int Poison { get; set; }
            public int Energy { get; set; }
            public int Chaos { get; set; }
            public int Direct { get; set; }

            public Type[] Uses { get; private set; }

            public AuraDefinition()
                : this(TimeSpan.FromSeconds(5), 5, 0, 0, 0, 0, 0, 0, 100, new Type[] { })
            {
            }

            public AuraDefinition(params Type[] uses)
                : this(TimeSpan.FromSeconds(5), 5, 0, 100, 0, 0, 0, 0, 0, uses)
            {
            }

            public AuraDefinition(TimeSpan cooldown, int baseDamage, int phys, int fire, int cold, int poison, int energy, int chaos, int direct, Type[] uses)
            {
                Cooldown = cooldown;
                Damage = baseDamage;
                Physical = phys;
                Fire = fire;
                Cold = cold;
                Poison = poison;
                Energy = energy;
                Chaos = chaos;
                Direct = direct;

                Uses = uses;
            }

            public static List<AuraDefinition> Definitions { get; private set; } = new List<AuraDefinition>();

            public static void Initialize()
            {
                AuraDefinition defaul;
                AuraDefinition cora;
                AuraDefinition fireAura;
                AuraDefinition coldAura;

                defaul = new AuraDefinition();
                Definitions.Add(defaul);

                cora = new AuraDefinition(typeof(CoraTheSorceress))
                {
                    Damage = 10,
                    Fire = 0
                };
                Definitions.Add(cora);

                fireAura = new AuraDefinition(typeof(FlameElemental), typeof(FireDaemon), typeof(LesserFlameElemental))
                {
                    Damage = 7
                };
                Definitions.Add(fireAura);

                coldAura = new AuraDefinition(typeof(ColdDrake), typeof(FrostDrake), typeof(FrostDragon), typeof(SnowElemental), typeof(FrostMite), typeof(IceFiend), typeof(IceElemental), typeof(CorporealBrume))
                {
                    Damage = 15,
                    Fire = 0,
                    Cold = 100
                };
                Definitions.Add(coldAura);
            }

            public static AuraDefinition GetDefinition(BaseCreature bc)
            {
                AuraDefinition def = Definitions.FirstOrDefault(d => d.Uses.Any(t => t == bc.GetType()));

                if (def == null)
                {
                    return Definitions[0]; // Default
                }

                return def;
            }
        }
    }
}
