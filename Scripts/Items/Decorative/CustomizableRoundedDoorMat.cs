using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    [Furniture]
    [FlipableAttribute(0x4790, 0x4791)]
    public class CustomizableRoundedDoorMat : Item, IDyable
    {
        public string[] Lines { get; set; }

        [Constructable]
        public CustomizableRoundedDoorMat()
            : base(0x4790)
        {
            Lines = new string[3];
            LootType = LootType.Blessed;
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendGump(new AddMessageGump(this));
            }
            else
            {
                from.SendLocalizedMessage(1116249); // That must be in your backpack for you to use it.
            }            
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);            
            
            if (Lines != null)
            {
                for (int i = 0; i < Lines.Length; i++)
                {
                    if (!string.IsNullOrEmpty(Lines[i]))
                    {
                        list.Add(1150301 + i, Lines[i]); // [ ~1_LINE0~ ]
                    }
                }
            }
        }

        public CustomizableRoundedDoorMat(Serial serial)
            : base(serial)
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && house.IsCoOwner(from))
            {
                list.Add(new EditSign(this, from));
            }
        }

        private class EditSign : ContextMenuEntry
        {
            private readonly CustomizableRoundedDoorMat Mat;
            private readonly Mobile _From;

            public EditSign(CustomizableRoundedDoorMat mat, Mobile from)
                : base(1151817) // Edit Sign
            {
                Mat = mat;
                _From = from;
            }

            public override void OnClick()
            {
                if (Mat.IsChildOf(_From.Backpack))
                {
                    _From.SendGump(new AddMessageGump(Mat));
                }
                else
                {
                    _From.SendLocalizedMessage(1116249); // That must be in your backpack for you to use it.
                }               
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)Lines.Length);

            for (int i = 0; i < Lines.Length; i++)
                writer.Write((string)Lines[i]);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Lines = new string[reader.ReadInt()];

            for (int i = 0; i < Lines.Length; i++)
                Lines[i] = reader.ReadString();
        }

        private class AddMessageGump : Gump
        {
            private readonly CustomizableRoundedDoorMat Mat;

            public AddMessageGump(CustomizableRoundedDoorMat mat)
                : base(100, 100)
            {
                Mat = mat;

                string line1 = "";
                string line2 = "";
                string line3 = "";

                if (Mat.Lines != null && Mat.Lines.Length > 0)
                {
                    line1 = Mat.Lines[0];
                    line2 = Mat.Lines[1];
                    line3 = Mat.Lines[2];
                }

                AddPage(0);

                AddBackground(0, 0, 420, 320, 0x2454);
                AddHtmlLocalized(10, 10, 400, 18, 1114513, "#1151680", 0x4000, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddHtmlLocalized(10, 37, 400, 90, 1151681, 0x14AA, false, false); // Enter up to three lines of personallized text.  you may enter up to 25 characters per line.
                AddHtmlLocalized(10, 136, 400, 16, 1150296, 0x14AA, false, false); // Line 1:
                AddBackground(10, 152, 400, 22, 0x2486);
                AddTextEntry(12, 154, 396, 18, 0x9C2, 0, line1, 25);
                AddHtmlLocalized(10, 178, 400, 16, 1150297, 0x14AA, false, false); // Line 2:
                AddBackground(10, 194, 400, 22, 0x2486);
                AddTextEntry(12, 196, 396, 18, 0x9C2, 1, line2, 25);
                AddHtmlLocalized(10, 220, 400, 16, 1150298, 0x14AA, false, false); // Line 3:
                AddBackground(10, 236, 400, 22, 0x2486);
                AddTextEntry(12, 238, 396, 18, 0x9C2, 2, line3, 25);
                AddButton(10, 290, 0xFAB, 0xFAC, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 290, 100, 20, 1150299, 0x10, false, false); // ACCEPT
                AddButton(380, 290, 0xFB4, 0xFB5, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(270, 290, 100, 20, 1114514, "#1150300 ", 0x10, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 1)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        TextRelay text = info.GetTextEntry(i);
                        string s = text.Text.Substring(0, 25);
                        Mat.Lines[i] = s;
                    }

                    Mat.InvalidateProperties();
                }
            }
        }
    }
}
