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
		public string[] Lines => TooltipsBase;

        [Constructable]
        public CustomizableWallSign()
            : base(0x4B20)
        {
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

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

			if (version < 1)
			{
				int lines = reader.ReadInt();

				for (int i = 0; i < lines && i < Lines.Length; i++)
					Lines[i] = reader.ReadString();
			}
        }
    }
}
