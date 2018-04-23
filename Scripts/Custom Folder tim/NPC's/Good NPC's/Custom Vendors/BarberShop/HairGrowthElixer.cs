using System;
using Server;
using System.Collections;
using Server.Targeting;
using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
	public class HairGrowthElixir : Item
	{
		[Constructable]
		public HairGrowthElixir() : base( 0xE26 )
		{
            Name = "hair growth elixir";
		}

		public HairGrowthElixir( Serial serial ) : base( serial )
		{
		}

        public override void OnDoubleClick(Mobile from)
        {

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            else
            {
                //veeery short // mohawk, krisna
                if (from.HairItemID == 0x2044 || from.HairItemID == 0x204A)
                {
                    Delete();
                    from.SendMessage("You use the elixir on your head. Hair grows on the bald parts of your head.");
                    from.HairItemID = 0x2045; 
                    return;
                }

                //short
                if (from.HairItemID == 0x2045 || from.HairItemID == 0x2047 || from.HairItemID == 0x203B || from.HairItemID == 0x2047 || from.HairItemID == 0x2FBF || from.HairItemID == 0x2FC0 || from.HairItemID == 0x2FC2 || from.HairItemID == 0x2FCE || from.HairItemID == 0x2FD0) 
                {
                    Delete();
                    from.SendMessage("You use the elixir on your hair.");
                    from.HairItemID = 0x203C;
                    return;
                }                

                //short / receeding

                if (from.HairItemID == 0x2048 || from.HairItemID == 0x2FC1 || from.HairItemID == 0x2FD1 || from.HairItemID == 0x203B) // receeding
                {
                    Delete();
                    from.SendMessage("You use the elixir on your hair.");
                    from.HairItemID = 0x2045; 
                    return;
                }

                if (from.HairItemID == 0) 
                {
                    Delete();
                    from.SendMessage("You use the elixir on your head.");
                    from.HairItemID = 0x2048;
                    return;
                }
                else
                {
                    Delete();
                    from.SendMessage("You use the elixir but nothing happens.");
                    return;
                }
            }
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