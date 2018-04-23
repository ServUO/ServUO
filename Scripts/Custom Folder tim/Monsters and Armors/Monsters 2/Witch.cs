using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Fifth;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
	[CorpseName( "a witch's corpse" )] 
	public class Witch : BaseCreature 
	{ 
		[Constructable] 
		public Witch() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
		{ 
			Name = NameList.RandomName( "female" );
			Title = "the witch";
			Body = 0x191;
			Hue = 0x83EC;

			SetStr( 81, 105 );
			SetDex( 191, 215 );
			SetInt( 126, 150 );

			SetHits( 120, 140 );

			SetDamage( 10, 14 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 80.2, 100.0 );
			SetSkill( SkillName.Magery, 95.1, 100.0 );
			SetSkill( SkillName.Meditation, 27.5, 50.0 );
			SetSkill( SkillName.MagicResist, 77.5, 100.0 );
			SetSkill( SkillName.Tactics, 65.0, 87.5 );
			SetSkill( SkillName.Wrestling, 20.3, 80.0 );

			Fame = 10500;
			Karma = -10500;

			VirtualArmor = 16;
			PackReg( 23 );
			if ( Utility.RandomBool() )
				PackItem( new Shoes() );
			else
				PackItem( new Sandals() );

			AddItem( new Robe( Utility.RandomMetalHue() ) );
			AddItem( new WizardsHat( Utility.RandomMetalHue() ) ); 
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override int Meat{ get{ return 1; } }
		public override int TreasureMapLevel{ get{ return Core.AOS ? 2 : 0; } }

		public void Polymorph( Mobile m )
		{
			if ( !m.CanBeginAction( typeof( PolymorphSpell ) ) || !m.CanBeginAction( typeof( IncognitoSpell ) ) || m.IsBodyMod )
				return;

			IMount mount = m.Mount;

			if ( mount != null )
				mount.Rider = null;

			if ( m.Mounted )
				return;

			if ( m.BeginAction( typeof( PolymorphSpell ) ) )
			{
				Item disarm = m.FindItemOnLayer( Layer.OneHanded );

				if ( disarm != null && disarm.Movable )
					m.AddToBackpack( disarm );

				disarm = m.FindItemOnLayer( Layer.TwoHanded );

				if ( disarm != null && disarm.Movable )
					m.AddToBackpack( disarm );

				m.BodyMod = 81;
				m.HueMod = 0;

				new ExpirePolymorphTimer( m ).Start();
			}
		}

		private class ExpirePolymorphTimer : Timer
		{
			private Mobile m_Owner;

			public ExpirePolymorphTimer( Mobile owner ) : base( TimeSpan.FromMinutes( 3.0 ) )
			{
				m_Owner = owner;

				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				if ( !m_Owner.CanBeginAction( typeof( PolymorphSpell ) ) )
				{
					m_Owner.BodyMod = 0;
					m_Owner.HueMod = -1;
					m_Owner.EndAction( typeof( PolymorphSpell ) );
				}
			}
		}

		public void DoSpecialAbility( Mobile target )
		{
			if ( target == null || target.Deleted ) //sanity
				return;
			if ( 0.6 >= Utility.RandomDouble() ) // 60% chance to polymorph attacker into a ratman
				Polymorph( target );

			if ( Hits < 500 && !IsBodyMod ) // Baracoon is low on life, polymorph into a ratman
				Polymorph( this );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			DoSpecialAbility( attacker );
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			DoSpecialAbility( defender );
		}

		public Witch( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}