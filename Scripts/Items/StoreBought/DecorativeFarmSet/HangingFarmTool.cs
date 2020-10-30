using Server.Gumps;

namespace Server.Items
{
    public class HangingFarmToolAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new HangingFarmToolDeed();

        [Constructable]
        public HangingFarmToolAddon()
            : this(true)
        {
        }

        [Constructable]
        public HangingFarmToolAddon(bool south)
        {
            if (south)
            {
                AddComponent(new AddonComponent(0xA32A), 0, 0, 0);
                AddComponent(new AddonComponent(0xA329), 1, 0, 0);
            }
            else
            {
                AddComponent(new AddonComponent(0xA302), 0, 0, 0);
                AddComponent(new AddonComponent(0xA303), 0, -1, 0);
            }
        }

        public HangingFarmToolAddon(Serial serial)
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
            reader.ReadInt();
        }
    }

    public class HangingFarmToolDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon => new HangingFarmToolAddon(m_South);
        public override int LabelNumber => 1159045;  // hanging farm tools

        private bool m_South;

        public void GetOptions(RewardOptionList list)
        {
            list.Add(0, 1116332); // South 
            list.Add(1, 1116333); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            m_South = choice == 0;

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

        [Constructable]
        public HangingFarmToolDeed()
        {
        }

        public HangingFarmToolDeed(Serial serial)
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
            reader.ReadInt();
        }
    }
}
