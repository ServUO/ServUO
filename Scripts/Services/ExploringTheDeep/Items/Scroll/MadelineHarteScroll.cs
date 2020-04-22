using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class MadelineHarteScroll : Item
    {
        public override int LabelNumber => 1023637;  // scroll

        [Constructable]
        public MadelineHarteScroll() : base(0x46AF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(MadelineHarteGump)))
            {
                from.SendGump(new MadelineHarteGump(from));
            }
        }

        public MadelineHarteScroll(Serial serial) : base(serial)
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

    public class MadelineHarteGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("MadelineHarteScroll", AccessLevel.GameMaster, MadelineHarteGump_OnCommand);
        }

        private static void MadelineHarteGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new MadelineHarteGump(e.Mobile));
        }

        public MadelineHarteGump(Mobile owner) : base(50, 50)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(170, 61, 250, 24, 1154394, 1062086, false, false); // Letter
            AddHtmlLocalized(150, 90, 250, 24, 1154466, 1062086, false, false); // Madeline Harte
            AddHtmlLocalized(42, 121, 323, 174, 1154399, 1, false, true); // I've been successful in business, in training, in innovation, in invention...in every aspect of my life save for the one I really care about, the relationship with my children. I have lost one to the earth and the other I've effectively lost to the sea. Willem...if only you hadn't been so eager to try and prove yourself to your father. We had enough money to get by, and I would rather live in the street with you than to have had my baby boy die in such a manner. Love is not a commodity worth trading for any amount of gold in the world. 
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
