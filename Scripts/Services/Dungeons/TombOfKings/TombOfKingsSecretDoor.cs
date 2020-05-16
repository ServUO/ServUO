using System;

namespace Server.Items
{
    public class TombOfKingsSecretDoor : Item
    {
        public override int LabelNumber => 1020233;  // secret door

        private int m_ClosedId;

        [CommandProperty(AccessLevel.GameMaster)]
        public int ClosedId
        {
            get { return m_ClosedId; }
            set { m_ClosedId = value; }
        }

        [Constructable]
        public TombOfKingsSecretDoor(int closedId)
            : base(closedId)
        {
            Movable = false;

            m_ClosedId = closedId;
        }

        public override void OnDoubleClickDead(Mobile from)
        {
            Open(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            Open(from);
        }

        public void Open(Mobile from)
        {
            if (!from.InRange(this, 1))
                return;

            if (ItemID == ClosedId)
            {
                ItemID = 1; // no draw

                Timer.DelayCall(TimeSpan.FromSeconds(120.0), delegate
                {
                    ItemID = m_ClosedId;
                });
            }
        }

        public TombOfKingsSecretDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_ClosedId);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_ClosedId = reader.ReadInt();

            // make sure we don't get stuck at opened state before deserialize
            ItemID = m_ClosedId;
        }
    }
}
