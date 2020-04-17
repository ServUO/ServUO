using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using System.Collections.Generic;

namespace Server.Items
{
    public class CustomizableSquaredDoorMatAddon : BaseAddon, ICustomizableMessageItem
    {
        public string[] Lines { get; set; }

        public override BaseAddonDeed Deed => new CustomizableSquaredDoorMatDeed();

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
                    AddComponent(new CustomizableSquaredDoorMatComponent(0x4AB4), 0, 1, 0);
                    AddComponent(new CustomizableSquaredDoorMatComponent(0x4AB5), 0, 0, 0);
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

    public class CustomizableSquaredDoorMatComponent : LocalizedAddonComponent
    {
        public override bool ForceShowProperties => true;

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

            if (house != null && house.IsCoOwner(from) && from is PlayerMobile)
            {
                list.Add(new EditSign((CustomizableSquaredDoorMatAddon)Addon, (PlayerMobile)from));
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
    }

    public class CustomizableSquaredDoorMatDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon => new CustomizableSquaredDoorMatAddon(m_CustomizableSquaredDoorMatType);

        private DirectionType m_CustomizableSquaredDoorMatType;

        public override int LabelNumber => 1151806;  // squared door mat deed

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
            writer.Write(0); // version
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
