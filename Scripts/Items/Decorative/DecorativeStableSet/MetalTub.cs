using Server.Gumps;

namespace Server.Items
{
    public class MetalTubAddon : BaseAddon, IWaterSource
    {
        [Constructable]
        public MetalTubAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(0xA4E0, 1126234), 0, 0, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(0xA4E2, 1126234), 0, 0, 0);
                    break;
            }
        }

        public int Quantity
        {
            get { return 500; }
            set { }
        }

        public MetalTubAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new MetalTubDeed();

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

    public class MetalTubDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1126234;  // metal tub

        public override BaseAddon Addon => new MetalTubAddon(_Direction);

        private DirectionType _Direction;

        [Constructable]
        public MetalTubDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public MetalTubDeed(Serial serial)
            : base(serial)
        {
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
