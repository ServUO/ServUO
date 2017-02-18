using System;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class SuspicionsScroll : Item
    {
        public override int LabelNumber { get { return 1023637; } } // scroll

        [Constructable]
        public SuspicionsScroll() : base(0x46AF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(SuspicionsGump)))
            {
                from.SendGump(new SuspicionsGump(from));
            }
        }

        public SuspicionsScroll(Serial serial) : base(serial)
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

    public class SuspicionsGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("SuspicionsLetters", AccessLevel.GameMaster, new CommandEventHandler(SuspicionsGump_OnCommand));
        }

        private static void SuspicionsGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new SuspicionsGump(e.Mobile));
        }

        public SuspicionsGump(Mobile owner) : base(50, 50)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(164, 61, 250, 24, 1154402, 1062086, false, false); // Suspicions
            AddHtmlLocalized(147, 90, 250, 24, 1154419, 1062086, false, false); // Champ Huthwait
            AddHtmlLocalized(42, 121, 323, 174, 1154403, 1, false, true); // I'm sure the bastards are cheating, but I can't find out what their system is or why they always win when it's a larger bet. I can't seem to figure out their system, and it's better than mine! If only I didn't have all those distractions with how loud it is in there from the other tables. People talking about how their piddling little days went, how happy they are to meet someone, even that once renowned thief getting hired by someone. It doesn't mean anything, and it's throwing me off my game! Mercutio's breathing down my neck as it is...at least I still have those hidden documents so he doesn't dare kill me outright...but it won't be long before his patience runs out and even that threat won't be enough.
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
