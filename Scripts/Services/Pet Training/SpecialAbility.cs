using System;
using Server;
using System.Collections.Generic;

namespace Server.Mobiles
{
	public abstract class SpecialAbility
	{
		public virtual int ManaCost { get { return 10;  } }
		public virtual int MaxRange { get { return 1; } }
		public virtual double TriggerChance { get { return 1.0; } }
		public virtual TimeSpan CooldownDuration { get { return TimeSpan.FromSeconds(30); } }
		
		public virtual bool TriggerOnGotMeleeDamage { get { return false; } }
		public virtual bool TriggerOnDoMeleeDamage { get { return false; } }
		public virtual bool TriggerOnGotSpellDamage { get { return false; } }
		public virtual bool TriggerOnDoSpellDamage { get { return false; } }
		public virtual bool TriggerOnThink { get { return false; } }

        public abstract void DoEffects(BaseCreature creature, Mobile defender, ref int damage);
		
		public SpecialAbility()
		{
		}

        public static void CheckCombatTrigger(Mobile attacker, Mobile defedner, ref int damage, DamageType type)
        {
            if (attacker is BaseCreature && !((BaseCreature)attacker).Summoned)
            {
                var bc = attacker as BaseCreature;
                var profile = PetTrainingHelper.GetProfile(bc);

                if (profile != null)
                {
                    SpecialAbility ability = null;

                    var abilties = profile.GetSpecialAbility().Where(m =>
                        (type == DamageType.Melee && m.TriggerOnDoMeleeDamage) || (type >= DamageType.Spell && m.TriggerOnDoSpellDamage) &&
                        !m.IsInCooldown(attacker));

                    if (abilties != null && abilties.Length > 0)
                    {
                        abilty = moves[Utility.Random(abilties.Length)];
                    }

                    if (abilty != null)
                    {
                        abilty.Trigger(bc, defender, ref damage);
                    }
                }
            }

            if (defender is BaseCreature && !((BaseCreature)defender).Summoned)
            {
                var bc = defender as BaseCreature;
                var profile = PetTrainingHelper.GetProfile(bc);

                if (profile != null)
                {
                    SpecialAbility ability = null;

                    var abilties = profile.GetSpecialAbilities().Where(m =>
                        (type == DamageType.Melee && m.TriggerOnGotMeleeDamage) || (type >= DamageType.Spell && m.TriggerOnGotSpellDamage) &&
                        !m.IsInCooldown(attacker));

                    if (abilties != null && abilties.Length > 0)
                    {
                        ability = abilties[Utility.Random(abilties.Length)];
                    }

                    if (ability != null)
                    {
                        move.Trigger(bc, attacker, ref 0);
                    }
                }
            }
        }
		
		public static void CheckThinkTrigger(BaseCreature bc)
		{
			var combatant = bc.Combatant;
			
			if(combatant is Mobile)
			{
                var profile = PetTrainingHelper.GetProfile(bc);

                if (profile != null)
                {
                    SpecialAbility ability = null;

                    var abilties = profile.GetSpecialAbilities().Where(m => m.TriggerOnThink && !m.IsInCooldown(attacker));

                    if (abilties != null && abilties.Length > 0)
                    {
                        ability = abilties[Utility.Random(abilties.Length)];
                    }

                    if (ability != null)
                    {
                        move.Trigger(bc, (Mobile)combatant, ref damage, type);
                    }
                }
			}
		}
		
		public virtual void Trigger(BaseCreature creature, Mobile defender, ref int damage)
		{
			if(CheckMana(attacker) && Validate(attacker, defender) && TriggerChance >= Utility.RandomDouble())
			{
				DoEffects(creature, defender, ref damage);
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
		
		public static SpecialAbility AngryFire = _Abilities[0];
		public static SpecialAbility ConductiveBlast = _Abilities[1];
		public static SpecialAbility DragonBreath = _Abilities[2];
		public static SpecialAbility GraspingClaw = _Abilities[3];
		public static SpecialAbility Inferno = _Abilities[4];
		public static SpecialAbility LightningForce = _Abilities[5];
		public static SpecialAbility ManaDrain = _Abilities[6];
		public static SpecialAbility RagingBreath = _Abilities[7];
		public static SpecialAbility Repel = _Abilities[8];
		public static SpecialAbility SearingWounds = _Abilities[9];
		public static SpecialAbility StealLife = _Abilities[10];
		public static SpecialAbility VenomousBite = _Abilities[11];
		public static SpecialAbility ViciousBite = _Abilities[12];
		public static SpecialAbility RuneCorruption = _Abilities[13];
		public static SpecialAbility LifeLeech = _Abilities[14];
		public static SpecialAbility StickySkin = _Abilities[15];
		public static SpecialAbility TailSwipe = _Abilities[16];
        public static SpecialAbility FlurryForce = _Abilities[17];
		
        public static SpecialAbiliti[] Abilities { get { return _Abilities; } }
		private static SpecialAbility[] _Abilities = new SpecialAbility[17];
		
		public static void Initialize()
		{
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
		}
    }
	
	public class AngryFire : SpecialAbility
	{
		public AngryFire()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, int damage)
		{
            int damage = defender.Hits / 2;

            AOS.Damage(defender, creature, damage, 60, 20, 0, 0, 20);

            defender.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
            defender.PlaySound(0x208);

            defender.SendLocalizedMessage(1070823); // The creature hits you with its Angry Fire.    
		}
	}
	
