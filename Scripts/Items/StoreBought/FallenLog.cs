using Server.Gumps;

namespace Server.Items
{
    public class FallenLogAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new FallenLogDeed();

        [Constructable]
        public FallenLogAddon()
            : this(true)
        {
        }

        [Constructable]
        public FallenLogAddon(bool east)
            : base()
        {
            if (east)
            {
                AddComponent(new AddonComponent(0x0CF5), -1, 0, 0);
                AddComponent(new AddonComponent(0x0CF6), 0, 0, 0);
                AddComponent(new AddonComponent(0x0CF7), 1, 0, 0);
            }
            else
            {
                AddComponent(new AddonComponent(0x0CF4), 0, 0, 0);
                AddComponent(new AddonComponent(0x0CF3), 0, -1, 0);
            }
        }

        public FallenLogAddon(Serial serial)
            : base(serial)
        {
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
    }

    public class FallenLogDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1071088;  // Fallen Log
        public override BaseAddon Addon => new FallenLogAddon(m_East);

        private bool m_East;

        [Constructable]
        public FallenLogDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public FallenLogDeed(Serial serial)
            : base(serial)
        {
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(0, 1116332); // South 
            list.Add(1, 1116333); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            m_East = choice == 1;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

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
    }
}