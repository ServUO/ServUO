using Server.Gumps;

namespace Server.Items
{
    public class SkullTiledFloorAddon : BaseAddon
    {
        [Constructable]
        public SkullTiledFloorAddon(bool east)
            : base()
        {
            if (!east)
            {
                AddComponent(new LocalizedAddonComponent(0xA34F, 1125827), 0, 0, 0);
                AddComponent(new LocalizedAddonComponent(0xA350, 1125827), -1, 0, 0);
                AddComponent(new LocalizedAddonComponent(0xA34E, 1125827), 1, 0, 0);
                AddComponent(new LocalizedAddonComponent(0xA34B, 1125827), 1, 1, 0);
                AddComponent(new LocalizedAddonComponent(0xA34C, 1125827), 0, 1, 0);
                AddComponent(new LocalizedAddonComponent(0xA34D, 1125827), -1, 1, 0);
                AddComponent(new LocalizedAddonComponent(0xA351, 1125827), 1, -1, 0);
                AddComponent(new LocalizedAddonComponent(0xA352, 1125827), 0, -1, 0);
                AddComponent(new LocalizedAddonComponent(0xA353, 1125827), -1, -1, 0);
            }
            else
            {
                AddComponent(new LocalizedAddonComponent(0xA358, 1125827), 0, 0, 0);
                AddComponent(new LocalizedAddonComponent(0xA357, 1125827), 0, 1, 0);
                AddComponent(new LocalizedAddonComponent(0xA359, 1125827), 0, -1, 0);
                AddComponent(new LocalizedAddonComponent(0xA354, 1125827), 1, 1, 0);
                AddComponent(new LocalizedAddonComponent(0xA355, 1125827), 1, 0, 0);
                AddComponent(new LocalizedAddonComponent(0xA356, 1125827), 1, -1, 0);
                AddComponent(new LocalizedAddonComponent(0xA35A, 1125827), -1, 1, 0);
                AddComponent(new LocalizedAddonComponent(0xA35B, 1125827), -1, 0, 0);
                AddComponent(new LocalizedAddonComponent(0xA35C, 1125827), -1, -1, 0);
            }
        }

        public SkullTiledFloorAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new SkullTiledFloorAddonDeed();

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SkullTiledFloorAddonDeed : BaseAddonDeed, IRewardOption
    {
        private bool m_East;

        [Constructable]
        public SkullTiledFloorAddonDeed()
        {
        }

        public SkullTiledFloorAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1159020;  // Skull Tiled Floor

        public override BaseAddon Addon => new SkullTiledFloorAddon(m_East);

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.   
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(1, 1150124); // South
            list.Add(2, 1150123); // East
        }

        public void OnOptionSelected(Mobile from, int option)
        {
            switch (option)
            {
                case 1:
                    m_East = false;
                    break;
                case 2:
                    m_East = true;
                    break;
            }

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}
