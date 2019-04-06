using System;
using Server;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public class AllegiancePouch : Backpack, IRewardItem
    {
        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; }
        }

        public override int LabelNumber { get { return 1113953; } } //Allegiance Pouch

        [Constructable]
        public AllegiancePouch()
        {
            Hue = 2958;
            Weight = 1.0;
            LootType = LootType.Regular;

            DropItem(new OrderBanner());
            DropItem(new ChaosBanner());
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1113802); // 12th Year Veteran Reward
        }

        public AllegiancePouch(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}