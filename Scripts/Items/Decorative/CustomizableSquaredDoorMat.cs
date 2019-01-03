using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class CustomizableSquaredDoorMatAddon : BaseAddon
    {
        public string[] Lines { get; set; }

        public override BaseAddonDeed Deed { get { return new CustomizableSquaredDoorMatDeed(); } }

        [Constructable]
        public CustomizableSquaredDoorMatAddon(DirectionType type)
        {
            Lines = new string[3];
            
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new CustomizableSquaredDoorMatComponent(0x4AB6), 0, 0, 0);
                    AddComponent(new CustomizableSquaredDoorMatComponent(0x4AB7), 1, 0, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new CustomizableSquaredDoorMatComponent(0x4AB4), 0, 0, 0);
                    AddComponent(new CustomizableSquaredDoorMatComponent(0x4AB5), 0, 1, 0);
                    break;
            }
        }

        public CustomizableSquaredDoorMatAddon(Serial serial)
            : base(serial)
        {
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
    }

    public class CustomizableSquaredDoorMatComponent : LocalizedAddonComponent
    {
        public override bool ForceShowProperties { get { return true; } }

        public CustomizableSquaredDoorMatComponent(int id)
            : base(id, 1097996) // door mat
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            CustomizableSquaredDoorMatAddon addon = Addon as CustomizableSquaredDoorMatAddon;

            if (addon != null)
            {
                if (addon.Lines != null)
                {
                    for (int i = 0; i < addon.Lines.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(addon.Lines[i]))
                        {
                            list.Add(1150301 + i, addon.Lines[i]); // [ ~1_LINE0~ ]
                        }
                    }
                }
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && house.IsCoOwner(from))
            {
                list.Add(new EditSign((CustomizableSquaredDoorMatAddon)Addon, from));
            }
        }

        private class EditSign : ContextMenuEntry
        {
            private readonly CustomizableSquaredDoorMatAddon Addon;
            private readonly Mobile _From;

            public EditSign(CustomizableSquaredDoorMatAddon addon, Mobile from)
                : base(1151817) // Edit Sign
            {
                Addon = addon;
                _From = from;
            }

            public override void OnClick()
            {
                _From.SendGump(new AddMessageGump(Addon));
            }
        }

        public CustomizableSquaredDoorMatComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private class AddMessageGump : Gump
        {
            private readonly CustomizableSquaredDoorMatAddon Addon;

            public AddMessageGump(CustomizableSquaredDoorMatAddon addon)
                : base(100, 100)
            {
                Addon = addon;

                string line1 = "";
                string line2 = "";
                string line3 = "";

                if (Addon.Lines != null && Addon.Lines.Length > 0)
                {
                    line1 = Addon.Lines[0];
                    line2 = Addon.Lines[1];
                    line3 = Addon.Lines[2];
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
                        Addon.Lines[i] = s;
                    }

                    Addon.UpdateProperties();
                }
            }
        }
    }

    public class CustomizableSquaredDoorMatDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon { get { return new CustomizableSquaredDoorMatAddon(m_CustomizableSquaredDoorMatType); } }

        private DirectionType m_CustomizableSquaredDoorMatType;

        public override int LabelNumber { get { return 1151806; } } // squared door mat deed

        [Constructable]
        public CustomizableSquaredDoorMatDeed()
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, LabelNumber));
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public CustomizableSquaredDoorMatDeed(Serial serial)
            : base(serial)
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

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1151815);
            list.Add((int)DirectionType.East, 1151816);
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            m_CustomizableSquaredDoorMatType = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }    
}
