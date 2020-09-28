namespace Server.Items
{
    public interface IShipwreckedItem
    {
        bool IsShipwreckedItem { get; set; }
    }

    public class ShipwreckedItem : Item, IDyable, IShipwreckedItem, IFlipable
    {
        private bool m_IsBarnacleItem;

        public ShipwreckedItem(int itemID, bool barnacle)
            : base(itemID)
        {
            m_IsBarnacleItem = barnacle;

            int weight = ItemData.Weight;

            if (weight >= 255 || weight <= 0)
                weight = 1;

            Weight = weight;
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            if (m_IsBarnacleItem)
            {
                if (LabelNumber > 0)
                    list.Add(1151075, string.Format("#{0}", LabelNumber)); //barnacle covered ~1_token~
                else
                    list.Add(1151075, ItemData.Name); //barnacle covered ~1_token~

                list.Add(1041645); // recovered from a shipwreck
            }
            else
            {
                base.AddNameProperties(list);
                list.Add(1041645); // recovered from a shipwreck
            }
        }

        public virtual void OnFlip(Mobile m)
        {
            switch (ItemID)
            {
                case 0x0E9F: ItemID = 0x0EC8; break;
                case 0x0EC8: ItemID = 0x0E9F; break;
                case 0x0EC9: ItemID = 0x0EE7; break;
                case 0x0EE7: ItemID = 0x0EC9; break;
                case 0x0EA1: ItemID++; break;
                case 0x0EA2: ItemID--; break;
                case 0x0EA3: ItemID++; break;
                case 0x0EA4: ItemID--; break;
                case 0x0EA5: ItemID = 0x0EA7; break;
                case 0x0EA6: ItemID = 0x0EA8; break;
                case 0x0EA7: ItemID = 0x0EA5; break;
                case 0x0EA8: ItemID = 0x0EA6; break;
            }
        }

        public ShipwreckedItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
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
            if (Deleted)
                return false;

            if (ItemID >= 0x13A4 && ItemID <= 0x13AE)
            {
                Hue = sender.DyedHue;
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
