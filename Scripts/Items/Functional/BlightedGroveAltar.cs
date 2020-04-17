using Server.Mobiles;
using System;

namespace Server.Items
{
    public class BlightedGroveAltar : PeerlessAltar
    {
        public override int KeyCount => 3;
        public override MasterKey MasterKey => new BlightedGroveKey();

        public override Type[] Keys => new Type[]
{
            typeof( DryadsBlessing )
};

        public override BasePeerless Boss => new LadyMelisande();

        [Constructable]
        public BlightedGroveAltar() : base(0x207B)
        {
            BossLocation = new Point3D(6483, 947, 23);
            TeleportDest = new Point3D(6518, 946, 36);
            ExitDest = new Point3D(587, 1641, -1);
        }

        public override Rectangle2D[] BossBounds => m_Bounds;

        private readonly Rectangle2D[] m_Bounds = new Rectangle2D[]
        {
            new Rectangle2D(6456, 922, 84, 47),
        };

        public BlightedGroveAltar(Serial serial) : base(serial)
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

            int version = reader.ReadInt();
        }
    }
}