using System;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class EliseTrentScroll : Item
    {
        public override int LabelNumber { get { return 1023637; } } // scroll

        [Constructable]
        public EliseTrentScroll() : base(0x46AF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(EliseTrentGump)))
            {
                from.SendGump(new EliseTrentGump(from));
            }
        }

        public EliseTrentScroll(Serial serial) : base(serial)
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

    public class EliseTrentGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("EliseTrentScroll", AccessLevel.GameMaster, new CommandEventHandler(EliseTrentGump_OnCommand));
        }

        private static void EliseTrentGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new EliseTrentGump(e.Mobile));
        }

        public EliseTrentGump(Mobile owner) : base(50, 50)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(130, 61, 250, 24, 1154388, 1062086, false, false); // Trinsic Tribune #189
            AddHtmlLocalized(150, 90, 250, 24, 1154376, 1062086, false, false); // Elise Trent
            AddHtmlLocalized(42, 121, 323, 174, 1154389, 1, false, true); // <I>*The article left out of this paper is entitled 'Terror on the Seas'.*</I>There have long been pirates along the waterways of Britannia, but none were as prevalent as Captain Silver John. His boat and his men were as feared for their piracy as for their innovation, and the intelligent captain kept a highly skilled crew to assist in his raids. Amidst them were a talented alchemist, an incredibly skilled blacksmith, a once renowned mage, and several former Rangers. Despite the attempts of Admiral Duarte of the Royal Navy, their preying upon the merchant vessels of the lands was not stopped until one of Silver's own crew sold them out. An unnamed member of his crew led Royal Guardsman to a hideout that they had utilized in the city of Skara Brae, and they laid in wait for the return of the crew. The informant was slain in the fight, as was Captain Silver John himself, and most of his crew is now safely behind bars, though some seem to have escaped. All those captured are refusing to talk, so it seems that those who escaped will continue to remain at large...though it remains to be seen whether they will join a new crew or form their own.
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
