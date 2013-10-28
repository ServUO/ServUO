using System;
using Server.Engines.VeteranRewards;

namespace Server.Items
{ 
    public class BloodyPentagramComponent : AddonComponent
    {
        public BloodyPentagramComponent(int itemID)
            : base(itemID)
        { 
        }

        public BloodyPentagramComponent(Serial serial)
            : base(serial)
        {
        }

        public override bool DisplayWeight
        {
            get
            {
                return false;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1080279;
            }
        }// Bloody Pentagram
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

    public class BloodyPentagramAddon : BaseAddon, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public BloodyPentagramAddon()
            : base()
        { 
            this.AddComponent(new BloodyPentagramComponent(0x1CF9), 0, 1, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1CF8), 0, 2, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1CF7), 0, 3, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1CF6), 0, 4, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1CF5), 0, 5, 0);

            this.AddComponent(new BloodyPentagramComponent(0x1CFB), 1, 0, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1CFA), 1, 1, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D09), 1, 2, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D08), 1, 3, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D07), 1, 4, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1CF4), 1, 5, 0);

            this.AddComponent(new BloodyPentagramComponent(0x1CFC), 2, 0, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D0A), 2, 1, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D11), 2, 2, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D10), 2, 3, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D06), 2, 4, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1CF3), 2, 5, 0);

            this.AddComponent(new BloodyPentagramComponent(0x1CFD), 3, 0, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D0B), 3, 1, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D12), 3, 2, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D0F), 3, 3, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D05), 3, 4, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1CF2), 3, 5, 0);

            this.AddComponent(new BloodyPentagramComponent(0x1CFE), 4, 0, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D0C), 4, 1, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D0D), 4, 2, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D0E), 4, 3, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D04), 4, 4, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1CF1), 4, 5, 0);

            this.AddComponent(new BloodyPentagramComponent(0x1CFF), 5, 0, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D00), 5, 1, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D01), 5, 2, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D02), 5, 3, 0);
            this.AddComponent(new BloodyPentagramComponent(0x1D03), 5, 4, 0);
        }

        public BloodyPentagramAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        { 
            get
            { 
                BloodyPentagramDeed deed = new BloodyPentagramDeed();
                deed.IsRewardItem = this.m_IsRewardItem;

                return deed; 
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
                this.InvalidateProperties();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
			
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
			
            this.m_IsRewardItem = reader.ReadBool();
        }
    }

    public class BloodyPentagramDeed : BaseAddonDeed, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public BloodyPentagramDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public BloodyPentagramDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1080384;
            }
        }// Bloody Pentagram
        public override BaseAddon Addon
        { 
            get
            { 
                BloodyPentagramAddon addon = new BloodyPentagramAddon();
                addon.IsRewardItem = this.m_IsRewardItem;

                return addon; 
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
                this.InvalidateProperties();
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            base.OnDoubleClick(from);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            if (this.m_IsRewardItem)
                list.Add(1076221); // 5th Year Veteran Reward
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
			
            this.m_IsRewardItem = reader.ReadBool();
        }
    }
}