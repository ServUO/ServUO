using Server.Gumps;

namespace Server.Items
{
    public class CrystalTableAddon : BaseAddon
    {
        [Constructable]
        public CrystalTableAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(0x3607, 1076673), 1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x3608, 1076673), 0, 0, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(0x3605, 1076673), 0, 1, 0);
                    AddComponent(new LocalizedAddonComponent(0x3606, 1076673), 0, 0, 0);
                    break;
            }
        }

        public CrystalTableAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new CrystalTableDeed();

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

    public class CrystalTableDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1076673;  // Crystal Table

        public override bool ExcludeDeedHue => true;

        public override BaseAddon Addon => new CrystalTableAddon(_Direction);

        private DirectionType _Direction;

        [Constructable]
        public CrystalTableDeed()
            : base()
        {
            LootType = LootType.Blessed;
            Hue = 1173;
        }

        public CrystalTableDeed(Serial serial)
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
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this, 1076727)); // Please select your crystal table position
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
