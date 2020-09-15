using System;

namespace Server.Items
{
    public class CorgulIslandMap : SeaChart
    {
        private Point3D m_DestinationPoint;

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D DestinationPoint => m_DestinationPoint;

        [CommandProperty(AccessLevel.GameMaster)]
        public CorgulAltar Altar { get; set; }

        [Constructable]
        public CorgulIslandMap(Point3D pnt, CorgulAltar altar)
        {
            Name = "Island Map";
            m_DestinationPoint = pnt;
            AddWorldPin(pnt.X, pnt.Y);
            Protected = true;

            Altar = altar;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Altar != null && Altar.DeadLine != DateTime.MinValue && Altar.DeadLine > DateTime.UtcNow)
            {
                list.Add(1072516, string.Format("map of the world\t{0}", (int)(Altar.DeadLine - DateTime.UtcNow).TotalSeconds)); // ~1_name~ will expire in ~2_val~ seconds!
            }
        }

        public CorgulIslandMap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
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
