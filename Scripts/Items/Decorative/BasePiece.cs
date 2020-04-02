using System;

namespace Server.Items
{
    public class BasePiece : Item
    {
        private BaseBoard m_Board;
        public BasePiece(int itemID, BaseBoard board)
            : base(itemID)
        {
            this.m_Board = board;
        }

        public BasePiece(Serial serial)
            : base(serial)
        {
        }

        public BaseBoard Board
        {
            get
            {
                return this.m_Board;
            }
            set
            {
                this.m_Board = value;
            }
        }
        public override bool IsVirtualItem
        {
            get
            {
                return true;
            }
        }
        public override bool CanTarget
        {
            get
            {
                return false;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write(this.m_Board);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Board = (BaseBoard)reader.ReadItem();

                        if (this.m_Board == null || this.Parent == null)
                            this.Delete();

                        break;
                    }
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            if (this.m_Board == null || this.m_Board.Deleted)
                this.Delete();
            else if (!this.IsChildOf(this.m_Board))
                this.m_Board.DropItem(this);
            else
                base.OnSingleClick(from);
        }

        public override bool OnDragLift(Mobile from)
        {
            if (this.m_Board == null || this.m_Board.Deleted)
            {
                this.Delete();
                return false;
            }
            else if (!this.IsChildOf(this.m_Board))
            {
                this.m_Board.DropItem(this);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            return false;
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            return (target == this.m_Board && p.X != -1 && p.Y != -1 && base.DropToItem(from, target, p));
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            return false;
        }

        public override int GetLiftSound(Mobile from)
        {
            return -1;
        }
    }
}