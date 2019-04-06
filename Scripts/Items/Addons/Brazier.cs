using System;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{ 
    public class RewardBrazier : Item, IRewardItem
    {
        private static readonly int[] m_Art = new int[]
        {
            0x19AA, 0x19BB
        };
        private bool m_IsRewardItem;
        private Item m_Fire;
        [Constructable]
        public RewardBrazier()
            : this(Utility.RandomList(m_Art))
        { 
        }

        [Constructable]
        public RewardBrazier(int itemID)
            : base(itemID)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 10.0;
        }

        public RewardBrazier(Serial serial)
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
        public override void OnDelete()
        {
            this.TurnOff();

            base.OnDelete();
        }

        public void TurnOff()
        {
            if (this.m_Fire != null)
            {
                this.m_Fire.Delete();
                this.m_Fire = null;
            }
        }

        public void TurnOn()
        {
            if (this.m_Fire == null)
                this.m_Fire = new Item();
 
            this.m_Fire.ItemID = 0x19AB;
            this.m_Fire.Movable = false;
            this.m_Fire.MoveToWorld(new Point3D(this.X, this.Y, this.Z + this.ItemData.Height + 2), this.Map);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (this.IsLockedDown)
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && house.IsCoOwner(from))
                {
                    if (this.m_Fire != null)
                        this.TurnOff();
                    else
                        this.TurnOn();
                }
                else
                    from.SendLocalizedMessage(502436); // That is not accessible.
            }
            else
                from.SendLocalizedMessage(502692); // This must be in a house and be locked down to work.
        }

        public override void OnLocationChange(Point3D old)
        {
            if (this.m_Fire != null)
                this.m_Fire.MoveToWorld(new Point3D(this.X, this.Y, this.Z + this.ItemData.Height), this.Map);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            if (this.m_IsRewardItem)
                list.Add(1076222); // 6th Year Veteran Reward
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
			
            writer.Write((bool)this.m_IsRewardItem);
            writer.Write((Item)this.m_Fire);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
			
            this.m_IsRewardItem = reader.ReadBool();
            this.m_Fire = reader.ReadItem();
        }
    }

    public class RewardBrazierDeed : Item, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public RewardBrazierDeed()
            : base(0x14F0)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
        }

        public RewardBrazierDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1080527;
            }
        }// Brazier Deed
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
                list.Add(1076222); // 6th Year Veteran Reward
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
            private readonly RewardBrazierDeed m_Brazier;
            public InternalGump(RewardBrazierDeed brazier)
                : base(100, 200)
            {
                this.m_Brazier = brazier;

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                this.AddPage(0);
                this.AddBackground(0, 0, 200, 200, 2600);

                this.AddPage(1);
                this.AddLabel(45, 15, 0, "Choose a Brazier:");

                this.AddItem(40, 75, 0x19AA);
                this.AddButton(55, 50, 0x845, 0x846, 0x19AA, GumpButtonType.Reply, 0);

                this.AddItem(100, 75, 0x19BB);
                this.AddButton(115, 50, 0x845, 0x846, 0x19BB, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Brazier == null || this.m_Brazier.Deleted)
                    return;

                Mobile m = sender.Mobile;

                if (info.ButtonID == 0x19AA || info.ButtonID == 0x19BB)
                {
                    RewardBrazier brazier = new RewardBrazier(info.ButtonID);
                    brazier.IsRewardItem = this.m_Brazier.IsRewardItem;

                    if (!m.PlaceInBackpack(brazier))
                    {
                        brazier.Delete();
                        m.SendLocalizedMessage(1078837); // Your backpack is full! Please make room and try again.
                    }
                    else
                        this.m_Brazier.Delete();
                }
            }
        }
    }
}