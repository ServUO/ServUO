using System;
using Server;

namespace Server.Items
{
	public class BowOfKings : CompositeBow
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public BowOfKings()
		{
			Name = "Bow Of Kings";
			Hue = 1177;
			Attributes.RegenStam = 10;
			Attributes.WeaponSpeed = 15;
			Attributes.WeaponDamage = 55;
			Attributes.BonusHits = 5;
		}

		

		public BowOfKings( Serial serial ) : base( serial )
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