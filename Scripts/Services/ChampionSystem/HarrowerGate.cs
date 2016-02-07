using System;

namespace Server.Items
{
    public class HarrowerGate : Moongate
    {
        private Mobile m_Harrower;
        public HarrowerGate(Mobile harrower, Point3D loc, Map map, Point3D targLoc, Map targMap)
            : base(targLoc, targMap)
        {
            this.m_Harrower = harrower;

            this.Dispellable = false;
            this.ItemID = 0x1FD4;
            this.Light = LightType.Circle300;

            this.MoveToWorld(loc, map);
        }

        public HarrowerGate(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1049498;
            }
        }// dark moongate
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Harrower);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Harrower = reader.ReadMobile();

                        if (this.m_Harrower == null)
                            this.Delete();

                        break;
                    }
            }

            if (this.Light != LightType.Circle300)
                this.Light = LightType.Circle300;
        }
    }
}