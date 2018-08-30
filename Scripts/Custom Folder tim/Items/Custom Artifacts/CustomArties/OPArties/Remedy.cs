
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class Remedy : GoldBracelet
  {


      
      [Constructable]
		public Remedy()
		{
          Name = "Remedy";
          Hue = 1176;
      Attributes.BonusHits = 10;
      Attributes.BonusMana = 10;
      Attributes.BonusStam = 10;
      Attributes.RegenHits = 10;
      Attributes.DefendChance = 25;
		}

		public Remedy( Serial serial ) : base( serial )
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
