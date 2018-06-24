using System;
using Server;
using System.Collections.Generic;
using Server.Spells;
using System.Linq;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
	public abstract class AreaEffect
	{
		public virtual int ManaCost { get { return 10;  } }
		public virtual int MaxRange { get { return 3; } }
		public virtual double TriggerChance { get { return 1.0; } }
		public virtual TimeSpan CooldownDuration { get { return TimeSpan.FromSeconds(30); } }

        public virtual int EffectRange { get { return 5; } }
		
		public AreaEffect()
		{
		}
		
		public static bool CheckThinkTrigger(BaseCreature bc)
		{
			var combatant = bc.Combatant;

			if(combatant is Mobile)
			{
                var profile = PetTrainingHelper.GetAbilityProfile(bc);

                if (profile != null)
                {
                    AreaEffect effect = null;

                    var effects = profile.GetAreaEffects().Where(a => !a.IsInCooldown(bc)).ToArray();

                    if (effects != null && effects.Length > 0)
                    {
                        effect = effects[Utility.Random(effects.Length)];
                    }

                    if (effect != null)
                    {
                        return effect.Trigger(bc, (Mobile)combatant);
                    }
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
			return defender != null && defender.Alive && !defender.Deleted && !defender.IsDeadBondedPet &&
					attacker.Alive && !attacker.IsDeadBondedPet && defender.InRange(attacker.Location, MaxRange) && 
					defender.Map == attacker.Map && attacker.InLOS(defender) && !attacker.BardPacified;
		}
		
		public bool CheckMana(Mobile m)
		{
			return m.Mana >= ManaCost;
		}

        public virtual void DoEffects(BaseCreature creature, Mobile combatant)
        {
            if (creature.Map == null || creature.Map == Map.Internal)
                return;

            IPooledEnumerable eable = creature.GetMobilesInRange(EffectRange);
            List<Mobile> toAffect = new List<Mobile>();

            foreach (Mobile m in eable)
            {
                if (ValidTarget(creature, m))
                {
                    toAffect.Add(m);
                }
            }
            eable.Free();

            foreach (var m in toAffect)
            {
                DoEffect(creature, m);
            }

            if (toAffect.Count > 0)
            {
                OnAfterEffects(creature, combatant);
            }

            ColUtility.Free(toAffect);
        }

        public virtual void DoEffect(BaseCreature creature, Mobile defender)
        {
        }

        public virtual void OnAfterEffects(BaseCreature creature, Mobile defender)
        {
        }

        public static bool ValidTarget(Mobile from, Mobile to)
        {
            return to != from && to.Alive && !to.IsDeadBondedPet &&
                    from.CanBeHarmful(to, false) &&
                    SpellHelper.ValidIndirectTarget(from, to) &&
                    from.InLOS(to);
        }

		public List<Mobile> _Cooldown;
		
		public bool IsInCooldown(Mobile m)
		{
			return _Cooldown != null && _Cooldown.Contains(m);
		}
		
		public void AddToCooldown(Mobile m)
		{
			if(CooldownDuration != TimeSpan.MinValue)
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

        public static AreaEffect AuraOfEnergy
        {
            get
            {
                if (_Effects[0] == null)
                    _Effects[0] = new AuraOfEnergy();

                return _Effects[0];
            }
        }

        public static AreaEffect AuraOfNausea
        {
            get
            {
                if (_Effects[1] == null)
                    _Effects[1] = new AuraOfNausea();

                return _Effects[1];
            }
        }

        public static AreaEffect EssenceOfDisease
        {
            get
            {
                if (_Effects[2] == null)
                    _Effects[2] = new EssenceOfDisease();

                return _Effects[2];
            }
        }

        public static AreaEffect EssenceOfEarth
        {
            get
            {
                if (_Effects[3] == null)
                    _Effects[3] = new EssenceOfEarth();

                return _Effects[3];
            }
        }

        public static AreaEffect ExplosiveGoo
        {
            get
            {
                if (_Effects[4] == null)
                    _Effects[4] = new ExplosiveGoo();

                return _Effects[4];
            }
        }

        public static AreaEffect Firestorm
        {
            get
            {
                if (_Effects[5] == null)
                    _Effects[5] = new Firestorm();

                return _Effects[5];
            }
        }

        public static AreaEffect PoisonBreath
        {
            get
            {
                if (_Effects[6] == null)
                    _Effects[6] = new PoisonBreath();

                return _Effects[6];
            }
        }

        public static AreaEffect[] Effects { get { return _Effects; } }
        private static AreaEffect[] _Effects = new AreaEffect[7];
    }

    public class AuraOfEnergy : AreaEffect
    {
        public AuraOfEnergy()
        {
        }

        public override void DoEffect(BaseCreature creature, Mobile defender)
        {
            AOS.Damage(defender, creature, Utility.RandomMinMax(20, 30), 0, 0, 0, 0, 100);

            defender.FixedParticles(0x374A, 10, 30, 5052, 1278, 0, EffectLayer.Waist);
            defender.PlaySound(0x51D);
        }
    }

    public class AuraOfNausea : AreaEffect
    {
        public override TimeSpan CooldownDuration { get { return TimeSpan.FromSeconds(40 + Utility.RandomDouble() * 30); } }
        public override int MaxRange { get { return 4; } }
        public override int EffectRange { get { return 4; } }
        public override int ManaCost { get { return 100; } }

        public static Dictionary<Mobile, Timer> _Table;

        public AuraOfNausea()
        {
        }

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

                _Table[defender] = Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(30), EndNausea, defender);
            }
            else
            {
                _Table.Add(defender, Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(30), EndNausea, defender));
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
        public EssenceOfDisease()
        {
        }

        public override void DoEffect(BaseCreature creature, Mobile defender)
        {
            AOS.Damage(defender, creature, Utility.RandomMinMax(20, 30), 0, 0, 0, 100, 0);

            defender.FixedParticles(0x374A, 10, 30, 5052, 1272, 0, EffectLayer.Waist);
            defender.PlaySound(0x476);
        }
    }

    public class EssenceOfEarth : AreaEffect
    {
        public EssenceOfEarth()
        {
        }

        public override void DoEffect(BaseCreature creature, Mobile defender)
        {
            AOS.Damage(defender, creature, Utility.RandomMinMax(20, 30), 100, 0, 0, 0, 0);

            defender.FixedParticles(0x374A, 10, 30, 5052, 1836, 0, EffectLayer.Waist);
            defender.PlaySound(0x22C);
        }
    }

    public class ExplosiveGoo : AreaEffect
    {
        public override int ManaCost { get { return 30; } }

        private bool _DoingEffect;

        public ExplosiveGoo()
        {
        }

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
            Timer.DelayCall<Mobile>(TimeSpan.FromMilliseconds(Utility.RandomMinMax(10, 1000)), m =>
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

                    AOS.Damage(m, creature, Utility.RandomMinMax(30, 40), 0, 100, 0, 0, 0);
                    m.SendLocalizedMessage(1112366); // The flammable goo covering you bursts into flame!
                }
            }, defender);
        }
    }

    public class Firestorm : AreaEffect
    {
        public Firestorm()
        {
        }

        public override void DoEffect(BaseCreature creature, Mobile defender)
        {
        }
    }

    public class PoisonBreath : AreaEffect
    {
        public override double TriggerChance { get { return 0.4; } }
        public override int EffectRange { get { return 10; } }
        public override int ManaCost { get { return 50; } }

        public PoisonBreath()
        {
        }

        public override void DoEffect(BaseCreature creature, Mobile m)
        {
            m.ApplyPoison(creature, creature.HitAreaPoison);

            Server.Effects.SendLocationParticles(
                EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x36B0, 1, 14, 63, 7, 9915, 0);

            Server.Effects.PlaySound(m.Location, m.Map, 0x229);

            if (creature.AreaPoisonDamage > 0)
            {
                AOS.Damage(m, creature, creature.AreaPoisonDamage, 0, 0, 0, 100, 0);
            }
        }

        public override void OnAfterEffects(BaseCreature creature, Mobile defender)
        {
            if (creature.Controlled)
            {
                var profile = PetTrainingHelper.GetAbilityProfile(creature);

                if ((profile != null && profile.HasAbility(MagicalAbility.Poisoning)) || 0.2 > Utility.RandomDouble())
                    creature.CheckSkill(SkillName.Poisoning, 0, creature.Skills[SkillName.Poisoning].Cap);
            }
        }
    }
}