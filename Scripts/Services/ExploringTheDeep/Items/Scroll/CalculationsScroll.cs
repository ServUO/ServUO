using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class CalculationsScroll : Item
    {
        public override int LabelNumber => 1023637;  // scroll

        [Constructable]
        public CalculationsScroll() : base(0x46AF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(CalculationsGump)))
            {
                from.SendGump(new CalculationsGump(from));
            }
        }

        public CalculationsScroll(Serial serial) : base(serial)
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

    public class CalculationsGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("CalculationsLetter", AccessLevel.GameMaster, CalculationsGump_OnCommand);
        }

        private static void CalculationsGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new CalculationsGump(e.Mobile));
        }

        public CalculationsGump(Mobile owner) : base(50, 50)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(164, 61, 250, 24, 1154400, 1062086, false, false); // Calculations
            AddHtmlLocalized(147, 90, 250, 24, 1154419, 1062086, false, false); // Champ Huthwait
            AddHtmlLocalized(42, 121, 323, 174, 1154401, 1, false, true); // 1259<BR>-300<BR>-324<BR>-569<BR>-1290<BR>245<BR>867<BR>-3250<BR>9230<BR>-12348<BR>-23398<BR>860<BR>-2407<BR><I>*There appears to be a lot of scratches of the pen all over this paper, as if it were calculated while under duress.*</I>
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
