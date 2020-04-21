using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class WillemHarteScroll : Item
    {
        public override int LabelNumber => 1023637;  // scroll

        [Constructable]
        public WillemHarteScroll() : base(0x46AF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(WillemHarteGump)))
            {
                from.SendGump(new WillemHarteGump(from));
            }
        }

        public WillemHarteScroll(Serial serial) : base(serial)
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

    public class WillemHarteGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("WillemHarte", AccessLevel.GameMaster, WillemHarteGump_OnCommand);
        }

        private static void WillemHarteGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new WillemHarteGump(e.Mobile));
        }

        public WillemHarteGump(Mobile owner) : base(50, 50)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(175, 61, 250, 24, 1154394, 1062086, false, false); // Letter
            AddHtmlLocalized(155, 90, 250, 24, 1154417, 1062086, false, false); // Willem Hart
            AddHtmlLocalized(42, 121, 323, 174, 1154395, 1, false, true); // <I>*The letter seems to have been read many times and has areas where the ink has run from tears.*</I><BR>Mother, it's time for me to go out and make my own way. Isaiah's long since left and though I know how it has hurt you that he has sent fewer and fewer letters, but I can no longer stay here while my destiny lies out there. I've joined a group of skilled fighters who are planning to earn their keep escorting a group of miners attempting to gather some sort of special ore from Destard. It will be dangerous, but we're being led by an experienced guardsman from Skara Brae.<BR><BR>Virtues be with you, Mother.
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
