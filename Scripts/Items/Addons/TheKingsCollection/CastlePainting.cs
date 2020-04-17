using Server.Gumps;

namespace Server.Items
{
    public class CastlePaintingAddon : BaseAddon
    {
        [Constructable]
        public CastlePaintingAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(0x4C21, 1154184), 0, 0, 0);
                    break;
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(0x4C20, 1154184), 0, 0, 0);
                    break;
            }
        }

        public CastlePaintingAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new CastlePaintingDeed();

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

    public class CastlePaintingDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1154184;  // Castle Painting

        private DirectionType _Direction;

        [Constructable]
        public CastlePaintingDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public CastlePaintingDeed(Serial serial)
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

        public override BaseAddon Addon => new CastlePaintingAddon(_Direction);

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
