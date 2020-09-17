using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.InstancedPeerless
{
    public class StygianDragonPlatform : PeerlessPlatform
    {
        public override Type KeyType => typeof(DraconicOrb);
        public override Type BossType => typeof(StygianDragon);

        [Constructable]
        public StygianDragonPlatform()
        {
            ExitLocation = new Point3D(367, 158, 0);

            // base
            AddHuedComponent(0x0709, 1, 1, 0);
            AddHuedComponent(0x0709, -1, -1, 0);
            AddHuedComponent(0x0709, 0, -1, 0);
            AddHuedComponent(0x0709, 1, -1, 0);
            AddHuedComponent(0x0709, 0, 0, 0);
            AddHuedComponent(0x0709, -1, 0, 0);
            AddHuedComponent(0x0709, -1, 1, 0);
            AddHuedComponent(0x0709, 0, 1, 0);

            // stairs
            AddHuedComponent(0x070B, 1, 0, 0);

            // blockers
            AddHuedComponent(0x21A4, 0, -1, 5);
            AddHuedComponent(0x21A4, -1, 0, 5);
            AddHuedComponent(0x21A4, 0, 1, 5);

            // floor cracks
            AddHuedComponent(0x1B07, 0, 0, 5);
            AddHuedComponent(0x1B05, 0, 0, 5);

            // floor
            AddHuedComponent(0x4338, -1, -1, 5);
            AddHuedComponent(0x4338, 1, -1, 5);
            AddHuedComponent(0x4338, 1, 1, 5);
            AddHuedComponent(0x4338, -1, 1, 5);
            AddHuedComponent(0x4339, 0, -1, 5);
            AddHuedComponent(0x4339, -1, 0, 5);
            AddHuedComponent(0x4339, 0, 1, 5);
        }

        public override void AddInstances()
        {
            AddInstance(0, -1, 7, Map.TerMur, new Point3D(80, 366, 0), new Point3D(61, 366, 20), new Rectangle2D(0, 286, 136, 176));
            AddInstance(-1, 0, 8, Map.TerMur, new Point3D(80, 564, 0), new Point3D(61, 564, 20), new Rectangle2D(0, 485, 136, 176));
            AddInstance(0, 1, 7, Map.TerMur, new Point3D(80, 772, 0), new Point3D(61, 772, 20), new Rectangle2D(0, 684, 136, 176));
        }

        public override void AddBraziers()
        {
            AddBrazier(-1, -1, 7);
            AddBrazier(1, -1, 8);
            AddBrazier(-1, 1, 8);
            AddBrazier(1, 1, 9);
        }

        private void AddHuedComponent(int itemId, int x, int y, int z)
        {
            AddonComponent component = new AddonComponent(itemId)
            {
                Hue = 0x2EF
            };
            AddComponent(component, x, y, z);
        }

        public StygianDragonPlatform(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            /*int version = */
            reader.ReadInt();
        }
    }
}