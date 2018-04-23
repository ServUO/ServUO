using System;
using System.Collections;
using Server.Items;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Engines.VeteranRewards;

namespace Server.Custom
{
    public class LinkedBag : Bag, IRewardItem
    {
        private LinkedBag mate;
        [CommandProperty(AccessLevel.GameMaster)]
        public LinkedBag Mate { get { return mate; } set { mate = value; } }

        private bool m_IsRewardItem;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }

        [Constructable]
        public LinkedBag()
            : base()
        {
            Name = "Linked Bag";
            Weight = 2.0;
        }

        public LinkedBag(Serial serial)
            : base(serial)
        {
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                from.SendMessage("This does not belong to you!!");
                return;
            }
            base.OnDoubleClick(from);
        }
        
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                from.SendMessage("This does not belong to you!!");
                return false;
            }
            return OnDragDropInto(from, dropped, new Point3D(20, 100, 0));
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                from.SendMessage("This does not belong to you!!");
                return false;
            }
            
            if (item is LinkedBag) return false;
            if (mate == null) return base.OnDragDropInto(from, item, p);
            if (!mate.CheckHold(from, item, true, true))
                return false;
            try
            {
                mate.DropItem(item);
            }
            catch
            {
                from.SendMessage("Unable to do that.");
                return false;
            }
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((bool)m_IsRewardItem);
            writer.Write(mate);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_IsRewardItem = reader.ReadBool();
            mate = reader.ReadItem() as LinkedBag;
        }
    }

    public class LinkedBagsBag : Bag
    {
        public override string DefaultName
        {
            get { return "a Bag of Linked Bags"; }
        }

        private bool m_IsRewardItem;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }

        [Constructable]
        public LinkedBagsBag() : base()
        {
            Movable = true;
            Hue = Utility.RandomBlueHue();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                from.SendMessage("This does not belong to you!!");
                return;
            }
                LinkedBag bagA = new LinkedBag();
                LinkedBag bagB = new LinkedBag();
                bagA.Mate = bagB;
                bagB.Mate = bagA;
                bagA.Name = string.Format("{0}'s Linked Bag", from.Name);
                bagB.Name = string.Format("{0}'s Linked Bag", from.Name);
                from.AddToBackpack(bagA);
                from.AddToBackpack(bagB);
                this.Delete();
        }

        public LinkedBagsBag(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((bool)m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_IsRewardItem = reader.ReadBool();
        }
    }
}
