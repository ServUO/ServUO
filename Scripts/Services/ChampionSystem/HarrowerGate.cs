namespace Server.Items
{
    public class HarrowerGate : Moongate
    {
        private Mobile m_Harrower;

        public override int LabelNumber => 1049498;// dark moongate

        public HarrowerGate(Mobile harrower, Point3D loc, Map map, Point3D targLoc, Map targMap)
            : base(targLoc, targMap)
        {
            m_Harrower = harrower;

            Dispellable = false;
            ItemID = 0x1FD4;
            Light = LightType.Circle300;

            MoveToWorld(loc, map);
        }

        public HarrowerGate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Harrower);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Harrower = reader.ReadMobile();

                        if (m_Harrower == null)
                            Delete();

                        break;
                    }
            }
        }
    }
}
