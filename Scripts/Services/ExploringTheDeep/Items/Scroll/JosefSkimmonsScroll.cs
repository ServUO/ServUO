using System;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class JosefSkimmonsScroll : Item
    {
        public override int LabelNumber { get { return 1023637; } } // scroll

        [Constructable]
        public JosefSkimmonsScroll() : base(0x46AF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(JosefSkimmonsPrivateGump)))
            {
                from.SendGump(new JosefSkimmonsPrivateGump(from));
            }
        }

        public JosefSkimmonsScroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class JosefSkimmonsPrivateGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("JosefSkimmonsScroll", AccessLevel.GameMaster, new CommandEventHandler(JosefSkimmonsPrivateGump_OnCommand));
        }

        private static void JosefSkimmonsPrivateGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new JosefSkimmonsPrivateGump(e.Mobile));
        }

        public JosefSkimmonsPrivateGump(Mobile owner) : base(50, 50)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(150, 61, 250, 24, 1154390, 1062086, false, false); // Private Journal
            AddHtmlLocalized(150, 90, 250, 24, 1154416, 1062086, false, false); // Josef Skimmons
            AddHtmlLocalized(42, 121, 323, 174, 1154391, 1, false, true); // <I>*This appears to be private journal of Skimmons Josef.*</I>Bloody old Cousteau keeps on coming around and asking me about things I'd much sooner forget. I never bloody wanted to work for that crazed wench on her blasted clockwork abominations, but there weren't much else I could manage after the mess that got made in Skara. Least I got out of both with my skin intact. Still, she's trying to go even further then we did back then...Lass is still trying to make that dead sister of hers proud I reckon. If we'd had the kinda things she's trying to make back then, nobody could've touched us. Makes the little goggles I used to make look like a kid's toy.
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
