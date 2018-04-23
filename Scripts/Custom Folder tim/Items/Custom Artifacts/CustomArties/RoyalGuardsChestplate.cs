using System;
using Server;

namespace Server.Items
{
	public class RoyalGuardsChestplate : PlateChest, ITokunoDyable
	{
		public override int ArtifactRarity{ get{ return 13; } }

		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 15; } }
		public override int BaseColdResistance{ get{ return 10; } }
		public override int BasePoisonResistance{ get{ return 15; } }
		public override int BaseEnergyResistance{ get{ return 10; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public RoyalGuardsChestplate()
		{
			Name = "Royal Guard's Chest Plate";
			Hue = 0x47E;
			Attributes.BonusHits = 10;
			Attributes.BonusMana = 10;
			Attributes.BonusStam = 10;
			Attributes.RegenHits = 5;
			Attributes.ReflectPhysical = 25;
		}

		public RoyalGuardsChestplate( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}