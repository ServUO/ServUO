using System;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class Hold : Container
    {
        private BaseBoat m_Boat;
        public Hold(BaseBoat boat)
            : base(0x3EAE)
        {
            this.m_Boat = boat;
            this.Movable = false;
        }

        public Hold(Serial serial)
            : base(serial)
        {
        }

        public override bool IsDecoContainer
        {
            get
            {
                return false;
            }
        }
        public void SetFacing(Direction dir)
        {
            switch ( dir )
            {
                case Direction.East:
                    this.ItemID = 0x3E65;
                    break;
                case Direction.West:
                    this.ItemID = 0x3E93;
                    break;
                case Direction.North:
                    this.ItemID = 0x3EAE;
                    break;
                case Direction.South:
                    this.ItemID = 0x3EB9;
                    break;
            }
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            if (this.m_Boat == null || !this.m_Boat.Contains(from) || this.m_Boat.IsMoving)
                return false;

            return base.OnDragDrop(from, item);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p, byte gridloc)
        {
            if (this.m_Boat == null || !this.m_Boat.Contains(from) || this.m_Boat.IsMoving)
                return false;

            return base.OnDragDropInto(from, item, p, gridloc);
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (item != this && (this.m_Boat == null || !this.m_Boat.Contains(from) || this.m_Boat.IsMoving))
                return false;

            return base.CheckItemUse(from, item);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (this.m_Boat == null || !this.m_Boat.Contains(from) || this.m_Boat.IsMoving)
                return false;

            return base.CheckLift(from, item, ref reject);
        }

        public override void OnAfterDelete()
        {
            if (this.m_Boat != null)
                this.m_Boat.Delete();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_Boat == null || !this.m_Boat.Contains(from))
            {
                if (this.m_Boat.TillerMan != null)
                    this.m_Boat.TillerMan.Say(502490); // You must be on the ship to open the hold.
            }
            else if (this.m_Boat.IsMoving)
            {
                if (this.m_Boat.TillerMan != null)
                    this.m_Boat.TillerMan.Say(502491); // I can not open the hold while the ship is moving.
            }
            else
            {
                base.OnDoubleClick(from);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

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

                        if (this.m_Boat == null || this.Parent != null)
                            this.Delete();

                        this.Movable = false;

                        break;
                    }
            }
        }
    }
}