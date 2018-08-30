using System;
using Server;

namespace Server.Items
{
	public class RingOfStats : GoldRing
	{
		public override int ArtifactRarity{ get{ return 11; } }

		[Constructable]
		public RingOfStats()
		{
			Name = "Ring Of Stats";
			Hue = 87;
			Attributes.BonusDex = 5;
			Attributes.BonusStr = 10;
			Attributes.BonusInt = 10;
			Attributes.AttackChance = 15;
		}

		public RingOfStats( Serial serial ) : base( serial )
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