	public class ConductiveBlast : SpecialAbility
	{
        public override bool TriggerOnDoMeleeDamage { get { return false; } }

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

                if(_Table.Count == 0)
                    _Table = null;
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
                ConductiveBlast.RemoveFromTable(m_Mobile);
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
        public override bool TriggerOnDoMeleeDamage { get { return false; } }

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

                if(_Table.Count == 0)
                    _Table = null;
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
                ConductiveBlast.RemoveFromTable(m_Mobile);
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
		public override TimeSpan CooldownDuration { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(30, 45)); } }
        public override int MaxRange { get { return 12; } }

		public DragonBreath()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
			attacker.BreathStart(defender);
		}
	}
	
	public class GraspingClaw : SpecialAbility
	{
        public override bool TriggerOnDoMeleeDamage { get { return true; } }
        public override double TriggerChance { get { return 0.1; } }

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

            defender.FixedEffect(0x37B9, 10, 5);
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

                if (_Table.Count == 0)
                    _Table = null;
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

                GraspingClaw.Expire(m_Mobile);
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
		public Inferno()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
		}
	}
	
	public class LightningForce : SpecialAbility
	{
		public LightningForce()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
		}
	}
	
	public class ManaDrain : SpecialAbility
	{
        public override bool TriggerOnGotMeleeDamage { get { return true; } }
        public override bool TriggerOnDoMeleeDamage { get { return true; } }
        public override double TriggerChance { get { return 0.25; } }

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
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != Team))
                    list.Add(m);
                else if (m.Player)
                    list.Add(m);
            }

            eable.Free();

            foreach (Mobile m in list)
            {
                DoHarmful(m);

                m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                m.PlaySound(0x231);

                m.SendMessage("You feel the mana drain out of you!");

                int toDrain = Utility.RandomMinMax(40, 60);

                Mana += toDrain;
                m.Mana -= toDrain;
            }
		}
	}
	
	public class RagingBreath : SpecialAbility
	{
        public static Dictionary<Mobile, InternalTimer> _Table;

		public RagingBreath()
		{
		}

        public override void DoEffects(BaseCreature creature, Mobile defender, int damage)
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

            attacker.PlaySound(0x227);

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

                if (_Table.Count == 0)
                    _Table = null;
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
                    RagingBreath.EndFire(m_Defender);
                }
                else
                {
                    // TODO: Effects?
                    AOS.Damage(Defener, Attacker, Utility.RandomMinMax(20, 30), 0, 100, 0, 0, 0);
                    Defender.PlaySound(0x1DD);
                }
            }
        }
	}
	
	public class Repel : SpecialAbility
	{
        public override bool TriggerOnGotMeleeDamage { get { return false; } }
        public override bool TriggerOnGotSpellDamage { get { return false; } }
        public override double TriggerChance { get { return 0.25; } }

		public Repel()
		{
		}
		
		// this will need another function hooked from AOS.Damage to repel damage.
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
            // TODO: Effects?
            defender.SendLocalizedMessage(1070844); // The creature repels the attack back at you.

            AOS.Damage(defener, creature, damage, 0, 0, 0, 0, 0, 0, 100);

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
			if(_Table != null && Table.ContainsKey(defender))
			{
				return;
			}
			
			if(_Table == null)
				_Table = new Dictionary<Mobile, InternalTimer>();
			
			_Table[defender] = new InternalTimer(defender);
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
				Defender = defener;
			}
			
			protected override void OnTick()
			{
				if(_Table != null && _Table.ContainsKey(Defender))
				{
					_Table.Remove(Defener);
					
					if(_Table.Count == null)
						_Table = null;
				}
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
            //TODO: Effects
			AOS.Damage(defender, attacker, damage, 0, 0, 0, 0, 100);
			
			new InternalTimer(attacker, defender, damage);
		}
		
		private class InternalTimer : Timer
		{
			public BaseCreature creature { get; set; }
			public Mobile Defender { get; set; }
			
			public InternalTimer(BaseCreature creature, Mobile defender, int toHeal)
				: base(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), 5)
			{
				Start();
			}
			
			public override void OnTick()
			{
				Attacker.Hits = Math.Min(Attacker.HitsMax, Attacker.Hits + toHeal / 5);
			}
		}
	}
	
	public class VenomousBite : SpecialAbility
	{
		public override int ManaCost { get { return 50;  } }
		public override double TriggerChance { get { return 0.1; } }
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

		public VenomousBite()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
			//TODO: Effects/Sounds?
			IPooledEnumerable eable = attacker.GetMobilesInRange(3);
			List<Mobile> list = new List<Mobile>();
			
			list.Add(defender);
			
			foreach(Mobile m in eable)
			{
				if(attacker.CanBeHarmful(m) && SpellHelper.ValidIndirectTarget(attacker, m) && attacker.InLOS(m))
				{
					list.Add(m);
				}
			}
			
			eable.Free();
			double total = Caster.Skills[SkillName.Poisoning].Value * 2;                        
			
			int level = 1;                       
			double dist = 0;

			foreach(var m in list)
			{                       
				dist = Caster.GetDistanceToSqrt(m);

				if (dist >= 3.0)
					total -= (dist - 3.0) * 10.0;

				if (total >= 200.0 && 1 > Utility.Random(10))
					level = 3;
				else if (total > (Core.AOS ? 170.1 : 170.0))
					level = 2;
				else if (total > (Core.AOS ? 130.1 : 130.0))
					level = 1;
				else
					level = 0; 
				
				m.ApplyPoison(attacker, Poison.GetPoison(level));
			}
		}
	}
	
	public class ViciousBite : SpecialAbility
	{
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

		public ViciousBite()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
			// Lightning Strike Sound, blue vortex
			// The creature gives you a particular vicious bite.
			// Your wounds are festering. every tick
			if(_Table != null && _Table.ContainsKey(defender))
				return;
			else if (_Table == null)
				_Table = new Dictionary<Mobile, InternalTimer>();
			
			defender.PlaySound(0x1324);
			defender.SendLocalizedMessage(1234567); // The creature gives you a particular vicious bite.
			
			_Table[defender] = new InternalTimer(attacker, defender);
		}
		
		private class InternalTimer : Timer
		{
			public BaseCreature creature { get; set; }
			public Mobile Defender { get; set; } 
			
			private int _Tick;
			
			public InternalTimer(BaseCreature creature, Mobile defender)
				: base(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20))
			{
				Attacker = attacker;
				Defender = defender;
				Start();
			}
			
			public override void OnTick()
			{
				_Tick++;
				
				AOS.Damage(Defender, Attacker, _Tick * 5, 0, 0, 0, 0, 0, 0, 100);
				Attacker.PrivateOverheadMessage(MessageType.Regular, 123416549, 0x1234);//Your wounds are festering.
				
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
		public Dictionary<Mobile, ExpireTimer> _Table;
		
        public virtual bool TriggerOnDoMeleeDamage { get { return false; } }

		public RuneCorruption()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
			if(_Table == null)
				_Table = new Dictionary<Mobile, ExpireTimer>();
			
			ExpireTimer timer;
			
			if(_Table.ContainsKey(attacker))
				timer = _Table[attacker];

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
		
		private class ExpireTimer : Timer
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
		public override int ManaCost { get { return 15;  } }
		public override double TriggerChance { get { return 0.5; } }
		public override TimeSpan CooldownDuration { get { return TimeSpan.FromSeconds(Utility.Random(10, 20)); } }
        public override bool TriggerOnDoMeleeDamage { get { return true; } }

        public static Dictionary<Mobile, InternalTimer> _Table;

		public LifeLeech()
		{
		}
		
		public override void DoEffects(BaseCreature creature, Mobile defender, ref int damage)
		{
            if (_Table != null)
                _Table = new Dictionary<Mobile, InternalTimer>();

            Timer t = null;

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

                defender.SendLocalizedMessage(1070847); // The creature continues to steal your life force!
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

                if (_Table.Count == 0)
                    _Table = null;
            }
        }

        private class InternalTimer : Timer
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

                if (Running && ++thism_Count == 5)
                    EndLifeDrain(m_Mobile);
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

                if (_Table.Count == 0)
                    _Table = null;
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

				// TODO: Effects/Sounds?
				
				target.AddStatMod(new StatMod(StatType.Dex, "[TailSwipe] Dex", -20, TimeSpan.FromSeconds(5)));
				target.AddStatMod(new StatMod(StatType.Int, "[TailSwipe] Int", -20, TimeSpan.FromSeconds(5)));
			}
		}
	}
}