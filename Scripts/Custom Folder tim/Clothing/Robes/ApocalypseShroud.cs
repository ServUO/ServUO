using System;
using Server;

namespace Server.Items
{
	public class ShroudOfApocalypse : HoodedShroudOfShadows
	{
		public override int ArtifactRarity{ get{ return 187; } }

		public override int BasePhysicalResistance{ get{ return 35; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
        public ShroudOfApocalypse()
		{
            Name = "Shroud Of Apocalypse 2012"; 
			Hue = 33;
			Attributes.BonusHits = 20;
			Attributes.Luck = 187;
			Attributes.ReflectPhysical = 12;
		}

		public ShroudOfApocalypse( Serial serial ) : base( serial )
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