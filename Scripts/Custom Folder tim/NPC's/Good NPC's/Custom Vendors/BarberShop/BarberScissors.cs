using System;
using Server;
using System.Collections;
using Server.Targeting;
using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
	public class BarberScissors : Item
	{
		[Constructable]
		public BarberScissors() : base( 0xDFC )
		{
		}

		public BarberScissors( Serial serial ) : base( serial )
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
                    from.SendMessage("You cannot cut your hair shorter. Try use a razor on it.");
                    return;
                }

                //short
                if (from.HairItemID == 0x2045 || from.HairItemID == 0x2047 || from.HairItemID == 0x203B || from.HairItemID == 0x2047 || from.HairItemID == 0x2FBF || from.HairItemID == 0x2FC0 || from.HairItemID == 0x2FC2 || from.HairItemID == 0x2FCE || from.HairItemID == 0x2FD0) 
                {
                    Point3D scissorloc = from.Location; // added cuthair
                    CutHair cuthair = new CutHair();
                    cuthair.Location = scissorloc;
                    cuthair.MoveToWorld(scissorloc, from.Map);

                    from.SendMessage("You cut your hair.");
                    from.HairItemID = 0x2048;
                    from.PlaySound(0x249); // added sound
                    return;
                }                

                //short / receeding

                if (from.HairItemID == 0x2048 || from.HairItemID == 0x2FC1 || from.HairItemID == 0x2FD1 || from.HairItemID == 0x203B) // receeding
                {

                    from.SendMessage("You cannot cut your hair shorter. Try use a razor on it.");
                    return;
                }

                if (from.HairItemID == 0)
                {
                    from.SendMessage("You cannot cut your hair shorter. There is none!");
                    return;
                }


                else
                {
                    Point3D scissorloc = from.Location;
                    CutHair cuthair = new CutHair();
                    cuthair.Location = scissorloc;
                    cuthair.MoveToWorld(scissorloc, from.Map);

                    from.SendMessage("You cut your hair.");
                    from.HairItemID = 0x2045;
                    from.PlaySound(0x249);
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