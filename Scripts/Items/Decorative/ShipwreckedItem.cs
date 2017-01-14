using System;

namespace Server.Items
{
    public interface IShipwreckedItem
    {
        bool IsShipwreckedItem { get; set; }
    }

    public class ShipwreckedItem : Item, IDyable, IShipwreckedItem
    {
        private bool m_IsBarnacleItem;

        public ShipwreckedItem(int itemID, bool barnacle)
            : base(itemID)
        {
            m_IsBarnacleItem = barnacle;

            int weight = this.ItemData.Weight;

            if (weight >= 255)
                weight = 1;

            this.Weight = weight;
        }

        public override void OnSingleClick(Mobile from)
        {
            this.LabelTo(from, 1050039, String.Format("#{0}\t#1041645", this.LabelNumber));
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            if (m_IsBarnacleItem)
            {
                if (LabelNumber > 0)
                    list.Add(1151075, String.Format("#{0}", LabelNumber)); //barnacle covered ~1_token~
                else
                    list.Add(1151075, this.ItemData.Name); //barnacle covered ~1_token~

                list.Add(1041645); // recovered from a shipwreck
            }
            else
            {
                base.AddNameProperties(list);
                list.Add(1041645); // recovered from a shipwreck
            }
        }

        public ShipwreckedItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
            writer.Write(m_IsBarnacleItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_IsBarnacleItem = reader.ReadBool();
                    goto case 0;
                case 0:
                    break;
            }
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (this.Deleted)
                return false;

            if (this.ItemID >= 0x13A4 && this.ItemID <= 0x13AE)
            {
                this.Hue = sender.DyedHue;
                return true;
            }

            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        #region IShipwreckedItem Members

        public bool IsShipwreckedItem
        {
            get
            {
                return true;	//It's a ShipwreckedItem item.  'Course it's gonna be a Shipwreckeditem
            }
            set
            {
            }
        }
        #endregion
    }
}