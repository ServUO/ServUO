using System;
using Server;
using System.Collections.Generic;
using Server.Spells;

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
		
		public static void CheckThinkTrigger(BaseCreature bc)
		{
			var combatant = bc.Combatant;
			
			if(combatant is Mobile)
			{
                var profile = PetTrainingHelper.GetProfile(bc);

                if (profile != null)
                {
                    AreaEffect effect = null;

                    var effects = profile.GetAreaEffects().Where(a => !a.IsInCooldown(attacker));

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
            if (CheckMana(attacker) && Validate(attacker, combatant) && TriggerChance >= Utility.RandomDouble())
			{
                DoEffects(creature, combatant);
				AddToCooldown(attacker);
			}
		}
		
		public virtual bool Validate(BaseCreater attacker, Mobile defender)
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
            if (Creature.Map == null || Creature.Map == Map.Internal)
                return;

            IPooledEnumerable eable = creature.GetMobilesInRange(EffectRange);
            List<Mobile> toAffect = new List<Mobile>();

            foreach (Mobile m in eable)
            {
                if (m.Alive && !m.IsDeadBondedPet &&
                    m.CanBeHarmful(creature) &&
                    SpellHelper.ValidIndirectTarget(m, creature) && 
                    (!Core.AOS || creature.InLOS(m)))
                {
                    toAffect.Add(m);
                }
            }

            eable.Free();

            foreach (var m in toEffect)
            {
                DoEffect(creature, m);
            }

            ColUtility.Free(toEffect);
        }

        public virtual void DoEffect(BaseCreature creature, Mobile defender)
        {
        }
		
		public List<Mobile> _CooldownTable;
		
		public bool IsInCooldown(Mobile m)
		{
			return _Cooldown != null && _Cooldown.Contains(m);
		}
		
		public bool AddToCooldown(Mobile m)
		{
			if(CooldownDuration != TimeSpan.MinValue)
			{
				_Cooldown.Add(m);
				Timer.DelayCall<Mobile>(CooldownDuration, RemoveFromCooldown, m);
			}
		}
		
		public void RemoveFromCooldown(Mobile m)
		{
			_Cooldown.Remove(m);
		}

        public static AreaEffect AuraOfEnergy = _Effects[0];
		public static AreaEffect AuraOfNausea = _Effects[1];
		public static AreaEffect EssenceOfDisease = _Effects[2];
		public static AreaEffect EssenceOfEarth = _Effects[3];
		public static AreaEffect ExplosiveGoo = _Effects[4];
		public static AreaEffect Firestorm = _Effects[5];
        public static AreaEffect PoisonBreath = _Effects[6];

        public static AreaEffect[] Effects { get { return _Effects; } }
        private static AreaEffect[] _Effects = new AreaEffect[7];
		
		public static void Initialize()
		{
			_Effects[0] = new AuraOfEnergy();
            _Effects[1] = new AuraOfNausea();
            _Effects[2] = new EssenceOfDisease();
            _Effects[3] = new EssenceOfEarth();
            _Effects[4] = new ExplosiveGoo();
            _Effects[5] = new Firestorm();
            _Effects[6] = new PoisonBreath();
		}
    }

    public class AuraOfEnergy : AreaEffect
    {
        public AuraOfEnergy()
        {
        }

        public override void DoEffect(BaseCreature creature, Mobile defender)
        {
            AOS.Damage(m, attacker, Utility.RandomMinMax(20, 30), 0, 0, 0, 0, 100);

            m.FixedParticles(0x374A, 10, 30, 5052, 1278, 0, EffectLayer.Waist);
            m.PlaySound(0x51D);
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

            if (_Table.ContainsKey(m))
            {
                Timer timer = _Table[m];

                if (timer != null)
                    timer.Stop();

                m_Table[m] = Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(30), EndNausea, m);
            }
            else
            {
                m_Table.Add(m, Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(30), EndNausea, m));
            }

            m.Animate(32, 5, 1, true, false, 0); // bow animation
            m.SendLocalizedMessage(1072068); // Your enemy's putrid presence envelops you, overwhelming you with nausea.

            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.AuraOfNausea, 1153792, 1153819, TimeSpan.FromSeconds(30), m, "60\t60\t60\t5"));
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
            AOS.Damage(m, attacker, Utility.RandomMinMax(20, 30), 0, 0, 0, 100, 0);

            m.FixedParticles(0x374A, 10, 30, 5052, 1272, 0, EffectLayer.Waist);
            m.PlaySound(0x476);
        }
    }

    public class EssenceOfEarth : AreaEffect
    {
        public EssenceOfEarth()
        {
        }

        public override void DoEffect(BaseCreature creature, Mobile defender)
        {
            AOS.Damage(m, attacker, Utility.RandomMinMax(20, 30), 100, 0, 0, 0, 0);

            m.FixedParticles(0x374A, 10, 30, 5052, 1836, 0, EffectLayer.Waist);
            m.PlaySound(0x22C);
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
                Point3D loc = Location;
                Map map = Map;
                Item acid = new ExplosiveGoo(TimeSpan.FromSeconds(10), 5, 10);

                bool validLocation = false;
                for (int j = 0; !validLocation && j < 25; ++j)
                {
                    loc = new Point3D(loc.X + Utility.Random(-3, 3), loc.Y + Utility.Random(-3, 3), loc.Z);
                    loc.Z = map.GetAverageZ(loc.X, loc.Y);
                    validLocation = map.CanFit(loc, 16, false, false);
                }

                acid.MoveToWorld(loc, map);
            }

            IPooledEnumerable eable = creature.Map.GetClientsInRange(15);

            foreach (NetState ns in eable)
            {
                ns.SendLocalizedMessage(1112365); // Flammable goo sprays into the air!
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

        public PoisonBreath()
        {
        }

        public override void DoEffect(BaseCreature creature, Mobile m)
        {
            m.ApplyPoison(this, creature.HitAreaPoison);

            Effects.SendLocationParticles(
                EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x36B0, 1, 14, 63, 7, 9915, 0);

            Effects.PlaySound(m.Location, m.Map, 0x229);

            if (creature.AreaPoisonDamage > 0)
            {
                AOS.Damage(m, creature, creature.AreaPoisonDamage, 0, 0, 0, 100, 0);
            }
        }
    }
}