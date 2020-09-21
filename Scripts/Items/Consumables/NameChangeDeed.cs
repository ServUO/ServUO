using Server.Gumps;
using Server.Misc;
using Server.Network;

namespace Server.Items
{
    public class NameChangeDeed : Item
    {
        [Constructable]
        public NameChangeDeed()
            : base(0x14F0)
        {
            LootType = LootType.Blessed;
        }

        public NameChangeDeed(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName => "a name change deed";
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

        public override void OnDoubleClick(Mobile from)
        {
            if (RootParent == from)
            {
                from.CloseGump(typeof(NameChangeDeedGump));
                from.SendGump(new NameChangeDeedGump(this));
            }
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }
    }
    public class NameChangeDeedGump : Gump
    {
        readonly Item m_Sender;

        public void AddBlackAlpha(int x, int y, int width, int height)
        {
            AddImageTiled(x, y, width, height, 2624);
            AddAlphaRegion(x, y, width, height);
        }

        public void AddTextField(int x, int y, int width, int height, int index)
        {
            AddBackground(x - 2, y - 2, width + 4, height + 4, 0x2486);
            AddTextEntry(x + 2, y + 2, width - 4, height - 4, 0, index, "");
        }

        public string Center(string text)
        {
            return string.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public void AddButtonLabeled(int x, int y, int buttonID, string text)
        {
            AddButton(x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0);
            AddHtml(x + 35, y, 240, 20, Color(text, 0xFFFFFF), false, false);
        }

        public NameChangeDeedGump(Item sender)
            : base(50, 50)
        {
            m_Sender = sender;

            Closable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            AddBlackAlpha(10, 120, 250, 85);
            AddHtml(10, 125, 250, 20, Color(Center("Name Change Deed"), 0xFFFFFF), false, false);

            AddLabel(73, 15, 1152, "");
            AddLabel(20, 150, 0x480, "New Name:");
            AddTextField(100, 150, 150, 20, 0);

            AddButtonLabeled(75, 180, 1, "Submit");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Sender == null || m_Sender.Deleted || info.ButtonID != 1 || m_Sender.RootParent != sender.Mobile)
                return;

            Mobile m = sender.Mobile;
            TextRelay nameEntry = info.GetTextEntry(0);

            string newName = (nameEntry == null ? null : nameEntry.Text.Trim());


            if (!NameVerification.Validate(newName, 2, 16, true, true, true, 1, NameVerification.SpaceDashPeriodQuote))
            {
                m.SendMessage("That name is unacceptable.");
                return;
            }
            else
            {
                m.RawName = newName;
                m.SendMessage("Your name has been changed!");
                m.SendMessage(string.Format("You are now known as {0}", newName));
                m_Sender.Delete();
            }
        }
    }
}