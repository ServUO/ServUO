using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class SealedLettersScroll : Item
    {
        public override int LabelNumber => 1023637;  // scroll

        [Constructable]
        public SealedLettersScroll() : base(0x46AF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(SealedLettersEntryGump)))
            {
                from.SendGump(new SealedLettersEntryGump(from));
            }
        }

        public SealedLettersScroll(Serial serial) : base(serial)
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

    public class SealedLettersEntryGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("SealedLettersEntry", AccessLevel.GameMaster, SealedLettersEntryGump_OnCommand);
        }

        private static void SealedLettersEntryGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new SealedLettersEntryGump(e.Mobile));
        }

        public SealedLettersEntryGump(Mobile owner) : base(50, 50)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(160, 61, 250, 24, 1154404, 1062086, false, false); // Sealed Letters
            AddHtmlLocalized(155, 90, 250, 24, 1154419, 1062086, false, false); // Champ Huthwait
            AddHtmlLocalized(42, 121, 323, 174, 1154406, 1, false, true); // <I>*The letters are sealed in tubes capped with wax to make them waterproof and hidden away. Written on the outside is “Open only on the occasion of the death of Champ Huthwait, to be delivered to the Britain Bugle, the Trinsic Tribune, and the Sosarian Scout.”*</I>
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
