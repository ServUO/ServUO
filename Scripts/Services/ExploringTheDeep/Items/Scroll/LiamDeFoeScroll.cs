using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class LiamDeFoeScroll : Item
    {
        public override int LabelNumber => 1023637;  // scroll

        [Constructable]
        public LiamDeFoeScroll() : base(0x46AF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(LiamDeFoeGump)))
            {
                from.SendGump(new LiamDeFoeGump(from));
            }
        }

        public LiamDeFoeScroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LiamDeFoeGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("LiamDeFoe", AccessLevel.GameMaster, LiamDeFoeGump_OnCommand);
        }

        private static void LiamDeFoeGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new LiamDeFoeGump(e.Mobile));
        }

        public LiamDeFoeGump(Mobile owner) : base(50, 50)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(150, 61, 250, 24, 1154396, 1062086, false, false); // It Is With Regret...
            AddHtmlLocalized(160, 90, 250, 24, 1154418, 1062086, false, false); // Liam DeFoe
            AddHtmlLocalized(42, 121, 323, 174, 1154397, 1, false, true); // Mrs. Madeline Hart,<BR>It is with our utmost regret that we must inform you of the death of your son, Willem Hart, who accompanied us on our mission into Destard. Despite our best efforts, we were attacked by a horde of dragons...your son fought valiantly and held to the highest pillars of Valor and Sacrifice in doing so. He acquitted himself incredibly so against them, and we thought the day was won, until an Ancient wyrm sprung from the depths of the dungeon. Willem showed some trepidation, but his manner was in the highest dealings of Courage as he assisted us in holding it off until the miners could escape. Unfortunately, this cost him his life, and no manner of magic could coax him back. As such, we have appended all of his commission for his aid, as well as a sizeable fund donated by the miners whose lives he saved. I am sorry for your loss, and will assist in any arrangements that are necessary.<BR>With Regards,<BR>Liam DeFoe.
        }

        public override void OnResponse(NetState state, RelayInfo info) //Function for GumpButtonType.Reply Buttons 
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        //Cancel                        
                        break;
                    }
            }
        }
    }
}
