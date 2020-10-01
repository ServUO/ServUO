using Server.Gumps;

namespace Server.Items
{
    public class OrnateBedAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new OrnateBedDeed();

        [Constructable]
        public OrnateBedAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(0x4C7F, 1154133), 1, 1, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C7A, 1154133), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C7C, 1154133), -1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C7E, 1154133), 0, 1, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C7B, 1154133), 1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C7D, 1154133), -1, 1, 0);
                    break;
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(0x4C77, 1154133), 0, -1, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C79, 1154133), 0, 1, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C74, 1154133), 1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C76, 1154133), 1, -1, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C75, 1154133), 1, 1, 0);
                    AddComponent(new LocalizedAddonComponent(0x4C78, 1154133), 0, 0, 0);
                    break;
            }
        }

        public OrnateBedAddon(Serial serial)
            : base(serial)
        {
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

    [Furniture]
    public class OrnateBedDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1154133;  // Ornate Bed

        private DirectionType _Direction;

        [Constructable]
        public OrnateBedDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public OrnateBedDeed(Serial serial)
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

        public override BaseAddon Addon => new OrnateBedAddon(_Direction);

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
