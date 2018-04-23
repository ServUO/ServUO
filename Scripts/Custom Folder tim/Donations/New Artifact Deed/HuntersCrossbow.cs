using System;
using Server;

namespace Server.Items
{
	public class HuntersCrossbow : HeavyCrossbow
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public HuntersCrossbow()
		{
			Name = "Hunters Crossbow";
			Hue = 1167;
			Slayer = SlayerName.Repond;
			Attributes.WeaponSpeed = 30;
			Attributes.WeaponDamage = 55;
			Attributes.BonusHits = 10;
		}

		

		public HuntersCrossbow( Serial serial ) : base( serial )
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