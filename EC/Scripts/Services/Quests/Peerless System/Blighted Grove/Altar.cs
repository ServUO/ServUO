using System;
using Server.Mobiles;

namespace Server.Items
{
    public class BlightedGroveAltar : PeerlessAltar
    { 
        [Constructable]
        public BlightedGroveAltar()
            : base(0x207B)
        { 
            this.BossLocation = new Point3D(6483, 947, 23);
            this.TeleportDest = new Point3D(6518, 946, 36);
            this.ExitDest = new Point3D(587, 1641, -1);
        }

        public BlightedGroveAltar(Serial serial)
            : base(serial)
        {
        }

        public override int KeyCount
        {
            get
            {
                return 3;
            }
        }
        public override MasterKey MasterKey
        {
            get
            {
                return new BlightedGroveKey();
            }
        }
        public override Type[] Keys
        {
            get
            {
                return new Type[]
                {
                    typeof(DryadsBlessing)
                };
            }
        }
        public override BasePeerless Boss
        {
            get
            {
                return new LadyMelisande();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}