using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class ElfGiftBox2017 : GiftBox
	{
		[Constructable]
		public  ElfGiftBox2017 ()
		{
			Name = "Elf Gift Box 2017";
			Hue = 33;
            

            DropItem(new SantaStatue());
            DropItem(new SantaBag());

            
          
			
		
		}

        public ElfGiftBox2017(Serial serial)
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