using Server.Gumps;

namespace Server.Items
{
    public class DungeonBullAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new DungeonBullDeed();

        [Constructable]
        public DungeonBullAddon()
            : this(true)
        {
        }

        [Constructable]
        public DungeonBullAddon(bool east)
            : base()
        {
            if (east)
            {
                AddComponent(new AddonComponent(42247), 0, 0, 0);
                AddComponent(new AddonComponent(42246), 0, 1, 0);
                AddComponent(new AddonComponent(42245), 1, 1, 0);
                AddComponent(new AddonComponent(42249), 1, 0, 0);
                AddComponent(new AddonComponent(42248), 1, -1, 0);
            }
            else
            {
                AddComponent(new AddonComponent(42237), 0, 0, 0);
                AddComponent(new AddonComponent(42236), 1, 0, 0);
                AddComponent(new AddonComponent(42235), 1, 1, 0);
                AddComponent(new AddonComponent(42239), 0, 1, 0);
                AddComponent(new AddonComponent(42238), -1, 1, 0);
            }
        }

        public DungeonBullAddon(Serial serial)
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

    public class DungeonBullDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1159467;  // Dungeon Bull
        public override BaseAddon Addon => new DungeonBullAddon(m_East);

        private bool m_East;

        [Constructable]
        public DungeonBullDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public DungeonBullDeed(Serial serial)
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
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
