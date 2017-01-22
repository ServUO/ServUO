using Server;
using System;

namespace Server.Items
{
    public class CorgulIslandMap : SeaChart
    {
        private Point3D m_DestinationPoint;

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D DestinationPoint { get { return m_DestinationPoint; } }

        [Constructable]
        public CorgulIslandMap(Point3D pnt)
        {
            Name = "Island Map";
            m_DestinationPoint = pnt;
            AddWorldPin(pnt.X, pnt.Y);
            Protected = true;
        }

        public CorgulIslandMap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_DestinationPoint);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_DestinationPoint = reader.ReadPoint3D();
        }
    }
}