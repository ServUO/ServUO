using System;
using Server.Gumps;

namespace Server.Items
{
    public class FourPostBedAddon : BaseAddon, IDyable
    {
        [Constructable]
        public FourPostBedAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(0x4C71, 1154131), 1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C70, 1154131), 1, 1, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C6E, 1154131), 0, 1, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C72, 1154131), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C73, 1154131), -1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C6F, 1154131), -1, 1, 0);
                    break;
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(0x4C69, 1154131), 1, 1, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C68, 1154131), 1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C6D, 1154131), 0, 1, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C6C, 1154131), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C6A, 1154131), 1, -1, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C6B, 1154131), 0, -1, 0);
                    break;
            }
        }

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;
            return true;
        }

        public FourPostBedAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed { get { return new FourPostBedDeed(); } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FourPostBedDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber { get { return 1154131; } } // Four Post Bed

        private DirectionType _Direction;

        [Constructable]
        public FourPostBedDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public FourPostBedDeed(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194)); // Choose a Facing:
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override BaseAddon Addon { get { return new FourPostBedAddon(_Direction); } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
