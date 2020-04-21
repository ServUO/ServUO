using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class LedgerScroll : Item
    {
        public override int LabelNumber => 1023637;  // scroll

        [Constructable]
        public LedgerScroll() : base(0x46AF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(LedgerGump)))
            {
                from.SendGump(new LedgerGump(from));
            }
        }

        public LedgerScroll(Serial serial) : base(serial)
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

    public class LedgerGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("LedgerLetter", AccessLevel.GameMaster, LedgerGump_OnCommand);
        }

        private static void LedgerGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new LedgerGump(e.Mobile));
        }

        public LedgerGump(Mobile owner) : base(50, 50)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(178, 61, 250, 24, 1154407, 1062086, false, false); // Ledger
            AddHtmlLocalized(170, 90, 250, 24, 1154420, 1062086, false, false); // Mercutio
            AddHtmlLocalized(42, 121, 323, 174, 1154408, 1, false, true); // Collected from Amos, Leto, and Kas. Sent the husks to Nick. 7/21<BR>Collected from Mathias. Sent the husks to Gaff and Bawdewyn. 7/24<BR>Collected from Nick and Bawdewyn. Set the dogs to Gaff. 7/27<BR>Partial collection from Champ. Sent the husks to Reann and Triston. 8/3<BR>Sent the husks to Irvine, Lora, and Visko. 8/5<BR>Collected from Triston. Set the dogs on Reann. 8/6<BR>Collected from Irvine and Lora. Visko fled. Set a warning if they return. 8/8<BR>
        }

        public override void OnResponse(NetState state, RelayInfo info)
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
