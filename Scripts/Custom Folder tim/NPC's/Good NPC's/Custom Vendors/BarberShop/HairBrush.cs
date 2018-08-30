using System;
using Server;
using System.Collections;
using Server.Targeting;
using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
	//[Flipable(0x1372,0x1373)]
	public class HairBrush : Item
	{
		[Constructable]
		public HairBrush() : base( 0x1372 )
		{
		}

		public HairBrush( Serial serial ) : base( serial )
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
                //veeery short
                if (from.HairItemID == 0x2044) // Mohawk
                {
                    from.SendMessage("You take your hair together.");
                    from.HairItemID = 0x204A; //Krisna
                    return;
                }
                if (from.HairItemID == 0x204A) // Krisna
                {
                    from.SendMessage("You open your hair.");
                    from.HairItemID = 0x2044; // Mohawk
                    return;
                }

                //short
                if (from.HairItemID == 0x2045) // Pageboy
                {
                    if (!from.Female)
                    {
                        from.SendMessage("You curl your hair.");
                        from.HairItemID = 0x2047; // Afro
                        return;
                    }
                    else
                    {
                        from.SendMessage("You brush your hair.");
                        from.HairItemID = 0x203B; // Short
                        return;
                    }
                }

                if (from.HairItemID == 0x203B && from.Female) // Short
                {
                    from.SendMessage("You curl your hair.");
                    from.HairItemID = 0x2047; // Afro
                    return;
                }
                if (from.HairItemID == 0x2047) // Afro
                {
                    from.SendMessage("You straighten your hair.");
                    from.HairItemID = 0x2FBF; //MidLong
                    return;
                }
                if (from.HairItemID == 0x2FBF) // MidLong
                {
                    //from.Backpack.FindItemByType(typeof(Feather));

                    //if (Feather.IsChildOf(from.Backpack))
                    //{

                        from.SendMessage("You put a feather in your hair.");
                        from.HairItemID = 0x2FC0; // Feather
                        return;
                    //}
                    //else
                    /*{
                        from.SendMessage("You brush your hair.");
                        from.HairItemID = 0x2045; // Pageboy
                        return;
                    }*/

                }
                if (from.HairItemID == 0x2FC0) // Feather
                {
                    from.SendMessage("You brush your hair and take the feather out.");
                    from.HairItemID = 0x2FC2; // Mullet
                    //from.AddToBackpack (new Feather());
                    return;
                }
                if (from.HairItemID == 0x2FC2) // Mullet
                {
                    from.SendMessage("You make a knot.");
                    from.HairItemID = 0x2FCE; // ElfKnot
                    return;
                }
                if (from.HairItemID == 0x2FCE) // ElfKnot
                {
                    from.SendMessage("You make a bun.");
                    from.HairItemID = 0x2FD0; // BigBun
                    return;
                }
                if (from.HairItemID == 0x2FD0) // BigBun
                {
                    from.SendMessage("You brush your hair.");
                    from.HairItemID = 0x2045; // Pageboy
                    return;
                }
                
                //long
                if (from.HairItemID == 0x2046) // Buns
                {
                    from.SendMessage("You open your hair.");
                    from.HairItemID = 0x203C; // Long
                    return;
                }
                if (from.HairItemID == 0x203C) // Long
                {
                    from.SendMessage("You brush your hair.");
                    from.HairItemID = 0x2FCD; // LongElf
                    return;
                }
                if (from.HairItemID == 0x2FCD) // LongElf
                {
                    from.SendMessage("You take your hair together in a ponytail.");
                    from.HairItemID = 0x203D; // Ponytail
                    return;
                }
                if (from.HairItemID == 0x203D) // Ponytail
                {
                    from.SendMessage("You braid your hair.");
                    from.HairItemID = 0x2FCF; // BraidElf
                    return;
                }
                if (from.HairItemID == 0x2FCF) // BraidElf
                {
                    if (!from.Female)
                    {
                        from.SendMessage("You make two pigtails.");
                        from.HairItemID = 0x2049; // Two Pigtails
                        return;
                    }
                    else
                    {
                    from.SendMessage("You put a flower in your hair.");
                    from.HairItemID = 0x2FCC; // Flower
                    return;
                    }
                }
                if (from.HairItemID == 0x2FCC) // Flower
                {
                    from.SendMessage("You make two pigtails.");
                    from.HairItemID = 0x2049; // Two Pigtails
                    return;
                }
                if (from.HairItemID == 0x2049) // Two Pigtails
                {
                    if (!from.Female)
                    {
                        from.SendMessage("You open your hair.");
                        from.HairItemID = 0x203C; // Long
                        return;
                    }
                    else
                    {
                        from.SendMessage("You roll your pigtails up in buns.");
                        from.HairItemID = 0x2046; // Buns
                        return;
                    }
                }

                

                //short / receeding

                if (from.HairItemID == 0x2048) // receeding
                {
                    from.SendMessage("You brush your hair.");
                    from.HairItemID = 0x2FC1; // ShortElven
                    return;
                }

                if (from.HairItemID == 0x2FC1) // ShortElven
                {
                    from.SendMessage("You brush your hair.");
                    from.HairItemID = 0x2FD1; // Spiked
                    return;
                }
                if (from.HairItemID == 0x2FD1) // Spiked
                {
                    if (!from.Female)
                    {
                        from.SendMessage("You brush your hair.");
                        from.HairItemID = 0x203B; // Short
                        return;
                    }
                    else
                    {
                        from.SendMessage("You brush your hair.");
                        from.HairItemID = 0x2048; // receeding
                        return;
                    }
                }
                if (!from.Female && from.HairItemID == 0x203B) // Short
                {
                    from.SendMessage("You brush your hair.");
                    from.HairItemID = 0x2048; // receeding
                    return;
                }


                else
                {
                    from.SendMessage("Your hair can't be styled.");
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