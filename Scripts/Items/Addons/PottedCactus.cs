using System;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class RewardPottedCactus : Item, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public RewardPottedCactus()
            : this(Utility.RandomMinMax(0x1E0F, 0x1E14))
        { 
        }

        [Constructable]
        public RewardPottedCactus(int itemID)
            : base(itemID)
        { 
            this.Weight = 5.0;
        }

        public RewardPottedCactus(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
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

            writer.WriteEncodedInt(1); // version

            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 1:
                    this.m_IsRewardItem = reader.ReadBool();
                    break;
            }
        }
    }

    public class PottedCactusDeed : Item, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public PottedCactusDeed()
            : base(0x14F0)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
        }

        public PottedCactusDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1080407;
            }
        }// Potted Cactus Deed
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

            if (this.IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
                from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_IsRewardItem)
                list.Add(1076219); // 3rd Year Veteran Reward
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

        private class InternalGump : Gump
        { 
            private readonly PottedCactusDeed m_Cactus;
            public InternalGump(PottedCactusDeed cactus)
                : base(100, 200)
            {
                this.m_Cactus = cactus;

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                this.AddPage(0);
                this.AddBackground(0, 0, 425, 250, 0xA28);

                this.AddPage(1);
                this.AddLabel(45, 15, 0, "Choose a Potted Cactus:");

                this.AddItem(45, 75, 0x1E0F);
                this.AddButton(55, 50, 0x845, 0x846, 0x1E0F, GumpButtonType.Reply, 0);
				
                this.AddItem(105, 75, 0x1E10);
                this.AddButton(115, 50, 0x845, 0x846, 0x1E10, GumpButtonType.Reply, 0);

                this.AddItem(160, 75, 0x1E14);
                this.AddButton(175, 50, 0x845, 0x846, 0x1E14, GumpButtonType.Reply, 0);
				
                this.AddItem(220, 75, 0x1E11);
                this.AddButton(235, 50, 0x845, 0x846, 0x1E11, GumpButtonType.Reply, 0);
				
                this.AddItem(280, 75, 0x1E12);
                this.AddButton(295, 50, 0x845, 0x846, 0x1E12, GumpButtonType.Reply, 0);

                this.AddItem(340, 75, 0x1E13);
                this.AddButton(355, 50, 0x845, 0x846, 0x1E13, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Cactus == null || this.m_Cactus.Deleted)
                    return;		
				
                Mobile m = sender.Mobile;	
			
                if (info.ButtonID >= 0x1E0F && info.ButtonID <= 0x1E14)
                {
                    RewardPottedCactus cactus = new RewardPottedCactus(info.ButtonID);
                    cactus.IsRewardItem = this.m_Cactus.IsRewardItem;

                    if (!m.PlaceInBackpack(cactus))
                    {
                        cactus.Delete();
                        m.SendLocalizedMessage(1078837); // Your backpack is full! Please make room and try again.
                    }
                    else
                        this.m_Cactus.Delete();
                }
            }
        }
    }
}