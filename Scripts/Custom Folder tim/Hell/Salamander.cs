using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a salamander corpse" )]
	public class Salamander : BaseCreature
	{
		[Constructable]
		public Salamander() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a salamander";
			Body = 0xCE;
			Hue = 0x15c;

			SetStr( 15 );
			SetDex( 2000 );
			SetInt( 1000 );

			SetHits( 2000 );
			SetStam( 500 );
			SetMana( 0 );

			SetDamage( 1 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetSkill( SkillName.MagicResist, 200.0 );
			SetSkill( SkillName.Tactics, 5.0 );
			SetSkill( SkillName.Wrestling, 5.0 );

			Fame = 1000;
			Karma = 0;

			VirtualArmor = 4;

			int carrots = Utility.RandomMinMax( 5, 10 );
			PackItem( new Carrot( carrots ) );

			if ( Utility.Random( 5 ) == 0 )
				PackItem( new BrightlyColoredEggs() );

			PackStatue();

			DelayBeginTunnel();
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Rich, 2 );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );	
			
			c.DropItem( new MonsterContractBook() );
		}

		public class SalamanderHole : Item
		{
			public SalamanderHole() : base( 0x913 )
			{
				Movable = false;
				Hue = 1;
				Name = "a mysterious salamander hole";

				Timer.DelayCall( TimeSpan.FromSeconds( 40.0 ), new TimerCallback( Delete ) );
			}

			public SalamanderHole( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize(writer);

				writer.Write( (int) 0 );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				Delete();
			}
		}

		public virtual void DelayBeginTunnel()
		{
			Timer.DelayCall( TimeSpan.FromMinutes( 3.0 ), new TimerCallback( BeginTunnel ) );
		}

		public virtual void BeginTunnel()
		{
			if ( Deleted )
				return;

			new SalamanderHole().MoveToWorld( Location, Map );

			Frozen = true;
			Say( "* The salamander begins to dig a tunnel back to its underground lair *" );
			PlaySound( 0x247 );

			Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( Delete ) );
		}

		public override int Meat{ get{ return 11; } }
		public override bool BardImmune{ get{ return !Core.AOS; } }

		public Salamander( Serial serial ) : base( serial )
		{
		}

		public override int GetAttackSound()
		{
			return 0xC9;
		}

		public override int GetHurtSound()
		{
			return 0xCA;
		}

		public override int GetDeathSound()
		{
			return 0xCB;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			DelayBeginTunnel();
		}
	}
}