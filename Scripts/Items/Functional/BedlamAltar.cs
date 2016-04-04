using System;
using Server.Mobiles;

namespace Server.Items
{
    public class BedlamAltar : PeerlessAltar
    { 
        [Constructable]
        public BedlamAltar()
            : base(0x207E)
        { 
            this.BossLocation = new Point3D(106, 1615, 90);
            this.TeleportDest = new Point3D(101, 1623, 50);
            this.ExitDest = new Point3D(2068, 1372, -75);
        }

        public BedlamAltar(Serial serial)
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
                return new BedlamKey();
            }
        }
        public override Type[] Keys
        {
            get
            {
                return new Type[]
                {
                    typeof(LibrariansKey)
                };
            }
        }
        public override BasePeerless Boss
        {
            get
            {
                return new MonstrousInterredGrizzle();
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