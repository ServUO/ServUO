using Server.Gumps;

namespace Server.Items
{
    public class BullTapestryAddon : BaseAddon, IDyable
    {
        [Constructable]
        public BullTapestryAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(19619, 1154148), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(19618, 1154148), 0, 1, 0);
                    break;
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(19617, 1154148), 1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(19616, 1154148), 0, 0, 0);
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

        public BullTapestryAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new BullTapestryDeed();

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BullTapestryDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1154148;  // Bull Tapestry

        private DirectionType _Direction;

        [Constructable]
        public BullTapestryDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public BullTapestryDeed(Serial serial)
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

        public override BaseAddon Addon => new BullTapestryAddon(_Direction);

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
