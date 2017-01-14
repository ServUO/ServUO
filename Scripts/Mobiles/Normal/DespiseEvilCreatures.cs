using Server;
using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Spells;

namespace Server.Engines.Despise
{
	public class Phantom : DespiseCreature
	{
		private DateTime m_NextDiscord;
	
		[Constructable]
		public Phantom() : this(1)
		{
		}
	
		[Constructable]
		public Phantom(int powerLevel) : base(AIType.AI_Melee, FightMode.Good)
		{
			Name = "Phantom";
			Body =  310;
			BaseSoundID = 0x482;

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 20, 30 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, SkillStart );
			SetSkill( SkillName.Tactics, SkillStart );
			SetSkill( SkillName.Wrestling, SkillStart );
			SetSkill( SkillName.Anatomy, SkillStart );
			SetSkill( SkillName.Musicianship, 50, 75 );
			SetSkill( SkillName.Discordance, 50, 75 );

			Fame = GetFame;
			Karma = GetKarmaEvil;
			
			m_NextDiscord = DateTime.Now;
            Power = powerLevel;
		}

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
		//public override bool AlwaysMurderer{ get{ return true; } }		
		public override int StrStart { get { return Utility.RandomMinMax(65, 75); } } 
		public override int DexStart { get { return Utility.RandomMinMax(100, 110); } } 
		public override int IntStart { get { return Utility.RandomMinMax(100, 110); } } 

        public override Mobile GetBardTarget(bool creaturesOnly = false)
        {
            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, RangePerception);

            Mobile closest = null;
            int range = 0;

            foreach (Mobile m in eable)
            {
                if (CanDoTarget(m))
                {
                    int dist = (int)GetDistanceToSqrt(m);

                    if (closest == null || dist < range)
                    {
                        closest = m;
                        range = dist;
                    }
                }
            }
            eable.Free();

