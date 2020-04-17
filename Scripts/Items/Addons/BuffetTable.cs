using Server.Gumps;

namespace Server.Items
{
    public class BuffetTableAddon : BaseAddon
    {
        [Constructable]
        public BuffetTableAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new AddonComponent(40269), 0, 0, 0);
                    AddComponent(new AddonComponent(40270), 0, 1, 0);
                    AddComponent(new AddonComponent(40271), 1, 0, 0);
                    AddComponent(new AddonComponent(40272), 1, 1, 0);
                    AddComponent(new AddonComponent(40273), 2, 0, 0);
                    AddComponent(new AddonComponent(40274), 2, 1, 0);
                    AddComponent(new AddonComponent(40265), -2, 0, 0);
                    AddComponent(new AddonComponent(40266), -2, 1, 0);
                    AddComponent(new AddonComponent(40267), -1, 0, 0);
                    AddComponent(new AddonComponent(40268), -1, 1, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new AddonComponent(40264), 1, 2, 0);
                    AddComponent(new AddonComponent(40263), 0, 2, 0);
                    AddComponent(new AddonComponent(40262), 1, 1, 0);
                    AddComponent(new AddonComponent(40261), 0, 1, 0);
                    AddComponent(new AddonComponent(40260), 1, 0, 0);
                    AddComponent(new AddonComponent(40259), 0, 0, 0);
                    AddComponent(new AddonComponent(40258), 1, -1, 0);
                    AddComponent(new AddonComponent(40257), 0, -1, 0);
                    AddComponent(new AddonComponent(40256), 1, -2, 0);
                    AddComponent(new AddonComponent(40255), 0, -2, 0);
                    break;
            }
        }

        public BuffetTableAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new BuffetTableDeed();

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

    public class BuffetTableDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1157068;  // Buffet Table

        public override BaseAddon Addon => new BuffetTableAddon(_Direction);

        private DirectionType _Direction;

        [Constructable]
        public BuffetTableDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public BuffetTableDeed(Serial serial)
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
