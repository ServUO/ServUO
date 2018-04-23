using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a Colt corpse" )]
	public class Colt : BaseMount
	{
		[Constructable]
		public Colt() : this( "a Colt" )
		{
		}

		[Constructable]
		public Colt( string name ) : base( name, 0xBE, 0x3E9E, AIType.AI_Animal, FightMode. Aggressor, 10, 1, 0.2, 0.4 )
		{
			Hue = 0x4001;

			BaseSoundID = 0xA8;

			SetStr( 90, 400 );
			SetDex( 250, 500 );
			SetInt( 400, 1200 );

			SetHits( 200, 500 );

			SetDamage( 30, 40 );

			SetDamageType( ResistanceType.Physical, 70 );
			SetDamageType( ResistanceType.Fire, 99 );

			SetResistance( ResistanceType.Physical, 60, 80 );
			SetResistance( ResistanceType.Fire, 70, 90 );
			SetResistance( ResistanceType.Cold, 60, 90 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 40, 60 );

			SetSkill( SkillName.MagicResist, 20.0, 120.0 );
			SetSkill( SkillName.Tactics, 20.0, 120.0 );
			SetSkill( SkillName.Wrestling, 20.0, 120.0 );
			SetSkill( SkillName.Anatomy, 30.0, 120.0 );
			SetSkill( SkillName.Magery, 30.0, 120.0 );

			Fame = 2000;
			Karma = 2000;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 110.0;

			PackGold( 300 );
			PackItem( new SulfurousAsh( Utility.RandomMinMax( 100, 250 ) ) );
			PackItem( new Ruby( Utility.RandomMinMax( 10, 30 ) ) );
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Daemon | PackInstinct.Equine; } }

		public Colt( Serial serial ) : base( serial )
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