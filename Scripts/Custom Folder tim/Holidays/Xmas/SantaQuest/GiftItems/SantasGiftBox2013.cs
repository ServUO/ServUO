using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class SantasGiftBox2013 : GiftBox
	{
		[Constructable]
		public  SantasGiftBox2013 ()
		{
			Name = " Santas Gift Box 2013 !";
			Hue = 33;
            

            DropItem(new SantasSleighSmallAddonDeed());
            DropItem(new SantasReindeer1A());
            DropItem(new SantasReindeer2A());
			
		
		}

        public SantasGiftBox2013(Serial serial)
            : base(serial)
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