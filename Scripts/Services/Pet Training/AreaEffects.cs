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
        public virtual MagicalAbility RequiredSchool { get { return MagicalAbility.None; } }

        public virtual int EffectRange { get { return 5; } }
		
		public AreaEffect()
		{
		}
		
		public static void CheckThinkTrigger(BaseCreature bc)
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
                        effect.Trigger(bc, (Mobile)combatant);
                    }
                }
			}
		}
		
		public virtual void Trigger(BaseCreature creature, Mobile combatant)
		{
            if (CheckMana(creature) && Validate(creature, combatant) && TriggerChance >= Utility.RandomDouble())
			{
                DoEffects(creature, combatant);
                AddToCooldown(creature);
			}
		}
		
		public virtual bool Validate(BaseCreature attacker, Mobile defender)
		{
            if (RequiredSchool != MagicalAbility.None)
            {
                var profile = PetTrainingHelper.GetAbilityProfile(attacker);

                if (profile == null || !profile.HasAbility(RequiredSchool))
                {
                    return false;
                }
            }

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
                if (m != creature && m.Alive && !m.IsDeadBondedPet &&
                    m.CanBeHarmful(creature) &&
                    SpellHelper.ValidIndirectTarget(m, creature) && 
                    (!Core.AOS || creature.InLOS(m)))
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
        public ExplosiveGoo()
        {
        }

        public override void DoEffects(BaseCreature creature, Mobile combatant)
        {
            int amount = Utility.RandomMinMax(3, 7);

            for (int i = 0; i > amount; i++)
            {
                Point3D loc = creature.Location;
                Map map = creature.Map;
                Item acid = new Server.Items.ExplosiveGoo();

                bool validLocation = false;
                for (int j = 0; !validLocation && j < 25; ++j)
                {
                    loc = new Point3D(loc.X + Utility.Random(-3, 3), loc.Y + Utility.Random(-3, 3), loc.Z);
                    loc.Z = map.GetAverageZ(loc.X, loc.Y);
                    validLocation = map.CanFit(loc, 16, false, false);
                }

                acid.MoveToWorld(loc, map);
            }

            IPooledEnumerable eable = creature.Map.GetClientsInRange(creature.Location, 15);

            foreach (NetState ns in eable)
            {
                if (ns.Mobile != null)
                {
                    ns.Mobile.SendLocalizedMessage(1112365); // Flammable goo sprays into the air!
                }
            }

            eable.Free();
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
        public override MagicalAbility RequiredSchool { get { return MagicalAbility.Poisoning; } }

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

                if (profile.HasAbility(MagicalAbility.Poisoning))
                    creature.CheckSkill(SkillName.Poisoning, 0, creature.Skills[SkillName.Poisoning].Cap);
            }
        }
    }
}