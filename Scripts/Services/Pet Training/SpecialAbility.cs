using System;
using Server;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	public abstract class SpecialAbility
	{
		public virtual int ManaCost { get { return 10;  } }
		public virtual int MaxRange { get { return 1; } }
		public virtual double TriggerChance { get { return 0.1; } }
        public virtual bool RequiresCombatant { get { return true; } }
		public virtual TimeSpan CooldownDuration { get { return TimeSpan.FromSeconds(30); } }

        public virtual MagicalAbility RequiredSchool { get { return MagicalAbility.None; } }
        public virtual bool NaturalAbility { get { return false; } }

		public virtual bool TriggerOnGotMeleeDamage { get { return false; } }
		public virtual bool TriggerOnDoMeleeDamage { get { return false; } }
		public virtual bool TriggerOnGotSpellDamage { get { return false; } }
		public virtual bool TriggerOnDoSpellDamage { get { return false; } }
		public virtual bool TriggerOnThink { get { return false; } }

        public abstract void DoEffects(BaseCreature creature, Mobile defender, ref int damage);
		
		public SpecialAbility()
		{
		}

        public static bool CheckCombatTrigger(Mobile attacker, Mobile defender, ref int damage, DamageType type)
        {
            if(defender == null)
                return false;
            
            if (attacker is BaseCreature && !((BaseCreature)attacker).Summoned)
            {
                var bc = attacker as BaseCreature;
                var profile = PetTrainingHelper.GetAbilityProfile(bc);

                if (profile != null)
                {
                    SpecialAbility ability = null;

                    var abilties = profile.EnumerateSpecialAbilities().Where(m =>
                        (type == DamageType.Melee && m.TriggerOnDoMeleeDamage) || (type >= DamageType.Spell && m.TriggerOnDoSpellDamage) &&
                        !m.IsInCooldown(attacker)).ToArray();

                    if (abilties != null && abilties.Length > 0)
                    {
                        ability = abilties[Utility.Random(abilties.Length)];
                    }

                    if (ability != null)
                    {
                       return ability.Trigger(bc, defender, ref damage);
                    }
                }

                return false;
            }

            if (defender is BaseCreature && !((BaseCreature)defender).Summoned)
            {
                var bc = defender as BaseCreature;
                var profile = PetTrainingHelper.GetAbilityProfile(bc);
                
                if (profile != null)
                {
                    SpecialAbility ability = null;

                    var abilties = profile.EnumerateSpecialAbilities().Where(m =>
                        (type == DamageType.Melee && m.TriggerOnGotMeleeDamage) || (type >= DamageType.Spell && m.TriggerOnGotSpellDamage) &&
                        !m.IsInCooldown(attacker)).ToArray();

                    if (abilties != null && abilties.Length > 0)
                    {
                        ability = abilties[Utility.Random(abilties.Length)];
                    }

                    if (ability != null)
                    {
                        int d = 0;
                        return ability.Trigger(bc, attacker, ref d);
                    }
                }
            }

            return false;
        }
		
		public static bool CheckThinkTrigger(BaseCreature bc)
		{
			var combatant = bc.Combatant;
            var profile = PetTrainingHelper.GetAbilityProfile(bc);

            if (combatant is Mobile)
            {
                if (profile != null)
                {
                    SpecialAbility ability = null;

                    var abilties = profile.EnumerateSpecialAbilities().Where(m => m.TriggerOnThink && !m.IsInCooldown(bc)).ToArray();

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

                var abilties = profile.EnumerateSpecialAbilities().Where(m => m.TriggerOnThink && !m.IsInCooldown(bc) && !m.RequiresCombatant).ToArray();

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
		
		protected List<Mobile> _Cooldown;
		
		public bool IsInCooldown(Mobile m)
		{
			return _Cooldown != null && _Cooldown.Contains(m);
		}
		
		public virtual void AddToCooldown(BaseCreature m)
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
		
		public static SpecialAbility AngryFire
        {
            get
            {
                if (_Abilities[0] == null)
                    _Abilities[0] = new AngryFire();

                return _Abilities[0];
            }
        }

		public static SpecialAbility ConductiveBlast
        {
            get
            {
                if (_Abilities[1] == null)
                    _Abilities[1] = new ConductiveBlast();

                return _Abilities[1];
            }
        }

		public static SpecialAbility DragonBreath
        {
            get
            {
                if (_Abilities[2] == null)
                    _Abilities[2] = new DragonBreath();

                return _Abilities[2];
            }
        }

		public static SpecialAbility GraspingClaw
        {
            get
            {
                if (_Abilities[3] == null)
                    _Abilities[3] = new GraspingClaw();

                return _Abilities[3];
            }
        }

		public static SpecialAbility Inferno
        {
            get
            {
                if (_Abilities[4] == null)
                    _Abilities[4] = new Inferno();

                return _Abilities[4];
            }
        }

		public static SpecialAbility LightningForce
        {
            get
            {
                if (_Abilities[5] == null)
                    _Abilities[5] = new LightningForce();

                return _Abilities[5];
            }
        }

		public static SpecialAbility ManaDrain
        {
            get
            {
                if (_Abilities[6] == null)
                    _Abilities[6] = new ManaDrain();

                return _Abilities[6];
            }
        }

        public static SpecialAbility RagingBreath
        {
            get
            {
                if (_Abilities[7] == null)
                    _Abilities[7] = new RagingBreath();

                return _Abilities[7];
            }
        }

		public static SpecialAbility Repel
        {
            get
            {
                if (_Abilities[8] == null)
                    _Abilities[8] = new Repel();

                return _Abilities[8];
            }
        }

		public static SpecialAbility SearingWounds
        {
            get
            {
                if (_Abilities[9] == null)
                    _Abilities[9] = new SearingWounds();

                return _Abilities[9];
            }
        }

		public static SpecialAbility StealLife
        {
            get
            {
                if (_Abilities[10] == null)
                    _Abilities[10] = new StealLife();

                return _Abilities[10];
            }
        }

        public static SpecialAbility RuneCorruption
        {
            get
            {
                if (_Abilities[13] == null)
                    _Abilities[13] = new RuneCorruption();

                return _Abilities[13];
            }
        }

        public static SpecialAbility LifeLeech
        {
            get
            {
                if (_Abilities[14] == null)
                    _Abilities[14] = new LifeLeech();

                return _Abilities[14];
            }
        }

        public static SpecialAbility StickySkin
        {
            get
            {
                if (_Abilities[15] == null)
                    _Abilities[15] = new StickySkin();

                return _Abilities[15];
            }
        }

        public static SpecialAbility TailSwipe
        {
            get
            {
                if (_Abilities[16] == null)
                    _Abilities[16] = new TailSwipe();

                return _Abilities[16];
            }
        }

        public static SpecialAbility VenomousBite
        {
            get
            {
                if (_Abilities[11] == null)
                    _Abilities[11] = new VenomousBite();

                return _Abilities[11];
            }
        }

        public static SpecialAbility ViciousBite
        {
            get
            {
                if (_Abilities[12] == null)
                    _Abilities[12] = new ViciousBite();

                return _Abilities[12];
            }
        }

        public static SpecialAbility FlurryForce
        {
            get
            {
                if (_Abilities[17] == null)
                    _Abilities[17] = new FlurryForce();

                return _Abilities[17];
            }
        }

        public static SpecialAbility Rage
        {
            get
            {
                if (_Abilities[18] == null)
                    _Abilities[18] = new Rage();

                return _Abilities[18];
            }
        }

        public static SpecialAbility Heal
        {
            get
            {
                if (_Abilities[19] == null)
                    _Abilities[19] = new Heal();

                return _Abilities[19];
            }
        }

        public static SpecialAbility HowlOfCacophony
        {
            get
            {
                if (_Abilities[20] == null)
                    _Abilities[20] = new HowlOfCacophony();

                return _Abilities[20];
            }
        }

        public static SpecialAbility Webbing
        {
            get
            {
                if (_Abilities[21] == null)
                    _Abilities[21] = new Webbing();

                return _Abilities[21];
            }
        }

        public static SpecialAbility Anemia
        {
            get
            {
                if (_Abilities[22] == null)
                    _Abilities[22] = new Anemia();

                return _Abilities[22];
            }
        }

        public static SpecialAbility BloodDisease
        {
            get
            {
                if (_Abilities[23] == null)
                    _Abilities[23] = new BloodDisease();

                return _Abilities[23];
            }
        }

        public static SpecialAbility[] Abilities { get { return _Abilities; } }
        private static SpecialAbility[] _Abilities = new SpecialAbility[24];
    }
	
	public class AngryFire : SpecialAbility
	{
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

		public AngryFire()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
            int d = defender.Hits / 2;

            AOS.Damage(defender, creature, d, 60, 20, 0, 0, 20);

            defender.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
            defender.PlaySound(0x208);

            defender.SendLocalizedMessage(1070823); // The creature hits you with its Angry Fire.    
		}
	}
	
	public class ConductiveBlast : SpecialAbility
	{
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

        private static Dictionary<Mobile, ExpireTimer> _Table;

		public ConductiveBlast()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
            ExpireTimer timer = null;

            if(_Table == null)
            {
                _Table = new Dictionary<Mobile, ExpireTimer>();
            }
            
            if(_Table.ContainsKey(defender))
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
            if(_Table != null && _Table.ContainsKey(m))
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
        public override bool TriggerOnDoMeleeDamage { get { return true; } }
        public override bool NaturalAbility { get { return true; } }

        private static Dictionary<Mobile, ExpireTimer> _Table;

		public FlurryForce()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
            ExpireTimer timer = null;

            if(_Table == null)
            {
                _Table = new Dictionary<Mobile, ExpireTimer>();
            }
            
            if(_Table.ContainsKey(defender))
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
            if(_Table != null && _Table.ContainsKey(m))
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
        public override int MaxRange { get { return 12; } }

        public override bool TriggerOnThink { get { return true; } }
        
		public DragonBreath()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
            creature.BreathStart(defender);
		}

        public override void AddToCooldown(BaseCreature m)
        {
            if (CooldownDuration != TimeSpan.MinValue)
            {
                if (_Cooldown == null)
                    _Cooldown = new List<Mobile>();

                _Cooldown.Add(m);
                Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(Utility.RandomMinMax(m.BreathMinDelay, m.BreathMaxDelay)), RemoveFromCooldown, m);
            }
        }
	}

    public class HowlOfCacophony : SpecialAbility
    {
        private static Dictionary<Mobile, InternalTimer> _Table;
        public override int ManaCost { get { return 25; } }
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

        public HowlOfCacophony()
        {
        }

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
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

        public static Dictionary<Mobile, ExpireTimer> _Table;

		public GraspingClaw()
		{
		}
		
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
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

        public static Dictionary<Mobile, ExpireTimer> _Table;

        public Inferno()
		{
		}
		
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

            int effect = -(defender.FireResistance / 4);

            ResistanceMod mod = new ResistanceMod(ResistanceType.Fire, effect);

            Effects.SendLocationParticles(defender, 0x3709, 10, 30, 5052);
            Effects.PlaySound(defender.Location, defender.Map, 0x208);

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
                m_Mobile.SendLocalizedMessage(1070834); // Your resistance to fire attacks has returned.
                DoExpire();
            }
        }
	}
	
	public class LightningForce : SpecialAbility
	{
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

		public LightningForce()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
            Server.Effects.SendBoltEffect(defender, true);
            AOS.Damage(defender, creature, Utility.RandomMinMax(15, 20), 0, 0, 0, 0, 100);
		}
	}
	
	public class ManaDrain : SpecialAbility
	{
        public override bool TriggerOnGotMeleeDamage { get { return true; } }
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

		public ManaDrain()
		{
		}
		
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
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

		public RagingBreath()
		{
		}

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

            creature.PlaySound(0x227);
            defender.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);

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

                if (_Tick > 3)
                {
                    EndFire(Defender);
                }
                else
                {
                    // TODO: Effects?
                    AOS.Damage(Defender, Attacker, Utility.RandomMinMax(20, 30), 0, 100, 0, 0, 0);
                    Defender.PlaySound(0x1DD);
                }
            }
        }
	}
	
	public class Repel : SpecialAbility
	{
        public override bool TriggerOnGotMeleeDamage { get { return true; } }
        public override bool TriggerOnGotSpellDamage { get { return true; } }

		public Repel()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
            defender.SendLocalizedMessage(1070844); // The creature repels the attack back at you.
            defender.FixedEffect(0x37B9, 10, 5);

            AOS.Damage(defender, creature, damage, 0, 0, 0, 0, 0, 0, 100);

            damage = 0;
		}
	}
	
	public class SearingWounds : SpecialAbility
	{
		private static Dictionary<Mobile, InternalTimer> _Table;
		public override int ManaCost { get { return 25;  } }
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

		public SearingWounds()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
			if(_Table != null && _Table.ContainsKey(defender))
			{
				return;
			}
			
			if(_Table == null)
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
				if(_Table != null && _Table.ContainsKey(Defender))
				{
					_Table.Remove(Defender);
				}

                Stop();
			}
		}
	}
	
	public class StealLife : SpecialAbility
	{
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

		public StealLife()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
            defender.FixedParticles(0x374A, 1, 15, 5054, 23, 7, EffectLayer.Head);
            defender.PlaySound(0x1F9);

            AOS.Damage(defender, creature, damage, 0, 0, 0, 0, 100);

            new InternalTimer(creature, defender, damage);
		}
		
		private class InternalTimer : Timer
		{
			public BaseCreature Attacker { get; set; }
			public Mobile Defender { get; set; }
            public int ToHeal { get; set; }
			
			public InternalTimer(BaseCreature creature, Mobile defender, int toHeal)
				: base(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), 5)
			{
                Attacker = creature;
                Defender = defender;
                ToHeal = toHeal;

                defender.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                defender.PlaySound(0x231);

				Start();
			}
			
			protected override void OnTick()
			{
                Defender.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                Attacker.Hits = Math.Min(Attacker.HitsMax, Attacker.Hits + (ToHeal / 5));
			}
		}
	}
	
	public class VenomousBite : SpecialAbility
	{
		public override int ManaCost { get { return 50;  } }
        public override bool TriggerOnDoMeleeDamage { get { return true; } }
        public override MagicalAbility RequiredSchool { get { return MagicalAbility.Poisoning; } }

		public VenomousBite()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
            IPooledEnumerable eable = creature.GetMobilesInRange(3);
			List<Mobile> list = new List<Mobile>();
			
			list.Add(defender);
			
			foreach(Mobile m in eable)
			{
                if (AreaEffect.ValidTarget(creature, m))
                    list.Add(m);
			}
			
			eable.Free();
            Poison p = creature.GetHitPoison();

            if (p == null)
                return;

			foreach(var m in list)
			{
                defender.PlaySound(0xDD);
                defender.FixedParticles(0x3728, 244, 25, 9941, 1266, 0, EffectLayer.Waist);

                m.SendLocalizedMessage(1008097, false, creature.Name); //  : poisoned you!

                m.ApplyPoison(creature, p);
			}

            if (creature.Controlled && list.Count > 0)
            {
                var profile = PetTrainingHelper.GetAbilityProfile(creature);

                if ((profile != null && profile.HasAbility(MagicalAbility.Poisoning)) || 0.2 > Utility.RandomDouble())
                    creature.CheckSkill(SkillName.Poisoning, 0, creature.Skills[SkillName.Poisoning].Cap);
            }

            ColUtility.Free(list);
		}
	}
	
	public class ViciousBite : SpecialAbility
	{
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

        private static Dictionary<Mobile, InternalTimer> _Table;

		public ViciousBite()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
			if(_Table != null && _Table.ContainsKey(defender))
				return;

			else if (_Table == null)
				_Table = new Dictionary<Mobile, InternalTimer>();
			
			defender.PlaySound(0x1324);
			defender.SendLocalizedMessage(1234567); // The creature gives you a particular vicious bite.
            Effects.SendLocationParticles(EffectItem.Create(defender.Location, defender.Map, EffectItem.DefaultDuration), 0x37CC, 1, 40, 97, 3, 9917, 0);

            _Table[defender] = new InternalTimer(creature, defender);
		}
		
		private class InternalTimer : Timer
		{
			public BaseCreature Attacker { get; set; }
			public Mobile Defender { get; set; } 
			
			private int _Tick;
			
			public InternalTimer(BaseCreature creature, Mobile defender)
				: base(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20))
			{
                Attacker = creature;
				Defender = defender;
				Start();
			}
			
			protected override void OnTick()
			{
				_Tick++;
				
				AOS.Damage(Defender, Attacker, _Tick * 5, 0, 0, 0, 0, 0, 0, 100);
                Defender.SendLocalizedMessage(1112473); //Your vicious wound is festering!
				
				if(_Tick >= 20 || !Defender.Alive || Defender.IsDeadBondedPet)
				{
					Stop();
					
					if(_Table.ContainsKey(Defender))
						_Table.Remove(Defender);
				}
			}
		}
	}
	
	public class RuneCorruption : SpecialAbility
	{
		public static Dictionary<Mobile, ExpireTimer> _Table;
		
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

		public RuneCorruption()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
			if(_Table == null)
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

			int phy = 0; int  fire = 0; int cold = 0; int poison = 0; int energy = 0;

			if (Core.ML)
			{
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
			}
			else
			{
				if (defender.PhysicalResistance > 0)
				{
					phy = (defender.PhysicalResistance > 70) ? 70 : defender.PhysicalResistance;

					mods.Add(new ResistanceMod(ResistanceType.Physical, -phy));
				}

				if (defender.FireResistance > 0)
				{
					fire = (defender.FireResistance > 70) ? 70 : defender.FireResistance;

					mods.Add(new ResistanceMod(ResistanceType.Fire, -fire));
				}

				if (defender.ColdResistance > 0)
				{
					cold = (defender.ColdResistance > 70) ? 70 : defender.ColdResistance;

					mods.Add(new ResistanceMod(ResistanceType.Cold, -cold));
				}

				if (defender.PoisonResistance > 0)
				{
					poison = (defender.PoisonResistance > 70) ? 70 : defender.PoisonResistance;

					mods.Add(new ResistanceMod(ResistanceType.Poison, -poison));
				}

				if (defender.EnergyResistance > 0)
				{
					energy = (defender.EnergyResistance > 70) ? 70 : defender.EnergyResistance;

					mods.Add(new ResistanceMod(ResistanceType.Energy, -energy));
				}
			}

			for (int i = 0; i < mods.Count; ++i)
				defender.AddResistanceMod(mods[i]);

			defender.FixedEffect(0x37B9, 10, 5);

			timer = new ExpireTimer(defender, mods, TimeSpan.FromSeconds(5.0));
			timer.Start();

			BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.RuneBeetleCorruption, 1153796, 1153823, TimeSpan.FromSeconds(5.0), defender, String.Format("{0}\t{1}\t{2}\t{3}\t{4}", phy, cold, poison, energy, fire)));

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
                m_Mobile.SendMessage("The corruption of your armor has worn off");
                DoExpire();
            }
        }
	}
	
	public class LifeLeech : SpecialAbility
	{
		public override int ManaCost { get { return 5;  } }
		public override TimeSpan CooldownDuration { get { return TimeSpan.FromSeconds(Utility.Random(10, 20)); } }
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

        public static Dictionary<Mobile, InternalTimer> _Table;

		public LifeLeech()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
            if (_Table == null)
                _Table = new Dictionary<Mobile, InternalTimer>();

            InternalTimer t = null;

            if (_Table.ContainsKey(defender))
                t = _Table[defender];

            if (t != null)
                t.Stop();

            t = new InternalTimer(creature, defender);
            _Table[defender] = t;

            defender.SendLocalizedMessage(1070848); // You feel your life force being stolen away!

            t.Start();
		}

        public static void DrainLife(Mobile m, Mobile from)
        {
            if (m.Alive)
            {
                int damageGiven = AOS.Damage(m, from, 5, 0, 0, 0, 0, 100);
                from.Hits += damageGiven;

                m.SendLocalizedMessage(1070847); // The creature continues to steal your life force!
            }
            else
            {
                EndLifeDrain(m);
            }
        }

        public static void EndLifeDrain(Mobile m)
        {
            if (_Table != null && _Table.ContainsKey(m))
            {
                _Table[m].Stop();
                _Table.Remove(m);
                m.SendLocalizedMessage(1070849); // The drain on your life force is gone.
            }
        }

        public class InternalTimer : Timer
        {
            private Mobile m_From;
            private Mobile m_Mobile;
            private int m_Count;

            public InternalTimer(Mobile from, Mobile m)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_From = from;
                m_Mobile = m;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                DrainLife(m_Mobile, m_From);

                if (Running && ++m_Count == 5)
                    EndLifeDrain(m_Mobile);
            }
        }
	}

    public class Webbing : SpecialAbility
    {
        public override int ManaCost { get { return 5; } }
        public override bool TriggerOnDoMeleeDamage { get { return true; } }
        public override bool TriggerOnGotMeleeDamage { get { return true; } }

        public Webbing()
        {
        }

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
        public override bool TriggerOnDoMeleeDamage { get { return true; } }
        public override bool TriggerOnGotMeleeDamage { get { return true; } }
        public override int ManaCost { get { return 0; } }

        private static Dictionary<Mobile, ExpireTimer> _Table;

        public Anemia()
        {
        }

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

                attacker.AddStatMod(new StatMod(StatType.Str, "BloodWorm_Str", str, TimeSpan.FromSeconds(60)));
                attacker.AddStatMod(new StatMod(StatType.Dex, "BloodWorm_Dex", dex, TimeSpan.FromSeconds(60)));
                attacker.AddStatMod(new StatMod(StatType.Int, "BloodWorm_Int", Int, TimeSpan.FromSeconds(60)));

                // -~1_STR~ strength.<br>-~2_INT~ intelligence.<br>-~3_DEX~ dexterity.<br> Drains all stamina.
                BuffInfo.AddBuff(attacker, new BuffInfo(BuffIcon.BloodwormAnemia, 1153797, 1153824, String.Format("{0}\t{1}\t{2}", str, dex, Int)));

                _Table.Add(attacker, timer);
            }
        }

        private class ExpireTimer : Timer
        {
            private DateTime _Expires;
            private Mobile m_Victim;

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
        public override bool TriggerOnDoMeleeDamage { get { return true; } }
        public override bool TriggerOnGotMeleeDamage { get { return true; } }
        public override int ManaCost { get { return 0; } }

        private static Dictionary<Mobile, ExpireTimer> _Table;

        public BloodDisease()
        {
        }

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

                ExpireTimer timer = new ExpireTimer(defender);
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
            private Mobile m_Victim;

            public ExpireTimer(Mobile m)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_Victim = m;
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
                    AOS.Damage(m_Victim, Utility.RandomMinMax(10, 20), 0, 0, 0, 100, 0);
                    m_Victim.Combatant = null;
                }

                m_Count++;
            }
        }
    }

    public class StickySkin : SpecialAbility
	{
		public override int ManaCost { get { return 5;  } }
        public override bool TriggerOnDoMeleeDamage { get { return true; } }
        public override bool TriggerOnGotMeleeDamage { get { return true; } }

        public static List<Mobile> _Table;
		
		public StickySkin()
		{
		}
		
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
		public override int ManaCost { get { return 30;  } }
        public override bool TriggerOnDoMeleeDamage { get { return true; } }
		
		public TailSwipe()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
			if(Utility.RandomBool())
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

    public class Rage : SpecialAbility
    {
        public override bool TriggerOnDoMeleeDamage { get { return true; } }
        public override bool NaturalAbility { get { return true; } }
        public override int ManaCost { get { return 30; } }

        private static Dictionary<Mobile, ExpireTimer> _Table;

        public Rage()
        {
        }

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
            private Mobile m_Mobile;
            private Mobile m_From;
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

    public class Heal : SpecialAbility
    {
        public override bool TriggerOnThink { get { return true; } }
        public override double TriggerChance { get { return 1.0; } }
        public override bool RequiresCombatant { get { return false; } }
        public override int ManaCost { get { return 15; } }
        public override TimeSpan CooldownDuration { get { return TimeSpan.MinValue; } }
        public override bool NaturalAbility { get { return true; } }

        public Heal()
        {
        }

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
}
