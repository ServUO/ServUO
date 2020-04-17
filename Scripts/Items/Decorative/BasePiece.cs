namespace Server.Items
{
    public class BasePiece : Item
    {
        private BaseBoard m_Board;
        public BasePiece(int itemID, BaseBoard board)
            : base(itemID)
        {
            m_Board = board;
        }

        public BasePiece(Serial serial)
            : base(serial)
        {
        }

        public BaseBoard Board
        {
            get
            {
                return m_Board;
            }
            set
            {
                m_Board = value;
            }
        }
        public override bool IsVirtualItem => true;
        public override bool CanTarget => false;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(m_Board);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Board = (BaseBoard)reader.ReadItem();

                        if (m_Board == null || Parent == null)
                            Delete();

                        break;
                    }
            }
        }

        public override bool OnDragLift(Mobile from)
        {
            if (m_Board == null || m_Board.Deleted)
            {
                Delete();
                return false;
            }
            else if (!IsChildOf(m_Board))
            {
                m_Board.DropItem(this);
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
            return (target == m_Board && p.X != -1 && p.Y != -1 && base.DropToItem(from, target, p));
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
