//Made By Silver
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class Christmasslash : BodySash
  {


      
      [Constructable]
		public Christmasslash()
		{
			Weight = 2;
          Name = "Christmas slash";
          Hue = 38;
      Attributes.LowerManaCost = 10;
      LootType = LootType.Blessed;
		}

		public Christmasslash( Serial serial ) : base( serial )
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
