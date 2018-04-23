using System;
using Server;

namespace Server.Items
{
	public class SleevesOfThePugilist : LeatherArms
	{
		public override int LabelNumber{ get{ return 1070690; } }

		public override int BasePhysicalResistance{ get{ return 18; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public SleevesOfThePugilist()
		{
			Name = "Sleeves of the Pugilist";
			Hue = 0x6D1;
			SkillBonuses.SetValues( 0, SkillName.Wrestling, 10.0 );
			Attributes.BonusDex = 8;
			Attributes.WeaponDamage = 15;
		}

		public SleevesOfThePugilist( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}