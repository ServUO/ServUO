using System;
using Server;

namespace Server.Items
{
	public class ShieldOfIntellect : MetalShield
	{ 
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public ShieldOfIntellect()
		{
			Name = "Shield Of Intellect";
			Hue = 1170;
			Attributes.LowerRegCost = 30;
			Attributes.BonusMana = 30;	
			Attributes.SpellChanneling = 1;
			Attributes.DefendChance = 15;
		}

		public ShieldOfIntellect( Serial serial ) : base( serial )
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