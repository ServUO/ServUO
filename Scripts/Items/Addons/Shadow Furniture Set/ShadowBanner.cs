using Server.Gumps;

namespace Server.Items
{
    public class ShadowBannerAddon : BaseAddon
    {
        [Constructable]
        public ShadowBannerAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(0x365C, 1076683), 1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x365D, 1076683), 0, 0, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(0x365E, 1076683), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x365F, 1076683), 0, 1, 0);
                    break;
            }
        }

        public ShadowBannerAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ShadowBannerDeed();

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

    public class ShadowBannerDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1076683;  // Shadow Banner

        public override bool ExcludeDeedHue => true;

        public override BaseAddon Addon => new ShadowBannerAddon(_Direction);

        private DirectionType _Direction;

        [Constructable]
        public ShadowBannerDeed()
            : base()
        {
            LootType = LootType.Blessed;
            Hue = 1908;
        }

        public ShadowBannerDeed(Serial serial)
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
                from.SendGump(new RewardOptionGump(this, 1076728)); // Please select your shadow banner position:
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
