using System;
using Server;

namespace Server.Items
{
	public class ShroudOfBale : HoodedShroudOfShadows
	{
		public override int ArtifactRarity{ get{ return 13; } }

		public override int BasePhysicalResistance{ get{ return 35; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public ShroudOfBale()
		{
			Name = "-Shroud Of Bale-";
			Hue = 1910;
			Attributes.BonusHits = 10;
			Attributes.Luck = 150;
			Attributes.ReflectPhysical = 5;
            Attributes.SpellDamage = Attributes.RegenHits = Utility.Random(100);
		}

		public ShroudOfBale( Serial serial ) : base( serial )
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