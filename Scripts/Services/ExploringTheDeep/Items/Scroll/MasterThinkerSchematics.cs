using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class MasterThinkerSchematics : Item
    {
        public override int LabelNumber => 1023637;  // scroll

        [Constructable]
        public MasterThinkerSchematics() : base(0x46AF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(MTSchematicsGump)))
            {
                from.SendGump(new MTSchematicsGump(from));
            }
        }


        public MasterThinkerSchematics(Serial serial) : base(serial)
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

    public class MTSchematicsGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("MTSchematics", AccessLevel.GameMaster, MTSchematicsGump_OnCommand);
        }

        private static void MTSchematicsGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new MTSchematicsGump(e.Mobile));
        }

        public MTSchematicsGump(Mobile owner) : base(50, 50)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(164, 61, 250, 24, 1154384, 1062086, false, false); // Schematics
            AddHtmlLocalized(147, 90, 250, 24, 1154415, 1062086, false, false); // Cousteau Perron
            AddHtmlLocalized(42, 121, 323, 174, 1154385, 1, false, true); // <I>*There are dozens of extremely detailed schematic drawings along the edges of the paper, and mixed throughout the notes. The writing seems somewhat disjointed, and the hand it's written in is a very jagged and harsh one.*</I> Pressure differential needs to be overcome to ensure test subject survival. Initial attempts resulted in implosion twice, leaks and catastrophic failure five times. Cold Temperature survival gear. Research suitable material. Homogenous distribution necessitated to maximize survival...supply carrying object? Possibly some sort of air elemental hybrid magic utilizing clockwork principles...

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
