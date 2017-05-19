using System;
using Server;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    [Flipable(0x42CD, 0x42CE)]
    public class OrderBanner : Item, IRewardItem
    {
        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; }
        }

        [Constructable]
        public OrderBanner()
            : base(0x42CD)
        {
            Name = "Order Banner";
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public OrderBanner(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }
    }

    [Flipable(0x42CB, 0x42CC)]
    public class ChaosBanner : Item, IRewardItem
    {
        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; }
        }

        [Constructable]
        public ChaosBanner()
            : base(0x42CB)
        {
            Name = "Chaos Banner";
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public ChaosBanner(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }
    }
}