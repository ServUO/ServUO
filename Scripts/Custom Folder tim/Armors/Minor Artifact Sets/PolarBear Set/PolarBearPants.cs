using System;
using Server;

namespace Server.Items
{
	public class PolarBearPants : HidePants
	{
		public override int LabelNumber{ get{ return 1070637; } }

		public override int BasePhysicalResistance{ get{ return 18; } }
		public override int BaseColdResistance{ get{ return 18; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public PolarBearPants()
		{
			Name = "Polar Bear Pants";
		
			Hue = 0x481;

			ArmorAttributes.SelfRepair = 3;

			Attributes.RegenHits = 2;
			Attributes.NightSight = 1;
		}

		public PolarBearPants( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 );
		}
			
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		
		}
	}
}