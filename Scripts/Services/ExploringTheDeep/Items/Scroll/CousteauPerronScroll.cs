using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class CousteauPerronScroll : Item
    {
        public override int LabelNumber => 1023637;  // scroll

        [Constructable]
        public CousteauPerronScroll() : base(0x46AF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(CousteauPerronInformationGump)))
            {
                from.SendGump(new CousteauPerronInformationGump(from));
            }
        }

        public CousteauPerronScroll(Serial serial) : base(serial)
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

    public class CousteauPerronInformationGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("CousteauPerronScroll", AccessLevel.GameMaster, CousteauPerronInformationGump_OnCommand);
        }

        private static void CousteauPerronInformationGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new CousteauPerronInformationGump(e.Mobile));
        }

        public CousteauPerronInformationGump(Mobile owner) : base(50, 50)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(130, 61, 250, 24, 1154392, 1062086, false, false); // Request for Information
            AddHtmlLocalized(150, 90, 250, 24, 1154415, 1062086, false, false); // Cousteau Perron
            AddHtmlLocalized(42, 121, 323, 174, 1154393, 1, false, true); // <I>*A hastily composed note seems scratched out on a shred of a scroll, with a few abstract equations and drawings near the periphery.*</I><BR>Close to breakthrough, require reforged shadow iron ringmail suit(10), power crystal (5), miniaturized clockwork assembly (20), valorite keg (20), and six more verite toolkits of the usual kind. Expect delivery in three days. Usual payment protocol.<BR><I>*At the end, there seems to be a note added on as if in afterthought.*</I><BR>Have reconsidered previous offer and will pay 95% asking price for withheld schematics.
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
