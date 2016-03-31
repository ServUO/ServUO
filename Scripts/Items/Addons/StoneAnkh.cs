using System;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{ 
    public class StoneAnkhComponent : AddonComponent
    {
        public StoneAnkhComponent(int itemID)
            : base(itemID)
        {
            this.Weight = 1.0;
        }

        public StoneAnkhComponent(Serial serial)
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
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            if (this.Addon is StoneAnkh && ((StoneAnkh)this.Addon).IsRewardItem)
                list.Add(1076221); // 5th Year Veteran Reward
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

    public class StoneAnkh : BaseAddon, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public StoneAnkh()
            : this(true)
        {
        }

        [Constructable]
        public StoneAnkh(bool east)
            : base()
        { 
            if (east)
            {
                this.AddComponent(new StoneAnkhComponent(0x2), 0, 0, 0);
                this.AddComponent(new StoneAnkhComponent(0x3), 0, -1, 0);
            }
            else
            {
                this.AddComponent(new StoneAnkhComponent(0x5), 0, 0, 0);
                this.AddComponent(new StoneAnkhComponent(0x4), -1, 0, 0);
            }
        }

        public StoneAnkh(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        { 
            get
            { 
                StoneAnkhDeed deed = new StoneAnkhDeed();
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
        public override void OnChop(Mobile from)
        {
            from.SendLocalizedMessage(500489); // You can't use an axe on that.
            return;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            if (Core.ML && this.m_IsRewardItem)
                list.Add(1076221); // 5th Year Veteran Reward
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (from.InRange(this.Location, 2))
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);  
			
                if (house != null && house.IsOwner(from))
                {
                    from.CloseGump(typeof(RewardDemolitionGump));
                    from.SendGump(new RewardDemolitionGump(this, 1049783)); // Do you wish to re-deed this decoration?
                }
                else
                    from.SendLocalizedMessage(1049784); // You can only re-deed this decoration if you are the house owner or originally placed the decoration.
            }
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
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

    public class StoneAnkhDeed : BaseAddonDeed, IRewardItem
    {
        private bool m_East;
        private bool m_IsRewardItem;
        [Constructable]
        public StoneAnkhDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public StoneAnkhDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1049773;
            }
        }// deed for a stone ankh
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
        public override BaseAddon Addon
        { 
            get
            { 
                StoneAnkh addon = new StoneAnkh(this.m_East);
                addon.IsRewardItem = this.m_IsRewardItem;

                return addon; 
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

        private void SendTarget(Mobile m)
        {
            base.OnDoubleClick(m);
        }

        private class InternalGump : Gump
        {
            private readonly StoneAnkhDeed m_Deed;
            public InternalGump(StoneAnkhDeed deed)
                : base(150, 50)
            {
                this.m_Deed = deed;				
				
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                this.AddPage(0);

                this.AddBackground(0, 0, 300, 150, 0xA28);

                this.AddItem(90, 30, 0x4);
                this.AddItem(112, 30, 0x5);
                this.AddButton(50, 35, 0x867, 0x869, (int)Buttons.South, GumpButtonType.Reply, 0); // South

                this.AddItem(170, 30, 0x2);
                this.AddItem(192, 30, 0x3);
                this.AddButton(145, 35, 0x867, 0x869, (int)Buttons.East, GumpButtonType.Reply, 0); // East
            }

            private enum Buttons
            {
                Cancel,
                South,
                East
            }
            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Deed == null || this.m_Deed.Deleted)
                    return;
					
                if (info.ButtonID != (int)Buttons.Cancel)
                {
                    this.m_Deed.m_East = (info.ButtonID == (int)Buttons.East);
                    this.m_Deed.SendTarget(sender.Mobile);
                }
            }
        }
    }
}