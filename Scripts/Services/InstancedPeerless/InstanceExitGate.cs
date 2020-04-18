using Server.Gumps;

namespace Server.Items
{
    public class InstanceExitGate : Item
    {
        private Map m_MapDest;
        private Point3D m_LocDest;

        public override int LabelNumber => 1113495; // (Exit)

        public override bool ForceShowProperties => true;

        [CommandProperty(AccessLevel.GameMaster)]
        public Map MapDest { get { return m_MapDest; } set { m_MapDest = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D LocDest { get { return m_LocDest; } set { m_LocDest = value; } }

        [Constructable]
        public InstanceExitGate()
            : this(Map.Internal, Point3D.Zero)
        {
        }

        [Constructable]
        public InstanceExitGate(Map mapDest, Point3D locDest)
            : base(0xF6C)
        {
            m_MapDest = mapDest;
            m_LocDest = locDest;

            Movable = false;
            Hue = 0x488;
            Light = LightType.Circle300;
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (!m.HasGump(typeof(ConfirmExitInstanceGump)))
                m.SendGump(new ConfirmExitInstanceGump(this));

            return base.OnMoveOver(m);
        }

        public InstanceExitGate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_MapDest);
            writer.Write(m_LocDest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            /*int version = */
            reader.ReadInt();

            m_MapDest = reader.ReadMap();
            m_LocDest = reader.ReadPoint3D();
        }
    }
}