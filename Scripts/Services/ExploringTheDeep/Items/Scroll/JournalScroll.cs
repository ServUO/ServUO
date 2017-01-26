using System;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class JournalScroll : Item
    {
        public override int LabelNumber { get { return 1023637; } } // scroll

        [Constructable]
        public JournalScroll() : base(0x46AF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(JournalGump)))
            {
                from.SendGump(new JournalGump(from));
            }
        }

        public JournalScroll(Serial serial) : base(serial)
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

    public class JournalGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("JournalLetter", AccessLevel.GameMaster, new CommandEventHandler(JournalGump_OnCommand));
        }

        private static void JournalGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new JournalGump(e.Mobile));
        }

        public JournalGump(Mobile owner) : base(50, 50)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(165, 61, 250, 24, 1094837, 1062086, false, false); // a journal
            AddHtmlLocalized(170, 90, 250, 24, 1154420, 1062086, false, false); // Mercutio
            AddHtmlLocalized(42, 121, 323, 174, 1154409, 1, false, true); // The idiots constantly think there's some kind of cheating going on here, but nothing could be further from the truth. We don't need to have any kind of cheat to deal with these idiots. They all think they have some kind of special system that'll work here, but the simple fact is that the odds are always on our side. We win because that's how it's always been meant to work...doesn't stop the bloody saps from trying though. Not that we discourage the practices of fools who think they found a way to win.
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