            return closest;
        }

        private bool CanDoTarget(Mobile m)
        {
            int discordanceEffect = 0;

            if (!CanBeHarmful(m, false) || Server.SkillHandlers.Discordance.GetEffect(m, ref discordanceEffect))
                return false;

            if ((m is DespiseCreature && ((DespiseCreature)m).Alignment != Alignment.Neutral && ((DespiseCreature)m).Alignment != this.Alignment) || m is DespiseBoss)
                return true;

            return m is PlayerMobile && !this.Controlled && ((m.Karma < 0 && this.Alignment == Alignment.Good) || (m.Karma > 0 && this.Alignment == Alignment.Evil));
        }

		public Phantom(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
			
			m_NextDiscord = DateTime.Now;
		}
	}
	
	public class Naba : DespiseCreature
	{
		[Constructable]
		public Naba() : this(1)
		{
		}
		
		[Constructable]
		public Naba(int powerLevel) : base(AIType.AI_Mage, FightMode.Good)
		{
			Name = "Naba";
			Body = 85;
			BaseSoundID = 639;

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 15 );
			SetDamageType( ResistanceType.Cold, 10 );
			SetDamageType( ResistanceType.Poison, 15 );
			SetDamageType( ResistanceType.Energy, 10 );

			SetResistance( ResistanceType.Physical, 30, 40 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 25, 30 );
			SetResistance( ResistanceType.Poison, 30, 35 );
			SetResistance( ResistanceType.Energy, 20, 25 );

			SetSkill( SkillName.MagicResist, SkillStart );
			SetSkill( SkillName.Tactics, SkillStart );
			SetSkill( SkillName.Wrestling, SkillStart );
			SetSkill( SkillName.Anatomy, SkillStart );
			SetSkill( SkillName.Magery, SkillStart );
			SetSkill( SkillName.EvalInt, SkillStart );
			SetSkill( SkillName.Meditation, SkillStart );
			
			Fame = GetFame;
			Karma = GetKarmaEvil;
            Power = powerLevel;
		}

        protected override BaseAI ForcedAI { get { return new DespiseMageAI(this); } }
		//public override bool AlwaysMurderer{ get{ return true; } }		
		public override int StrStart { get { return Utility.RandomMinMax(65, 80); } } 
		public override int DexStart { get { return Utility.RandomMinMax(70, 80); } } 
		public override int IntStart { get { return Utility.RandomMinMax(110, 150); } } 
		
		public Naba(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class Darkmane : DespiseCreature
	{
		[Constructable]
		public Darkmane() : this(1)
		{
		}
		
		[Constructable]
		public Darkmane(int powerLevel) : base(AIType.AI_Melee, FightMode.Good)
		{
			Name = "Darkmane";
			BaseSoundID = 0xA8;
			
			switch(Utility.Random(3))
			{
				case 0: Body = 116; break;
				case 1: Body = 178; break;
				case 2: Body = 179; break;
			}

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 30 );
			SetResistance( ResistanceType.Fire, 25 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 30 );
			SetResistance( ResistanceType.Energy, 30 );

			SetSkill( SkillName.MagicResist, SkillStart );
			SetSkill( SkillName.Tactics, SkillStart );
			SetSkill( SkillName.Wrestling, SkillStart );
			SetSkill( SkillName.Anatomy, SkillStart );

			Fame = GetFame;
			Karma = GetKarmaEvil;
            Power = powerLevel;
		}

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
		//public override bool AlwaysMurderer{ get{ return true; } }		
		public override int StrStart { get { return Utility.RandomMinMax(80, 100); } } 
		public override int DexStart { get { return Utility.RandomMinMax(110, 115); } } 
		public override int IntStart { get { return Utility.RandomMinMax(100, 115); } } 
		
		public override bool RaiseDamage { get { return true; } }
		public override int MinDamMax { get { return 15; } }
		public override int MaxDamMax { get { return 26; } }
		
		public override double WeaponAbilityChance { get { return 0.5; } }

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.ArmorIgnore;
		}
		
		public Darkmane(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class Skeletrex : DespiseCreature
	{
		[Constructable]
		public Skeletrex() : this(1)
		{
		}
		
		[Constructable]
		public Skeletrex(int powerLevel) : base(AIType.AI_Melee, FightMode.Good)
		{
			Name = "Skeletrex";
			Body = 147;
			BaseSoundID = 451;

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 25 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 15, 25 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, SkillStart );
			SetSkill( SkillName.Tactics, SkillStart );
			SetSkill( SkillName.Wrestling, SkillStart );
			SetSkill( SkillName.Anatomy, SkillStart );
			SetSkill( SkillName.Archery, SkillStart );

			Fame = GetFame;
			Karma = GetKarmaEvil;
			
			AddItem( new Bow() );
			PackItem( new Arrow( Utility.RandomMinMax( 5, 10 ) ) ); // OSI it is different: in a sub backpack, this is probably just a limitation of their engine
            Power = powerLevel;
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
		//public override bool AlwaysMurderer{ get{ return true; } }		
		public override int StrStart { get { return Utility.RandomMinMax(40, 55); } } 
		public override int DexStart { get { return Utility.RandomMinMax(160, 180); } } 
		public override int IntStart { get { return Utility.RandomMinMax(110, 120); } }
				
		public override int MinDamMax { get { return 15; } }
		public override int MaxDamMax { get { return 26; } }
		public override bool RaiseDamage { get { return true; } }
		
		public Skeletrex(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class Hellion : DespiseCreature
	{
		[Constructable]
		public Hellion() : this(1)
		{
		}
		
		[Constructable]
		public Hellion(int powerLevel) : base(AIType.AI_Melee, FightMode.Good)
		{
			Name = "Hellion";
			Body = 130;
			BaseSoundID = 0x174;

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 30 );
			SetResistance( ResistanceType.Fire, 25 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 30 );
			SetResistance( ResistanceType.Energy, 30 );

			SetSkill( SkillName.MagicResist, SkillStart );
			SetSkill( SkillName.Tactics, SkillStart );
			SetSkill( SkillName.Wrestling, SkillStart );
			SetSkill( SkillName.Anatomy, SkillStart );

			Fame = GetFame;
			Karma = GetKarmaEvil;
            Power = powerLevel;
		}

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
		//public override bool AlwaysMurderer{ get{ return true; } }		
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Bear; } }
		public override int MinDamMax { get { return 15; } }
		public override int MaxDamMax { get { return 26; } }
		public override bool RaiseDamage { get { return true; } }
		
		public override int StrStart { get { return Utility.RandomMinMax(150, 175); } } 
		public override int DexStart { get { return Utility.RandomMinMax(90, 105); } } 
		public override int IntStart { get { return Utility.RandomMinMax(30, 40); } } 
		
		public override double WeaponAbilityChance { get { return 0.5; } }

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.CrushingBlow;
		}

		public Hellion(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class Echidnite : DespiseCreature
	{
		[Constructable]
		public Echidnite() : this(1)
		{
		}
		
		[Constructable]
		public Echidnite(int powerLevel) : base(AIType.AI_Melee, FightMode.Good)
		{
			Name = "Echidnite";
			Body = 250;
			BaseSoundID = 0x52A;

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, SkillStart );
			SetSkill( SkillName.Tactics, SkillStart );
			SetSkill( SkillName.Wrestling, SkillStart );
			SetSkill( SkillName.Anatomy, SkillStart );

			Fame = GetFame;
			Karma = GetKarmaEvil;
            Power = powerLevel;
		}

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
		//public override bool AlwaysMurderer{ get{ return true; } }		

		public override int MinDamMax { get { return 15; } }
		public override int MaxDamMax { get { return 26; } }
		public override bool RaiseDamage { get { return true; } }
		
		public override int StrStart { get { return Utility.RandomMinMax(150, 175); } } 
		public override int DexStart { get { return Utility.RandomMinMax(120, 130); } } 
		public override int IntStart { get { return Utility.RandomMinMax(50, 60); } } 
		
		public override double WeaponAbilityChance { get { return 0.5; } }

        public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.ConcussionBlow;
		}
		
		public Echidnite(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class BerlingBlades : DespiseCreature
	{	
		[Constructable]
		public BerlingBlades() : this(1)
		{
		}
		
		[Constructable]
		public BerlingBlades(int powerLevel) : base(AIType.AI_Melee, FightMode.Good)
		{
			Name = "Berling Blades";
			Body = 574;
			BaseSoundID = 224;

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, SkillStart );
			SetSkill( SkillName.Tactics, SkillStart );
			SetSkill( SkillName.Wrestling, SkillStart );
			SetSkill( SkillName.Anatomy, SkillStart );

			Fame = GetFame;
			Karma = GetKarmaEvil;
            Power = powerLevel;
		}

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
		//public override bool AlwaysMurderer{ get{ return true; } }		
		
		public override int MinDamMax { get { return 15; } }
		public override int MaxDamMax { get { return 26; } }
		public override bool RaiseDamage { get { return true; } }
		
		public override int StrStart { get { return Utility.RandomMinMax(100, 120); } } 
		public override int DexStart { get { return Utility.RandomMinMax(140, 155); } } 
		public override int IntStart { get { return Utility.RandomMinMax(30, 50); } } 
		
		public override double WeaponAbilityChance { get { return 0.5; } }

        public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.DoubleStrike;
		}
		
		public override int GetAngerSound()
		{
			return 0x23A;
		}

		public override int GetAttackSound()
		{
			return 0x3B8;
		}

		public override int GetHurtSound()
		{
			return 0x23A;
		}
		
		public BerlingBlades(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class Prometheoid : DespiseCreature
	{
		private DateTime m_NextHeal;
		private readonly double HealThreshold = 0.5;
		
		public virtual int MinHeal { get { return Math.Max(10, Power * 3); } }
		public virtual int MaxHeal { get { return Math.Max(25, Power * 5); } }
	
		[Constructable]
		public Prometheoid() : this(1)
		{
		}
		
		[Constructable]
		public Prometheoid(int powerLevel) : base(AIType.AI_Melee, FightMode.Good)
		{
			Name = "Prometheoid";
			Body = 305;
			BaseSoundID = 224;

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 10, 20 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, SkillStart );
			SetSkill( SkillName.Tactics, SkillStart );
			SetSkill( SkillName.Wrestling, SkillStart );
			SetSkill( SkillName.Anatomy, SkillStart );

			Fame = GetFame;
			Karma = GetKarmaEvil;
			
			m_NextHeal = DateTime.Now;
            Power = powerLevel;
		}

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
		//public override bool AlwaysMurderer{ get{ return true; } }		
		
		public override int StrStart { get { return Utility.RandomMinMax(85, 100); } } 
		public override int DexStart { get { return Utility.RandomMinMax(110, 125); } } 
		public override int IntStart { get { return Utility.RandomMinMax(130, 150); } }
		
		public override void OnThink()
		{
			base.OnThink();
			
			if(m_NextHeal < DateTime.Now && this.Map != null && this.Map != Map.Internal)
			{
				List<Mobile> eligables = new List<Mobile>();
				IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 8);
				
				foreach(Mobile m in eable)
				{
					if(m.Alive && m.Hits <= (int)((double)m.HitsMax * HealThreshold) && CanDoHeal(m))
						eligables.Add(m);
				}
				
				if(eligables.Count > 0)
				{
					Mobile m = eligables[Utility.Random(eligables.Count)];
					
					Direction = GetDirectionTo( m );
					
					SpellHelper.Heal( Utility.RandomMinMax(MinHeal, MaxHeal), m, this );
					m.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
					m.PlaySound( 0x202 );

                    int nextHeal = Utility.RandomMinMax(20 - Power, 30 - Power);
					m_NextHeal = DateTime.Now + TimeSpan.FromSeconds(nextHeal);
					return;
				}
				
				m_NextHeal = DateTime.Now + TimeSpan.FromSeconds(5);
			}
		}

        private bool CanDoHeal(Mobile toHeal)
        {
            if (toHeal is DespiseCreature && ((DespiseCreature)toHeal).Alignment == Alignment)
                return true;

            return toHeal is PlayerMobile && ((toHeal.Karma < 0 && Alignment == Alignment.Evil) || (toHeal.Karma > 0 && Alignment == Alignment.Good));
        }
		
		public Prometheoid(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
			
			m_NextHeal = DateTime.Now;
		}
	}
}