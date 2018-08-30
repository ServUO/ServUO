using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a battle lama corpse" )]
	public class BattleLlama : BaseMount
	{
        public override bool InitialInnocent { get { return true; } }
        public override bool DeleteOnRelease { get { return true; } }
		[Constructable]
		public BattleLlama() : this( "A Battle Llama" )
		{
		}

		[Constructable]
		public BattleLlama( string name ) : base( name, 0xDC, 0x3EA6, AIType.AI_Animal, FightMode.Good, 10, 1, 0.2, 0.4 )
		{
			BaseSoundID = 0x3F3;

			SetStr( 1666 );
			SetDex( 780 );
			SetInt( 189 );

			SetHits( 1287 );
			SetMana( 255 );
            SetStam( 604 );

			SetDamage( 22, 30 );

			SetDamageType( ResistanceType.Physical, 50 );
            SetDamageType(ResistanceType.Cold, 25);
            SetDamageType(ResistanceType.Fire, 25);

			SetResistance( ResistanceType.Physical, 75 );
			SetResistance( ResistanceType.Fire, 75 );
			SetResistance( ResistanceType.Cold, 75 );
			SetResistance( ResistanceType.Poison, 75 );
			SetResistance( ResistanceType.Energy, 75 );

			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Wrestling, 120.0 );

			Fame = 0;
			Karma = 1000;

			Tamable = true;
			ControlSlots = 4;
			MinTameSkill = 0;
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 12; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public BattleLlama( Serial serial ) : base( serial )
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