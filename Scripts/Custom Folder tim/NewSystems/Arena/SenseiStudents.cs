using System;
using Server.Mobiles;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a Sensei corpse" )]
	public class Sensei : BaseCreature
	{

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.WhirlwindAttack;
		}

		private ArrayList m_students;
		int studentCount = Utility.RandomMinMax( 2, 2 );

		public override bool CanRegenHits{ get{ return true; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RespawnStudents
		{
			get{ return false; }
			set{ if( value ) SpawnStudents(); }
		}

		[Constructable]
		public Sensei() : base( AIType.AI_Melee,FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "Sensei";
			Body = 400;

			SetStr( 491, 610 );
			SetDex( 276, 295 );
			SetInt( 331, 350 );

			SetHits( 1042, 1068 );
			SetMana( 600 );

			SetDamage( 21, 31 );

			SetDamageType( ResistanceType.Physical, 85 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 30, 50 );
			SetResistance( ResistanceType.Cold, 50, 65 );
			SetResistance( ResistanceType.Poison, 25, 45 );
			SetResistance( ResistanceType.Energy, 40, 65 );

			SetSkill( SkillName.MagicResist, 130.6, 145.0 );
			SetSkill( SkillName.Tactics, 130.1, 140.0 );
			SetSkill( SkillName.Wrestling, 130.1, 155.0 );
			SetSkill( SkillName.Swords, 130.1, 155.0 );
			SetSkill( SkillName.Parry, 130.1, 155.0 );
			SetSkill( SkillName.Wrestling, 130.1, 155.0 );
			SetSkill( SkillName.EvalInt, 130.1, 155.0 );
			SetSkill( SkillName.Anatomy, 130.1, 155.0 );
			SetSkill( SkillName.Focus, 130.1, 155.0 );			

			Fame = 40000;
			Karma = -10000;

			VirtualArmor = 40;
			
			AddItem( new Hakama( 1109 ) );
			AddItem( new HakamaShita( 1109 ) );
			AddItem( new SamuraiTabi() );
			AddItem( new Bandana( 1109 ) );

			Katana weapon = new Katana();		
			weapon.Movable = true;
			weapon.LootType = LootType.Blessed;
			weapon.Skill = SkillName.Swords;

			AddItem( weapon );

			m_students = new ArrayList();
			Timer m_timer = new SenseiFamilyTimer( this );
			m_timer.Start();
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 2 );
		}

		public override bool AlwaysMurderer { get { return true; } }

		public override bool OnBeforeDeath()
		{
			this.Say( "Meet Your Next Challenge!!!" );
			ArenaNinja ni = new ArenaNinja();			
			ni.MoveToWorld( new Point3D( 2369, 1127, -90 ), Map.Malas );
			ni.FixedParticles( 0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot );
			ni.PlaySound( 0x22F );
			ni.Team = this.Team;
			ni.Combatant = this.Combatant;

			ArenaEliteNinja ni1 = new ArenaEliteNinja();			
			ni1.MoveToWorld( new Point3D( 2372, 1125, -90 ), Map.Malas );
			ni1.FixedParticles( 0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot );
			ni1.PlaySound( 0x22F );
			ni1.Team = this.Team;
			ni1.Combatant = this.Combatant;

			ArenaEliteNinja ni2 = new ArenaEliteNinja();			
			ni2.MoveToWorld( new Point3D( 2373, 1130, -90 ), Map.Malas );
			ni2.FixedParticles( 0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot );
			ni2.PlaySound( 0x22F );
			ni2.Team = this.Team;
			ni2.Combatant = this.Combatant;

			ArenaEliteNinja ni3 = new ArenaEliteNinja();			
			ni3.MoveToWorld( new Point3D( 2363, 1130, -90 ), Map.Malas );
			ni3.FixedParticles( 0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot );
			ni3.PlaySound( 0x22F );
			ni3.Team = this.Team;
			ni3.Combatant = this.Combatant;

			ArenaEliteNinja ni4 = new ArenaEliteNinja();			
			ni4.MoveToWorld( new Point3D( 2362, 1124, -90 ), Map.Malas );
			ni4.FixedParticles( 0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot );
			ni4.PlaySound( 0x22F );
			ni4.Team = this.Team;
			ni4.Combatant = this.Combatant;
		
			return true;
		}
		
		
		public void SpawnStudents()
		{

			Defrag();
			int family = m_students.Count;

			if( family >= studentCount )
				return;

			//Say( "family {0}, should be {1}", family, studentCount );

			Map map = this.Map;

			if ( map == null )
				return;

			int hr = (int)((this.RangeHome + 1) / 2);

			for ( int i = family; i < studentCount; ++i )
			{
				Student student = new Student();

				bool validLocation = false;
				Point3D loc = this.Location;

				for ( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = X + Utility.Random( 5 ) - 1;
					int y = Y + Utility.Random( 5 ) - 1;
					int z = map.GetAverageZ( x, y );

					if ( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
						loc = new Point3D( x, y, Z );
					else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
						loc = new Point3D( x, y, z );
				}

				student.Teacher = this;
				student.Team = this.Team;
				student.Home = this.Location;
				student.RangeHome = ( hr > 4 ? 4 : hr );
				
				student.MoveToWorld( loc, map );
				m_students.Add( student );
			}
		}

		protected override void OnLocationChange( Point3D oldLocation )
		{

			try
			{
				foreach( Mobile m in m_students )
				{	
					if( m is Student && m.Alive )
					{
						((Student)m).Home = this.Location;
					}
				}
			}
			catch{}

			base.OnLocationChange( oldLocation );
		}
		
		public void Defrag()
		{
			for ( int i = 0; i < m_students.Count; ++i )
			{
				try
				{
					object o = m_students[i];

					Student student = o as Student;

					if ( student == null || !student.Alive )
					{
						m_students.RemoveAt( i );
						--i;
					}

				}
				catch{}
			}
		}

		public override void OnDelete()
		{
			Defrag();

			foreach( Mobile m in m_students )
			{	
				if( m.Alive )
					m.Delete();
			}

			base.OnDelete();
		}

		public Sensei(Serial serial) : base(serial)
		{
			m_students = new ArrayList();
			Timer m_timer = new SenseiFamilyTimer( this );
			m_timer.Start();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
			writer.WriteMobileList( m_students, true );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			m_students = reader.ReadMobileList();
		}
	}

	[CorpseName( "a Samurai Student corpse" )]
	public class Student : BaseCreature
	{

		private Sensei m_master;

		[CommandProperty( AccessLevel.GameMaster )]
		public Sensei Teacher
		{
			get{ return m_master; }
			set{ m_master = value; }
		}

		[Constructable]
		public Student() : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "Student";
			Body = 400;

			SetStr( 391, 510 );
			SetDex( 176, 195 );
			SetInt( 231, 250 );

			SetHits( 942, 968 );
			SetMana( 500 );

			SetDamage( 21, 41 );

			SetDamageType( ResistanceType.Physical, 85 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 20, 40 );
			SetResistance( ResistanceType.Cold, 40, 55 );
			SetResistance( ResistanceType.Poison, 15, 35 );
			SetResistance( ResistanceType.Energy, 30, 55 );

			SetSkill( SkillName.MagicResist, 120.6, 135.0 );
			SetSkill( SkillName.Tactics, 120.1, 130.0 );
			SetSkill( SkillName.Wrestling, 120.1, 145.0 );
			SetSkill( SkillName.Swords, 120.1, 145.0 );
			SetSkill( SkillName.Parry, 120.1, 145.0 );
			SetSkill( SkillName.Wrestling, 120.1, 145.0 );
			SetSkill( SkillName.EvalInt, 120.1, 145.0 );
			SetSkill( SkillName.Anatomy, 120.1, 145.0 );
			SetSkill( SkillName.Focus, 120.1, 145.0 );			

			Fame = 20000;
			Karma = -10000;

			VirtualArmor = 30;
			
			AddItem( new TattsukeHakama( 1109 ) );
			AddItem( new HakamaShita( 1150 ) );
			AddItem( new SamuraiTabi() );
			AddItem( new Bandana( 1109 ) );

			Katana weapon = new Katana();		
			weapon.Movable = true;
			weapon.LootType = LootType.Blessed;
			AddItem( weapon );

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 1 );
		}
		public override bool AlwaysMurderer { get { return true; } }

		public override void OnCombatantChange()
		{
			if( Combatant != null && Combatant.Alive && m_master != null && m_master.Combatant == null )
				m_master.Combatant = Combatant;
		}

		public Student(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
			writer.Write( m_master );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			m_master = (Sensei)reader.ReadMobile();
		}
	}

	public class SenseiFamilyTimer : Timer
	{
		private Sensei m_from;

		public SenseiFamilyTimer( Sensei from  ) : base( TimeSpan.FromMinutes( 1 ), TimeSpan.FromMinutes( 5 ) )
		{
			Priority = TimerPriority.OneMinute; 
			m_from = from;
		}

		protected override void OnTick()
		{
			if ( m_from != null && m_from.Alive )
				m_from.SpawnStudents();
			else
				Stop();
		}
	}
}