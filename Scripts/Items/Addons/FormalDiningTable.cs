using Server.Gumps;

namespace Server.Items
{
    public class FormalDiningTableAddon : BaseAddon
    {
        [Constructable]
        public FormalDiningTableAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new AddonComponent(40318), 3, 1, 0);
                    AddComponent(new AddonComponent(40317), 3, 0, 0);
                    AddComponent(new AddonComponent(40316), 2, 1, 0);
                    AddComponent(new AddonComponent(40315), 2, 0, 0);
                    AddComponent(new AddonComponent(40314), 1, 1, 0);
                    AddComponent(new AddonComponent(40313), 1, 0, 0);
                    AddComponent(new AddonComponent(40312), 0, 1, 0);
                    AddComponent(new AddonComponent(40311), 0, 0, 0);
                    AddComponent(new AddonComponent(40310), -1, 1, 0);
                    AddComponent(new AddonComponent(40309), -1, 0, 0);
                    AddComponent(new AddonComponent(40308), -2, 1, 0);
                    AddComponent(new AddonComponent(40307), -2, 0, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new AddonComponent(40306), 1, 3, 0);
                    AddComponent(new AddonComponent(40305), 0, 3, 0);
                    AddComponent(new AddonComponent(40304), 1, 2, 0);
                    AddComponent(new AddonComponent(40303), 0, 2, 0);
                    AddComponent(new AddonComponent(40302), 1, 1, 0);
                    AddComponent(new AddonComponent(40301), 0, 1, 0);
                    AddComponent(new AddonComponent(40300), 1, 0, 0);
                    AddComponent(new AddonComponent(40299), 0, 0, 0);
                    AddComponent(new AddonComponent(40298), 1, -1, 0);
                    AddComponent(new AddonComponent(40297), 0, -1, 0);
                    AddComponent(new AddonComponent(40295), 0, -2, 0);
                    AddComponent(new AddonComponent(40296), 1, -2, 0);
                    break;
            }
        }

        public FormalDiningTableAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new FormalDiningTableDeed();

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
    public class FormalDiningTableDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1157069;  // Formal Dining Table

        public override BaseAddon Addon => new FormalDiningTableAddon(_Direction);

        private DirectionType _Direction;

        [Constructable]
        public FormalDiningTableDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public FormalDiningTableDeed(Serial serial)
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
