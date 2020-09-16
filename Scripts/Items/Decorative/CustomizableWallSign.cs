using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using System.Collections.Generic;

namespace Server.Items
{
    [Flipable(0x4B20, 0x4B21)]
    public class CustomizableWallSign : Item, ICustomizableMessageItem
    {
        public string[] Lines { get; set; }

        [Constructable]
        public CustomizableWallSign()
            : base(0x4B20)
        {
            Lines = new string[3];
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (from is PlayerMobile)
                    BaseGump.SendGump(new AddCustomizableMessageGump((PlayerMobile)from, this));
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

        public CustomizableWallSign(Serial serial)
            : base(serial)
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && house.IsCoOwner(from) && from is PlayerMobile)
            {
                list.Add(new EditSign(this, (PlayerMobile)from));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(Lines.Length);

            for (int i = 0; i < Lines.Length; i++)
                writer.Write(Lines[i]);
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
}
