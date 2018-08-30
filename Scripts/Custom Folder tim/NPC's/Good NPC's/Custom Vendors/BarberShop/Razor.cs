using System;
using Server;
using System.Collections;
using Server.Targeting;
using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
	public class Razor : Item
	{
		[Constructable]
		public Razor() : base( 0xEC4 )
		{
            Name = "Razor";
		}

		public Razor( Serial serial ) : base( serial )
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
                    Point3D scissorloc = from.Location; // added cuthair
                    CutHair cuthair = new CutHair();
                    cuthair.Location = scissorloc;
                    cuthair.MoveToWorld(scissorloc, from.Map);

                    from.SendMessage("You shave your hair.");
                    from.HairItemID = 0; // no hair
                    return;
                }

                // Middle
                if (from.HairItemID == 0x2045 || from.HairItemID == 0x2047 || from.HairItemID == 0x203B || from.HairItemID == 0x2047 || from.HairItemID == 0x2FBF || from.HairItemID == 0x2FC0 || from.HairItemID == 0x2FC2 || from.HairItemID == 0x2FCE || from.HairItemID == 0x2FD0) 
                {
                    Point3D scissorloc = from.Location; // added cuthair
                    CutHair cuthair = new CutHair();
                    cuthair.Location = scissorloc;
                    cuthair.MoveToWorld(scissorloc, from.Map);

                    from.SendMessage("You shave your hair.");
                    from.HairItemID = 0x2044; // mohawk
                    return;
                }                

                // Short
                if (from.HairItemID == 0x2048 || from.HairItemID == 0x2FC1 || from.HairItemID == 0x2FD1 || from.HairItemID == 0x203B) // receeding
                {
                    Point3D scissorloc = from.Location; // added cuthair
                    CutHair cuthair = new CutHair();
                    cuthair.Location = scissorloc;
                    cuthair.MoveToWorld(scissorloc, from.Map);

                    from.SendMessage("You shave your hair.");
                    from.HairItemID = 0; // no hair
                    return;
                }

                if (from.HairItemID == 0)
                {
                    from.SendMessage("You cannot shave your hair. You got none!");
                    return;
                }

                else
                {
                    from.SendMessage("You cannot shave your hair. First cut it a bit.");
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