using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class FortuneseekerTreads : Sandals
  {

  

      
      [Constructable]
		public FortuneseekerTreads()
		{
          Name = "Fortuneseeker's Treads";
          Hue = 1161;
    
      Attributes.BonusHits = 10;
      Attributes.BonusMana = 10;
      Attributes.DefendChance = 5;
	  Attributes.Luck = 75;
		}

		public FortuneseekerTreads( Serial serial ) : base( serial )
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
