using Server.Gumps;

namespace Server.Items
{
    public class CrystalBullStatueAddon : BaseAddon
    {
        [Constructable]
        public CrystalBullStatueAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(0x3600, 1076671), 1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x3601, 1076671), 0, 0, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(0x35FE, 1076671), 0, 1, 0);
                    AddComponent(new LocalizedAddonComponent(0x35FF, 1076671), 0, 0, 0);
                    break;
            }
        }

        public CrystalBullStatueAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new CrystalBullStatueDeed();

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

    public class CrystalBullStatueDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1076671;  // Crystal Bull Statue

        public override bool ExcludeDeedHue => true;

        public override BaseAddon Addon => new CrystalBullStatueAddon(_Direction);

        private DirectionType _Direction;

        [Constructable]
        public CrystalBullStatueDeed()
            : base()
        {
            LootType = LootType.Blessed;
            Hue = 1173;
        }

        public CrystalBullStatueDeed(Serial serial)
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
                from.SendGump(new RewardOptionGump(this, 1076726)); // Please select your crystal bull statue position
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
