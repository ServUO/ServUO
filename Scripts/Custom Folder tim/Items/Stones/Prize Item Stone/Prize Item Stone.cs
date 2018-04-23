//Scripted by Energy!
//ICQ 411-144-844

using System;
using Server.Items;
using Server.Gumps;

namespace Server.Items
{
	public class PrizeStone : Item
	{
		[Constructable]
		public PrizeStone() : base( 0xED4 )
		{
			Movable = false;
			Hue = 1109;
            Name = "Prize Stone";
		}

        public override void OnDoubleClick(Mobile from) //Дабл клик открывает гамп PrizeGump
		{

            from.SendGump(new PrizeGump());
		
		   	
					
		}

        public PrizeStone(Serial serial)
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
} //КОнец
