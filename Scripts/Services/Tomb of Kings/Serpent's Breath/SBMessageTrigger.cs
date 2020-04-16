namespace Server.Items
{
    public class SBMessageTrigger : Item
    {
        private FlameOfOrder m_Flame;

        [Constructable]
        public SBMessageTrigger(FlameOfOrder flame)
            : base(0x1BC3)
        {
            m_Flame = flame;

            Name = "Serpent's Breath Message Trigger";

            Movable = false;
            Visible = false;
        }

        public SBMessageTrigger(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.Location.Y < Location.Y && !m_Flame.Deleted && m_Flame.Visible)
            {
                m.SendLocalizedMessage(1112225); // The Serpent's Breath burns brighter than ever, blocking your escape! You shall have to venture further into the tomb in search of an exit.
            }

            return base.OnMoveOver(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Flame);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Flame = reader.ReadItem() as FlameOfOrder;
        }
    }
}