using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a American Saddlebred Horse corpse" )]
	public class AmericanSaddlebredHorse : BaseMount
	{
		[Constructable]
		public AmericanSaddlebredHorse() : this( "a American Saddlebred Horse" )
		{
		}

		[Constructable]
		public AmericanSaddlebredHorse( string name ) : base( name, 0xBE, 0x3E9E, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Hue = 918;

			BaseSoundID = 0xA8;

			SetStr( 60, 120 );
			SetDex( 100, 120 );
			SetInt( 80, 90 );

			SetHits( 120, 600 );

			SetDamage( 20, 30 );

			SetDamageType( ResistanceType.Physical, 30 );
			SetDamageType( ResistanceType.Fire, 59 );

			SetResistance( ResistanceType.Physical, 30, 55 );
			SetResistance( ResistanceType.Fire, 50, 55 );
			SetResistance( ResistanceType.Cold, 30, 50 );
			SetResistance( ResistanceType.Poison, 20, 40 );
			SetResistance( ResistanceType.Energy, 40, 60 );

			SetSkill( SkillName.MagicResist, 20.0, 120.0 );
			SetSkill( SkillName.Tactics, 100.0, 120.0 );
			SetSkill( SkillName.Wrestling, 100.0, 130.0 );
			SetSkill( SkillName.Anatomy, 30.0, 120.0 );
			SetSkill( SkillName.Magery, 30.0, 100.0 );

			Fame = 2000;
			Karma = 2000;

			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 120.0;

			PackGold( 300 );
			PackItem( new SulfurousAsh( Utility.RandomMinMax( 100, 250 ) ) );
			PackItem( new Ruby( Utility.RandomMinMax( 10, 30 ) ) );
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Daemon | PackInstinct.Equine; } }

		public AmericanSaddlebredHorse( Serial serial ) : base( serial )
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

			if ( BaseSoundID <= 0 )
				BaseSoundID = 0xA8;
		}
	}
}