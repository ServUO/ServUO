using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class CaptainsLogScroll : Item
    {
        public override int LabelNumber => 1023637;  // scroll

        [Constructable]
        public CaptainsLogScroll() : base(0x46AF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(CaptainsLogGump)))
            {
                from.SendGump(new CaptainsLogGump(from));
            }
        }

        public CaptainsLogScroll(Serial serial) : base(serial)
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

    public class CaptainsLogGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("CaptainsLogLetter", AccessLevel.GameMaster, CaptainsLogGump_OnCommand);
        }

        private static void CaptainsLogGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new CaptainsLogGump(e.Mobile));
        }

        public CaptainsLogGump(Mobile owner) : base(50, 50)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(164, 61, 250, 24, 1154469, 1062086, false, false); // Captain's Log
            AddHtmlLocalized(180, 90, 250, 24, 1154470, 1062086, false, false); // Johne
            AddHtmlLocalized(42, 121, 323, 174, 1154490, 1, false, true); // What have I done...what...I cannot...these vile creatures...I have managed to lock them away within the central cargo hold and I have destroyed the winch assembly needed to remove the cargo hold's cover. I can only hope my efforts to lock away the repair parts will deter any foolish enough to start smashing the supply barrels in an attempt to recover the locker keys...they're all gone now...it is only I now, forever damned to this watery grave....
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
