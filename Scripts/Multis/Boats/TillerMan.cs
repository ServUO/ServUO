using System;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class TillerMan : Item
    {
        private BaseBoat m_Boat;
        public TillerMan(BaseBoat boat)
            : base(0x3E4E)
        {
            this.m_Boat = boat;
            this.Movable = false;
        }

        public TillerMan(Serial serial)
            : base(serial)
        {
        }

        public void SetFacing(Direction dir)
        {
            switch ( dir )
            {
                case Direction.South:
                    this.ItemID = 0x3E4B;
                    break;
                case Direction.North:
                    this.ItemID = 0x3E4E;
                    break;
                case Direction.West:
                    this.ItemID = 0x3E50;
                    break;
                case Direction.East:
                    this.ItemID = 0x3E55;
                    break;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(this.m_Boat.Status);
        }

        public void Say(int number)
        {
            this.PublicOverheadMessage(MessageType.Regular, 0x3B2, number);
        }

        public void Say(int number, string args)
        {
            this.PublicOverheadMessage(MessageType.Regular, 0x3B2, number, args);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (this.m_Boat != null && this.m_Boat.ShipName != null)
                list.Add(1042884, this.m_Boat.ShipName); // the tiller man of the ~1_SHIP_NAME~
            else
                base.AddNameProperty(list);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (this.m_Boat != null && this.m_Boat.ShipName != null)
                this.LabelTo(from, 1042884, this.m_Boat.ShipName); // the tiller man of the ~1_SHIP_NAME~
            else
                base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_Boat != null && this.m_Boat.Contains(from))
                this.m_Boat.BeginRename(from);
            else if (this.m_Boat != null)
                this.m_Boat.BeginDryDock(from);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is MapItem && this.m_Boat != null && this.m_Boat.CanCommand(from) && this.m_Boat.Contains(from))
            {
                this.m_Boat.AssociateMap((MapItem)dropped);
            }

            return false;
        }

        public override void OnAfterDelete()
        {
            if (this.m_Boat != null)
                this.m_Boat.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version

            writer.Write(this.m_Boat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Boat = reader.ReadItem() as BaseBoat;

                        if (this.m_Boat == null)
                            this.Delete();

                        break;
                    }
            }
        }
    }
}