namespace Server.Items
{
    public class MetallicLeatherDyeTub : DyeTub, Engines.VeteranRewards.IRewardItem
    {
        private bool m_IsRewardItem;

        [Constructable]
        public MetallicLeatherDyeTub()
        {
            LootType = LootType.Blessed;
        }

        public MetallicLeatherDyeTub(Serial serial)
            : base(serial)
        {
        }

        public override bool AllowDyables => false;
        public override bool AllowLeather => true;
        public override int TargetMessage => 1042416;  // Select the leather item to dye.
        public override int FailMessage => 1042418;  // You can only dye leather with this tub.
        public override int LabelNumber => 1153495;  // Metallic Leather Dye Tub
        public override CustomHuePicker CustomHuePicker => CustomHuePicker.MetallicDyeTub;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !Engines.VeteranRewards.RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            base.OnDoubleClick(from);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076221); // 5th Year Veteran Reward
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_IsRewardItem = reader.ReadBool();
        }
    }
}
